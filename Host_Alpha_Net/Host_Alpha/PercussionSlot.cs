using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace Host_Alpha
{
    class PercussionSlot : Slot
    {
        private int _numberOfConnections;
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

        /// <summary> used to receive real-time play signals from the phone </summary>
        private UdpClient _receivingSocket = null;

        /// <summary>used to receive non real-time signlas from the phone concerning the state of the phone's sound engine</summary>
        private TcpListener _syncServer = null;

        //  TODO: this may not be neccessary....
        // connection number
        private int _id = -1;

        private bool stereo = true;

        // TODO: stop condition is unimplemented.
        private bool stop = false;

        private PercussionModel _percussionModel;

        public PercussionSlot(string _phoneType, string _phoneAddress, int playReceivingPort, int syncReceivingPort, int _numberOfConnections)
        {
            // TODO: Complete member initialization
            this._type = _phoneType;
            this._phoneAddress = _phoneAddress;
            this._playReceivingPort = playReceivingPort;
            this._syncReceivingPort = syncReceivingPort;
            this._numberOfConnections = _numberOfConnections;

            _sendingSocket = new UdpClient(_phoneAddress, _sendingPort);
            _receivingSocket = new UdpClient(_playReceivingPort);
        }

        public void setUpConnection()
        {
            string startPlayMessage = "hey, host is ready for play signals on port: " + _playReceivingPort.ToString() + ", and sync signals on port: " + _syncReceivingPort.ToString();
            byte[] startPlayMessageBytes = Encoding.ASCII.GetBytes(startPlayMessage);
            _sendingSocket.Connect(_phoneAddress, _sendingPort);
            _sendingSocket.Send(startPlayMessageBytes, startPlayMessageBytes.Length);

            listener.OnConnect(this);
            _percussionModel = new PercussionModel();

            _syncServer = new TcpListener(IPAddress.Any, _syncReceivingPort);
            _syncServer.Start();
           
            Thread playThread = new Thread(new ThreadStart(receivePlaySignal));
            Thread syncThread = new Thread(new ThreadStart(receiveSyncSignal));
            playThread.Start();
            syncThread.Start();
        }

        /// <summary>
        /// method to receive play/control signals from the phone
        /// </summary>
        public void receivePlaySignal()
        {
 
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            while (true)
            {
                Byte[] playMessageBytes = _receivingSocket.Receive(ref ep);
                String playMessageString = Encoding.ASCII.GetString(playMessageBytes);

                Console.WriteLine("receivePlaySignal was called");

                // parse the incoming message
                
                if(playMessageString.StartsWith("s")){  receiveShakerPlaySignal(playMessageString);     }
                else if (playMessageString.StartsWith("d")) {   receiveDoublePadPlaySignal(playMessageString);     }
                else if (playMessageString.StartsWith("h")) 
                {
                    receiveHiHatPlaySignal(playMessageString);     
                }

                //correct these messages later

                else if (playMessageString.StartsWith("rc")) //rotator clockwise
                {}
                else if (playMessageString.StartsWith("rcc")) //rotator counter clockwise
                {}
                else if (playMessageString.StartsWith("rp")) //rotator push
                {}
            }
        }

        private void receiveShakerPlaySignal(string signal)
        {
            /*
                 * Shaker Commands
                 */
            // forward signal
            if (signal.StartsWith("sf"))
            {
                _percussionModel.hi_shk1.Position = 0;
                listener.OnPlaySample(_percussionModel.hi_shk1);

            }
            // back signal
            else if (signal.StartsWith("sb"))
            {
                _percussionModel.hi_shk2.Position = 0;
                listener.OnPlaySample(_percussionModel.hi_shk2);
            }
        }

        private void receiveDoublePadPlaySignal(string signal)
        {
            //listener.OnStopSample(_percussionModel.dp_1);
            //listener.OnStopSample(_percussionModel.dp_2);

            if (signal.StartsWith("dp1"))
            {
                /*
                     * Double Pad Commands
                     */
                // Double Pad 1
                if (signal.StartsWith("dp11"))
                {
                    //_percussionModel.dp_1.Position = 0;
                    _percussionModel.dp_1[_percussionModel._currentDP1ReaderIndex].Position = 0;
                    listener.OnPlaySample(PercussionManager.DoublePadVelocityType.Low, _percussionModel.dp_1[_percussionModel._currentDP1ReaderIndex]);
                }
                else if (signal.StartsWith("dp12"))
                {
                    //_percussionModel.dp_1.Position = 0;
                    _percussionModel.dp_1[_percussionModel._currentDP1ReaderIndex].Position = 0;
                    listener.OnPlaySample(PercussionManager.DoublePadVelocityType.Middle, _percussionModel.dp_1[_percussionModel._currentDP1ReaderIndex]);
                }
                else if (signal.StartsWith("dp13"))
                {
                    //_percussionModel.dp_1.Position = 0;
                    _percussionModel.dp_1[_percussionModel._currentDP1ReaderIndex].Position = 0;
                    listener.OnPlaySample(PercussionManager.DoublePadVelocityType.High, _percussionModel.dp_1[_percussionModel._currentDP1ReaderIndex]);
                    // listener.OnPlaySample( _percussionModel.dp_1);
                }

                // increment  dp1's reader index
                _percussionModel._currentDP1ReaderIndex = _percussionModel._currentDP1ReaderIndex == (_percussionModel._numberOfReaders - 1) ? 0 : _percussionModel._currentDP1ReaderIndex + 1;
            }
            else
            {
                // Double Pad 2
                if (signal.StartsWith("dp21"))
                {
                    // _percussionModel.dp_2.Position = 0;
                    _percussionModel.dp_2[_percussionModel._currentDP2ReaderIndex].Position = 0;
                    listener.OnPlaySample(PercussionManager.DoublePadVelocityType.Low, _percussionModel.dp_2[_percussionModel._currentDP2ReaderIndex]);
                }
                else if (signal.StartsWith("dp22"))
                {
                    //_percussionModel.dp_2.Position = 0;
                    _percussionModel.dp_2[_percussionModel._currentDP2ReaderIndex].Position = 0;
                    listener.OnPlaySample(PercussionManager.DoublePadVelocityType.Middle, _percussionModel.dp_2[_percussionModel._currentDP2ReaderIndex]);
                }
                else if (signal.StartsWith("dp23"))
                {
                    //_percussionModel.dp_2.Position = 0;
                    _percussionModel.dp_2[_percussionModel._currentDP2ReaderIndex].Position = 0;
                    listener.OnPlaySample(PercussionManager.DoublePadVelocityType.High, _percussionModel.dp_2[_percussionModel._currentDP2ReaderIndex]);
                }

                // increment dp2's reader index
                _percussionModel._currentDP2ReaderIndex = _percussionModel._currentDP2ReaderIndex == (_percussionModel._numberOfReaders - 1) ? 0 : _percussionModel._currentDP2ReaderIndex + 1;
            }
        }

        private void receiveHiHatPlaySignal(string signal)
        {
            if (signal.StartsWith("hho"))
                {
                    listener.OnStopSample(_percussionModel.hhc);

                    _percussionModel.hho.Position = 0;
                    listener.OnPlaySample(_percussionModel.hho);
                    // listener.OnPlaySample(PercussionManager.HiHatType.Open, percussionModel.hho);
                }
            else if (signal.StartsWith("hhc"))
                {
                    //stop opposite sample if it is in the mixer
                    listener.OnStopSample(_percussionModel.hho);

                    _percussionModel.hhc.Position = 0;
                    listener.OnPlaySample(_percussionModel.hhc);
                    // listener.OnPlaySample(PercussionManager.HiHatType.Closed, percussionModel.hhc);
                }
        }

        
        public void receiveSyncSignal()
        {

            /*file header format
             * file: <fileSlot> <fileName>
             * ex: file: <dp1> <snare.wav>
             */
            const string path = "./";
            const string fileHeader = "file: ";

            while (true)
            {
                TcpClient client = _syncServer.AcceptTcpClient();
                Console.WriteLine("Connection made with client");

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
                    if (dataReceived.Contains(fileHeader))
                    {
                        string fileName;
                        string fileSlot;
                        string[] fileParams = dataReceived.Split(' ');

                        //set filename
                        fileSlot = fileParams[1];
                        fileName = fileParams[2];
                        Console.WriteLine("file slot retrieved: " + fileSlot);
                        Console.WriteLine("filename retrieved: " + fileName);

                        if (ValidateFileSlot(fileSlot))
                        {
                            //---write back the text to the client---
                            string response = "okay now send file";
                            nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);

                            //--wait for incoming file---
                            using (var stream = client.GetStream())
                            using (var output = File.Create(path))
                            {
                                Console.WriteLine("Starting to receive the file from client");

                                // read the file in chunks of 1KB
                                buffer = new byte[1024];
                                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    output.Write(buffer, 0, bytesRead);
                                }

                                WaveFileReader reader = new WaveFileReader(path);
                                _percussionModel.SetFile(fileSlot, reader);

                                //---write back to the client---
                                Console.WriteLine("File retrieved [" + fileSlot + "][" + fileName + "]");
                                response = "got file";
                                nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                            }
                        }
                        else
                        {
                            string response = "invalid file slot";
                            nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                        }
                    }
                }

                client.Close();
            }
        }
        public bool ValidateFileSlot(string fileSlot)
        {
            if (_percussionModel.ContainsFileSlot(fileSlot))
            {
                return true;
            }
            else
            {
                return false;    
            }
        }
    }
}
