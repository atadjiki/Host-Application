using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Host_Alpha
{
    /*
     * This class essentially acts as the main server of the application.
     * Devices can connect to the Manager by initiating a handshake handled over UDP
     * Once connected, that device is given a slot based on what type of app it is. 
     * From there, that device established two new connections, one for syncing and one for playing. 
     */
    public class Manager : AudioListener
    {
        //private AsioOut _soundEngine = null;

        //Udp sockets to handle the broadcast
        private static UdpClient _handShakeListener = null;
       // private static UdpClient _handShakeSender = null;

        // port number reserved for connection requests by phones
        private const int _handshakeReceivePort = 9998;
        private  int _handshakeSendPort = 9999;
        private const int _playSendPort = 10000;

        // number of phones currently connected to the host.
        private static int _numberOfConnections = 0;

        // the port number assigned to the first connected phone,
        //      will be incremented as more connections occur.
        private static int _startingPortNumber = 10001;

        private MelodySlot[] _MelodySlotArray = null;
        private PercussionSlot[] _PercussionSlotArray = null;
        private SamplerSlot[] _SamplerSlotArray = null;

        private static string _phoneAddress = "";
        private static string _phoneType = "";
        private static string _songName = "";

        // TODO: we are going to need arrays of UdpClients for sending and responding to phones after they have connected to the host.
        //              all responders will listen on port _numberOfConnections + _startingPortNumber
        public void BeginListen()
        {
            string received_data;
            byte[] receive_byte_array;
            bool done = false; 
            IPEndPoint phoneEP = new IPEndPoint(IPAddress.Any, _handshakeReceivePort);
            _handShakeListener = new UdpClient(_handshakeReceivePort);
            
            

            try
            {
                //  TODO: this while loop is strictly in place to receive connection requests from phones.
                while (!done)
                {
                    
                    
                //     _handShakeSender = new UdpClient(_handshakeSendPort);
                    Console.WriteLine("[Manager] Waiting For Client to Initiate");
                    receive_byte_array = _handShakeListener.Receive(ref phoneEP);
                   
                    //receive_byte_array = _handShakeSender.Receive(ref phoneEP);
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    Console.WriteLine("{0}", received_data);

                    // if we received the proper connection request message from a phone...
                    if (received_data.StartsWith("Hello Audio-Mobile Address:"))
                    {
                        // parse message to array
                        string[] messageArray = received_data.Split(' ');

                        _phoneAddress = messageArray[3];
                        _phoneType = messageArray[5];
                        if (_phoneType != "Percussion")
                        {
                            _songName = messageArray[7];
                            if (messageArray.Length >= 9)
                            {
                                for (int i = 8; i < messageArray.Length; i++)
                                {
                                    _songName = _songName + "_" + messageArray[i];
                                }
                            }
                        }
                           

                        int playAssignedPort = _startingPortNumber + (_numberOfConnections * 2); //<- each phone will have a port for UDP and a port for TCP
                        int syncAssignedPort = playAssignedPort + 1;

                        string hostName = Dns.GetHostName();
                        IPAddress[] ipAddresses = Dns.GetHostAddresses(hostName);
                        string hostIpAddress = "";

                        // TODO: out of the 7 ipaddresses for my thinkpad on the home network,
                        //                  the only one that was not ipv6 had an adress family of InterNetwork.
                        //                      we need to double check and make sure that this kind of discrimination will work on other machines,
                        //                          and on other networks....
                        foreach (IPAddress address in ipAddresses)
                        {
                            if (address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                hostIpAddress = address.ToString();
                            }
                        }

                        // assemble connection response
                        string response = "Hello there, use port: " + playAssignedPort.ToString() + " for play signals, and port: " + syncAssignedPort.ToString() + " for sync signals; at address: " + hostIpAddress;
                        byte[] responseArray = Encoding.ASCII.GetBytes(response);
                        int g = responseArray.Length;

                        //try { _handShakeSender.Connect(_phoneAddress, _handshakeSendPort); }
                        //catch (Exception) { Console.WriteLine("_handShakeSender threw exception when connecting to phone"); }

                        try
                        {
                            //  TODO: it appears that the phone is not receiving this message, unless it is the first song
                          //  _handShakeSender.Send(responseArray, responseArray.Length, _phoneAddress, _handshakeSendPort);
                            _handShakeListener.Send(responseArray, responseArray.Length, _phoneAddress, _handshakeSendPort);
                            Console.WriteLine("[Manager] Sending response to client");
                        }
                        catch (Exception) { Console.WriteLine("_handShakeSender threw exception when sending handshake response message"); }

                        receive_byte_array = new Byte[0];
                        received_data = "";

                        try
                        {
                            //TODO: no data is being received here
                            // get ready to receive port assignment ack
                            _handShakeListener.Client.ReceiveTimeout = 5000;
                            Console.WriteLine("[Manager] Waiting For Client To Reply");
                           receive_byte_array = _handShakeListener.Receive(ref phoneEP);
                           
                           
                           // receive_byte_array = _handShakeSender.Receive(ref phoneEP);
                        }
                        catch (Exception) { Console.WriteLine("_handShakeListener threw exception when receiving port assignment ack message from phone"); }

                        _handShakeListener.Client.ReceiveTimeout = 0;
                        received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                        Console.WriteLine("{0}", received_data);

                        if (received_data.StartsWith("Okay song: ") || received_data.StartsWith("Okay perc phone:"))
                        {
                            //  TODO: at some point we need to assert that the port sent back from the phone matches the port we assigned to it.

                            // add a new slot to the slot array
                            if (_phoneType == "Melody")
                            {
                                _MelodySlotArray[_numberOfConnections] = new MelodySlot(_phoneType, _phoneAddress, playAssignedPort, syncAssignedPort, _numberOfConnections, _songName);
                                _MelodySlotArray[_numberOfConnections].SetAudioListener(this);
                                // TODO: we may want to keep a member thread array,
                                //          it might be easier to keep track of whether threads are alive or not....
                                // launch new thread based upon new Slot
                                // Thread thread = new Thread(new ThreadStart(_slotArray[_numberOfConnections].setUpPlayConnection));
                                //thread.Start();
                                // this method will lauch two threads,
                                //  one to handle play signals,
                                //      and one to handle sync signals
                                _MelodySlotArray[_numberOfConnections].setUpConnection();

                                // increment number of connections member
                                _numberOfConnections++;
                            }
                            else if (_phoneType == "Percussion")
                            {
                                _PercussionSlotArray[_numberOfConnections] = new PercussionSlot(_phoneType, _phoneAddress, playAssignedPort, syncAssignedPort, _numberOfConnections);
                                _PercussionSlotArray[_numberOfConnections].SetAudioListener(this);
                                _PercussionSlotArray[_numberOfConnections].setUpConnection();
                                _numberOfConnections++;
                            }
                            else if (_phoneType == "Sampler")
                            {
                                _SamplerSlotArray[_numberOfConnections] = new SamplerSlot(_phoneType, _phoneAddress, playAssignedPort, syncAssignedPort, _numberOfConnections, _songName);
                                _SamplerSlotArray[_numberOfConnections].SetAudioListener(this);
                                _SamplerSlotArray[_numberOfConnections].setUpConnection();
                                _numberOfConnections++;
                            }
                        }
                    }

                    
                    Console.WriteLine("[Manager] Transaction Complete\n");
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
            _handShakeListener.Close();
            //       _handShakeSender.Close();
        }
        public void StartHost()
        {

            _MelodySlotArray = new MelodySlot[12];
            _PercussionSlotArray = new PercussionSlot[12];
            _SamplerSlotArray = new SamplerSlot[12];

            Thread thread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                BeginListen();
            });


            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

        }

        public void OnPlayWave(NAudio.Wave.SampleProviders.MixingSampleProvider input, Slot slot)
        {
            Mixer.Instance.PlayWave(input, slot);
        }

        public void OnPlayWave(MixingSampleProvider input)
        {
            Mixer.Instance.PlayWave(input);
        }

        public void OnStopWave(NAudio.Wave.SampleProviders.MixingSampleProvider input, Slot slot)
        {
            Mixer.Instance.StopWave(input, slot);
        }

        public void OnPlaySample(PercussionManager.ShakerType type, Slot slot)
        {
            Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyShaker(type), slot);
        }
        public void OnPlaySample(PercussionManager.ShakerType type)
        {
            Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyShaker(type));
           
        }
        public void OnPlaySample(WaveFileReader file)
        {
            Mixer.Instance.PlaySample(file.ToSampleProvider());
        }

        public void OnStopSample(WaveFileReader file)
        {
            Mixer.Instance.StopSample(file.ToSampleProvider());
        }

        public void OnPlaySample(PercussionManager.DoublePadType type, PercussionManager.DoublePadVelocityType velocity, Slot slot)
        {
            Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyDoublePad(type, velocity), slot);
        }

        public void OnPlaySample(PercussionManager.DoublePadType type, PercussionManager.DoublePadVelocityType velocity)
        {
            Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyDoublePad(type, velocity));
        }

        public void OnPlaySample(PercussionManager.DoublePadVelocityType velocity, WaveFileReader file)
        {
            Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyDoublePad(velocity, file));
        }

        public void OnPlaySample(PercussionManager.HiHatType type, WaveFileReader file)
        {
            Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyHiHat(type, file));
        }

        public void OnConnect(Slot slot)
        {
            Mixer.Instance.AddSlot(slot);
        }

        public void OnDisconnect(Slot slot)
        {
            Mixer.Instance.RemoveSlot(slot);
        }


        public void OnStopWave(MixingSampleProvider input)
        {
            Mixer.Instance.StopWave(input);
        }


      
    }


    /// <summary>
    /// Start the sound engine, currently an ASIO instance.
    /// </summary>
    //private void initSoundEngine()
    //{
    //    try{    _soundEngine = new AsioOut("ASIO4ALL v2");  }
    //    catch(Exception){   Console.WriteLine("ERROR: Failed to initialize sound engine."); }
    //}
}
