using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using NAudio.Wave.SampleProviders;

namespace Host_Alpha
{
    //  TODO: we need an array of synth modules,
    //                      each module will correspond to a song's movment
    //  TODO: we need a synth model object which we can update and will play sounds.
    //  TODO: we are currently writing this class to be used with the melody app.
    //                      we either need to make this class be useable for all three phone contexts,
    //                          or we will just write separate Slot classes for each context.
    //  TODO: this class needs to handle the creation of the Melody, Percussion, and Sampler contexts.
    class MelodySlot : Slot
    {

        /// <summary> Oscillator to manage play requests and configuration commands. </summary>
        private Oscillator oscillator = new Oscillator(44100, 0.25f); //default sample rate and amplitude

        //  TODO: we probably don't need this any more

        /// <summary>   The number of movements associated with the song    </summary>
        private int _nMovements = -1;

        /// <summary>ip address of the phone</summary>
        private string _phoneAddress = "";

        private IPAddress _phoneIP = null;

        // TODO: "_songName" may be an inappropriate descriptor for the Percussion and Sampler instruments.
        /// <summary>   name of the song    </summary>
        private string _songName = "";

        /// <summary>assigned by the host depending on how many phones are connected.
        /// used exclusively to receive play signals from the phone over a UDP socket</summary>
        private int _playReceivingPort = -1;

        /// <summary>assigned by the host, used exclusively for receiving signals which will update the state of the synth.
        /// TCP</summary>
        private int _syncReceivingPort = -1;

        //private IPEndPoint _receivingEndPoint = null;

        // the port the phone will be listening on,
        //  all phones will be listening on 9999 for now???
        private int _sendingPort = 10000;

        /// <summary>used for the final step of the UDP handshake between the host and the phone</summary>
     //   private UdpClient _sendingSocket = null;

        /// <summary> used to receive real-time play signals from the phone </summary>
        private UdpClient _receivingSocket = null;

        /// <summary>used to receive non real-time signlas from the phone concerning the state of the phone's sound engine</summary>
        private TcpListener _syncServer = null;

        //  TODO: this may not be neccessary....
        // connection number
        private int _id = -1;

        // TODO: stop condition is unimplemented.
        private bool stop = false;

        public MelodySlot(string type, string address, int playReceivingPort, int syncReceivingPort, int id, string songName)
        {
            _type = type;
            _phoneAddress = address;
            _songName = songName;
            _playReceivingPort = playReceivingPort;
            _syncReceivingPort = syncReceivingPort;
            _id = id;                       // TODO: may get scrapped.

            //  TODO: this initialization is suspect, 
            //          providing an address might be screwing with us..
            _sendingSocket = new UdpClient(_phoneAddress, _sendingPort);
            //  TODO: it would be tidy if the receving socket for any given slot would only receive data from the phone it was assigned to...
            //_receivingSocket = new UdpClient(_phoneAddress, _playReceivingPort);    // SUSPect
            _receivingSocket = new UdpClient(_playReceivingPort);
        }

        /// <summary>
        /// sends a message back to phone with assigned ports,
        /// and then starts threads for UDP and TCP signals
        /// </summary>
        public void setUpConnection()
        {
            string startPlayMessage = "hey, host is ready for play signals on port: " + _playReceivingPort.ToString() + ", and sync signals on port: " + _syncReceivingPort.ToString();
            byte[] startPlayMessageBytes = Encoding.ASCII.GetBytes(startPlayMessage);
            _sendingSocket.Connect(_phoneAddress, _sendingPort);
            _sendingSocket.Send(startPlayMessageBytes, startPlayMessageBytes.Length);

            _syncServer = new TcpListener(IPAddress.Any, _syncReceivingPort);
            _syncServer.Start();

            //  TODO: Suspect
            //_receivingSocket.Connect(_phoneAddress, _playReceivingPort);

            Thread playThread = new Thread(new ThreadStart(receivePlaySignal));
            Thread syncThread = new Thread(new ThreadStart(receiveSyncSignal));

           // listener.OnConnect(this);

            playThread.Start();
            syncThread.Start();
        }

        //  TODO: would having separate receive methods for the three phone contexts reduce latency????
        //  TODO: this method is unimplemented.
        //  TODO: how are we going to handle the three phone contexts here???,
        //                      we'll start with the Melody Maker since it will probably be the simplest to trigger.
        /// <summary>
        /// method to receive play/control signals from the phone
        /// </summary>
        public void receivePlaySignal()
        {

            //IPEndPoint ep = new IPEndPoint(IPAddress.Parse(_phoneAddress), _playReceivingPort);
            //  TODO: we need to make this endpoint more specific
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("[Melody Slot] Waiting for Play Signals for song " + _songName + ", on port " + _playReceivingPort + " from IP "  + _phoneAddress + "\n");

            //  TODO: how/when are we going to break out of this loop...?
            while (true)
            {
                //  TODO: running this call in a loop causes a stack overflow, or the like
                // _receivingSocket.BeginReceive(new AsyncCallback(receiveSignalCallback), this);
                Byte[] playMessageBytes = _receivingSocket.Receive(ref ep);
                String playMessageString = Encoding.ASCII.GetString(playMessageBytes);

                //Console.WriteLine("receivePlaySignal was called");
                //Console.WriteLine(Encoding.ASCII.GetString(playMessageBytes));

                // parse the incoming message
                // play signal
                if (playMessageString.StartsWith("p"))
                {
                    try
                    {
                       // listener.OnPlayWave(oscillator.PreparePlay(500));
                   //listener.OnPlayWave(oscillator.PreparePlay(Convert.ToDouble(playMessageString.Substring(6))), this);
                   listener.OnPlayWave(oscillator.PreparePlay(Convert.ToDouble(playMessageString.Substring(6))));
                   //     listener.OnPlaySample(PercussionManager.ShakerType.Forward);
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("falied to convert frequency to double");
                    }
                }
                // stop signal
                else if (playMessageString.StartsWith("s"))
                {
                    listener.OnStopWave(oscillator.PrepareStop());
                }
                //  TODO: movement signals will require a response...
                // movment signal
                else
                {
                    //  TODO: if we have written the phone side correctly,
                    //                      there should be no possible way that this case has a string that does not start with movement

                }
                ////  TODO: receive, then parse the signal
                //int frequency = 1500;
                //oscillator.StopWave(); //stop any waves that were previously playing
                //oscillator.StartWave(frequency);
            }
        }

        /// <summary>
        /// receive synth sync signals from the phone,
        /// in order to have the phone's synth state mirror this synth state.
        /// This method does not need to be real-time.
        /// https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener(v=vs.110).aspx
        /// </summary>
        /// <param name="signal">a message with info on which synth parameter to update</param>
        public void receiveSyncSignal()
        {
            TcpClient client = _syncServer.AcceptTcpClient();
            Console.WriteLine("Connection made with client");

            while (true)
            {

                //---get the incoming data through a network stream---
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                //---read incoming stream---
                int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

                //---convert the data received into a string---
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received : " + dataReceived);

                if (dataReceived != null)
                {
                    processUpdateMessage(dataReceived);
                }
            }
        }

        private void processUpdateMessage(string message)
        {
            //  ------------------------------------------------ osc 1 updates -----------------------------------------------------------------------------------------------------------
            if (message == "osc1On:")
            {
                //TODO: get boolean
                oscillator.SetOscillators(true, Oscillator.OscillatorIndex.Primary);
            }
            else if (message == "osc1Wave:")
            {
                //TODO: get wavetype
                oscillator.SetWaveType(SignalGeneratorType.Sin, Oscillator.OscillatorIndex.Primary);

            }
            else if (message == "osc1Vol:")
            {
                //TODO: get volume
                oscillator.SetVolume(1.0f, Oscillator.OscillatorIndex.Primary);
            }
            else if (message == "osc1Attack:")
            {
                float attack = 0.01f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Attack, Oscillator.OscillatorIndex.Primary, attack);
            }
            else if (message == "osc1Decay:")
            {
                float decay = 0.5f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Decay, Oscillator.OscillatorIndex.Primary, decay);
            }
            else if (message == "osc1Sustain:")
            {
                float sustain = 0.01f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Sustain, Oscillator.OscillatorIndex.Primary, sustain);
            }
            else if (message == "osc1Release:")
            {
                float release = 0.3f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Release, Oscillator.OscillatorIndex.Primary, release);
            }
            //  ------------------------------------------------ end of osc 1 updates -----------------------------------------------------------------------------------------------------------


            //  ------------------------------------------------ osc 2 updates -----------------------------------------------------------------------------------------------------------
            else if (message == "osc2On:")
            {
                //TODO: get boolean
                oscillator.SetOscillators(true, Oscillator.OscillatorIndex.Secondary);
            }
            else if (message == "osc2Wave:")
            {
                //TODO: get wavetype
                oscillator.SetWaveType(SignalGeneratorType.Sin, Oscillator.OscillatorIndex.Secondary);
            }
            else if (message == "osc2Vol:")
            {
                //TODO: get volume
                oscillator.SetVolume(1.0f, Oscillator.OscillatorIndex.Secondary);
            }
            else if (message == "osc2Attack:")
            {
                float attack = 0.01f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Attack, Oscillator.OscillatorIndex.Secondary, attack);
            }
            else if (message == "osc2Decay:")
            {
                float decay = 0.5f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Decay, Oscillator.OscillatorIndex.Secondary, decay);
            }
            else if (message == "osc2Sustain:")
            {
                float sustain = 0.01f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Sustain, Oscillator.OscillatorIndex.Secondary, sustain);
            }
            else if (message == "osc2Release:")
            {
                float release = 0.3f;
                oscillator.ConfigureADSR(Oscillator.ADSR.Release, Oscillator.OscillatorIndex.Secondary, release);
            }
            //  ------------------------------------------------ end of osc 2 updates -----------------------------------------------------------------------------------------------------------

            else
            {
                Console.WriteLine("received undefined synth update message");
            }
        }

        /// <summary>
        /// Thread is launced off of this method.
        /// it sends the message back to the phone stating that the host is ready to receive play/control signals from the phone.
        /// </summary>
        /// <param name="ar"></param>
        public void receiveSignalCallback(IAsyncResult ar)
        {
            byte[] addressBytes = Encoding.ASCII.GetBytes(((MelodySlot)ar)._phoneAddress);
            IPAddress ipAddress = new IPAddress(addressBytes);
            IPEndPoint ep = new IPEndPoint(ipAddress, ((MelodySlot)ar)._playReceivingPort);
            byte[] receivedBytes = ((MelodySlot)ar)._receivingSocket.EndReceive(ar, ref ep);

            string receivedString = Encoding.ASCII.GetString(receivedBytes);

            Console.WriteLine("receiveSignalCallback received string: " + receivedString);

            //  TODO: receive, then parse the signal
            int frequency = 1500;
            oscillator.StopWave(); //stop any waves that were previously playing
            oscillator.StartWave(frequency);
        }
    }
}