using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Host_Alpha
{
    public class SamplerSlot : Slot
    {
        private string _phoneAddress;
        private int _playReceivingPort;
        private int _syncReceivingPort;
        private int _sendingPort = 10000;
        private int _numberOfConnections;
        private string _songName;
        private UdpClient _receivingSocket = null;
        private TcpListener _syncServer = null;
        private bool stereo = true;
        private bool loop = false;
        private bool syncingSong;
        private SamplerModel samplerModel;
        public static ISampleProvider lastPlayed = null;

        public SamplerSlot(string _phoneType, string _phoneAddress, int playReceivingPort, int syncReceivingPort, int _numberOfConnections, string _songName)
        {
            // TODO: Complete member initialization
            this._type = _phoneType;
            this._phoneAddress = _phoneAddress;
            this._playReceivingPort = playReceivingPort;
            this._syncReceivingPort = syncReceivingPort;
            this._numberOfConnections = _numberOfConnections;
            this._songName = _songName;
            this.samplerModel = new SamplerModel(_songName, 3);

            _sendingSocket = new UdpClient(_phoneAddress, _sendingPort);
            _receivingSocket = new UdpClient(_playReceivingPort);
        }

        public void setUpConnection()
        {
            syncingSong = true; 

            string startPlayMessage = "hey, host is ready for play signals on port: " + _playReceivingPort.ToString() + ", and sync signals on port: " + _syncReceivingPort.ToString();
            byte[] startPlayMessageBytes = Encoding.ASCII.GetBytes(startPlayMessage);
            _sendingSocket.Connect(_phoneAddress, _sendingPort);
            _sendingSocket.Send(startPlayMessageBytes, startPlayMessageBytes.Length);

            _syncServer = new TcpListener(IPAddress.Any, _syncReceivingPort);
            _syncServer.Start();

            CreateSamplerDirectory();

           // listener.OnConnect(this);

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
                int bankIndex, padIndex;

                if (playMessageString.StartsWith("p"))
                {
                    string[] fileParams = playMessageString.Split(' ');
                    bankIndex = Int16.Parse(fileParams[1]);
                    padIndex = Int16.Parse(fileParams[2]);

                    Console.WriteLine("receivePlaySignal - play was called");

                    //play the file associated with the incoming filename
                    //dont decrement bank index
                    var input = RetrievePadAudio(bankIndex, padIndex);
                   // input.SkipOver = new TimeSpan((long)(samplerModel.song.banks[bankIndex].pads[padIndex].startPoint/0.0000001));
                    input.SkipOver = samplerModel.song.banks[bankIndex].pads[padIndex].startPoint ;
                    int duration 
                        = samplerModel.song.banks[bankIndex].pads[padIndex].endPoint.Seconds - samplerModel.song.banks[bankIndex].pads[padIndex].startPoint.Seconds;
                    
                    //input.SkipOver = new TimeSpan(samplerModel.song.banks[bankIndex].pads[padIndex].startPoint);
                    if (!ReferenceEquals(input, null))
                    {
                        //if this sample is already playing, stop it
                        Mixer.Instance.StopSample(input.Take(new TimeSpan(0,0,duration)));

                        //play sound and remember to reset audio file
                       samplerModel.song.banks[bankIndex].pads[padIndex].
                           audioFile[samplerModel.song.banks[bankIndex].pads[padIndex]._currentReaderIndex].Position = 0;
                       // samplerModel.song.banks[bankIndex].pads[padIndex].audioFile.Position = samplerModel.song.banks[bankIndex].pads[padIndex].startPoint;
                        Mixer.Instance.PlaySample(true, input.Take(new TimeSpan(0,0,duration)));
                      //  lastPlayed = input;        
                    }
                }
                else if (playMessageString.StartsWith("s"))
                {
                    string[] fileParams = playMessageString.Split(' ');
                    bankIndex = Int16.Parse(fileParams[1]);
                    padIndex = Int16.Parse(fileParams[2]);
                     
                    Console.WriteLine("receivePlaySignal - stop was called");

                    //stop the last played sample
                    if(!ReferenceEquals(null, lastPlayed))
                        Mixer.Instance.StopSample(lastPlayed);
                }
                
            }
        }

        /* Applies volume and pan parameters to outgoing samples*/
        private ISampleProvider ProcessInput(ISampleProvider input, int bankIndex, int padIndex)
        {
            VolumeSampleProvider VSP = new VolumeSampleProvider(input);
            VSP.Volume = samplerModel.song.banks[bankIndex].pads[padIndex].volume;
            PanningSampleProvider PSP = new PanningSampleProvider(VSP.ToWaveProvider16().ToSampleProvider());                                                  // <- this throws ArgumentException
            PSP.Pan = samplerModel.song.banks[bankIndex].pads[padIndex].pan;

            //ISampleProvider result = PSP.Skip(new TimeSpan(0, 0, (int) samplerModel.song.banks[bankIndex].pads[padIndex].startPoint));
            ISampleProvider result = PSP.Skip(samplerModel.song.banks[bankIndex].pads[padIndex].startPoint);
            //int duration = (int)(samplerModel.song.banks[bankIndex].pads[padIndex].endPoint - samplerModel.song.banks[bankIndex].pads[padIndex].startPoint);
            TimeSpan duration = (samplerModel.song.banks[bankIndex].pads[padIndex].endPoint - samplerModel.song.banks[bankIndex].pads[padIndex].startPoint);

            //result = result.Take(new TimeSpan(0,0,duration));
            result = result.Take(duration);

            return result.ToWaveProvider16().ToSampleProvider();
        }

        public void receiveSyncSignal()
        {
            //const string fileHeader = "file: ";
            const string fileNameHeader = "fileName: ";
            const string volumeHeader = "volume: ";
            const string panHeader = "pan: ";
            const string startingPointHeader = "startingPoint: ";
            const string endingPointHeader = "endPoint: ";
            const string triggerModeHeader = "triggerMode: ";
            const string pitchHeader = "pitch: ";

            TcpClient client = _syncServer.AcceptTcpClient();
            Console.WriteLine("Connection made with client");
            while (true)
            {
                //reset timeouts every iteration
                client.Client.ReceiveTimeout = 5000;
                client.Client.SendTimeout = 5000;
                client.ReceiveTimeout = 5000;
                client.SendTimeout = 5000;

                //---get the incoming data through a network stream---
                NetworkStream nwStream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                // This is being called when I am not actually sending anything,
                //      which is entirely possible,
                //          can you make it so it won't time out??
                int bytesRead;
                string dataRecieved = null;
                try
                {
                    //---read incoming stream---
                    bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
                    dataRecieved = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                  //  Console.WriteLine("Received : " + dataRecieved);
                    //if (dataRecieved.Length == 0 || ReferenceEquals(dataRecieved, null))
                    //{
                    //    nwStream.Flush();
                    //    continue;
                    //}

                    // look for file ending string
                    if (dataRecieved.Contains("DONESENDINGALLPADS!!!"))
                    {
                        syncingSong = false;
                    }

                    if(dataRecieved.Contains("SONGWILLDISCONNECT!!!"))
                    {
                        Console.WriteLine(dataRecieved);
                        break;
                    }

                }
                catch (System.IO.IOException e) // time out
                {
                   // Console.WriteLine(e.Message);

                    if (syncingSong == true)
                    {
                        //throw e;
                        continue;
                    }
                    else if (syncingSong == false)
                    {
                        continue;
                    } 
                }
                
                /*
                 * Transaction is as follows:
                 * > file: <bank> <pad> <fileName>
                 * < ACK
                 * > NSData object
                 * < ACK
                 */
                if (dataRecieved != null)
                {
                    if (dataRecieved.Contains(fileNameHeader))
                    {
                        string fileName;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        fileName = fileParams[3];
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("filename retrieved: " + fileName);

                        //---write back the text to the client---
                        //"okay now send file: <filename> <bank number> <pad number>"
                        string response = "okay now send file: " + fileName + " " + bankIndex + " " + padIndex;
                        nwStream.Write(Encoding.UTF8.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                        nwStream.ReadTimeout = 15000;

                        string filePath = GetFilePath(_songName, bankIndex, padIndex);

                        

                        try
                        {
                            //--wait for incoming file---
                            // using (var stream = client.GetStream())
                            using (var output = File.Create(filePath+"/"+ fileName))
                            {
                                Console.WriteLine("Starting to receive the file from client");

                                // read the file in chunks of 1KB
                                buffer = new byte[1024];
                                while ((bytesRead = nwStream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    // convert every KB to see if it contains the done-sending-file message
                                    string receivedKBString = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                                    // look for file ending string
                                    if (receivedKBString.Contains("ALLDONESENDINGFILE!!!"))
                                    {
                                        output.Write(buffer, 0, bytesRead /* - 21 */); // Don't want the ending string in the file, ????
                                        output.Close();
                                        break;
                                    }
                                    output.Write(buffer, 0, bytesRead);
                                }

                                Console.WriteLine("Bytes Read From Client");
                                // output.Close();
                            }
                        }
                        catch (System.IO.IOException e) // time out
                        {
                            Console.WriteLine(e.Message);
                            continue;
                        }

                        //---write back to the client---
                        Console.WriteLine("File retrieved [" + bankIndex + "][" + padIndex + "]");
                        response = "got file";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);

                        Console.WriteLine("Loading File Into Sample Model");

                        // init bank in question if null
                        if (samplerModel.song.banks[bankIndex - 1] == null)
                        {
                            samplerModel.song.banks[bankIndex - 1] = new Bank();
                        }
                        //use samplermodel isntead of sampler manager
                        //remember that bankindex is 1 indexed
                        samplerModel.song.banks[bankIndex - 1].pads[padIndex] = new Pad(filePath + "/" + fileName);
                    }
                    else if (dataRecieved.Length >= volumeHeader.Length && dataRecieved.Substring(0, volumeHeader.Length).Equals(volumeHeader))
                    {
                        float volume;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        volume = float.Parse(fileParams[3]);
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("volume retrieved: " + volume);

                        if (!ReferenceEquals(samplerModel.song.banks[bankIndex - 1].pads[padIndex], null))
                            samplerModel.song.banks[bankIndex - 1].pads[padIndex].volume = volume;

                        //---write back the text to the client---
                        Console.WriteLine("volume set [" + bankIndex + "][" + padIndex + "]");
                        string response = "volume set";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                    }
                    else if (dataRecieved.Length >= panHeader.Length && dataRecieved.Substring(0, panHeader.Length).Equals(panHeader))
                    {
                        float pan;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        pan = float.Parse(fileParams[3]);
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("pan retrieved: " + pan);

                        if (!ReferenceEquals(samplerModel.song.banks[bankIndex - 1].pads[padIndex], null))
                            samplerModel.song.banks[bankIndex - 1].pads[padIndex].pan = pan;

                        //---write back the text to the client---
                        Console.WriteLine("pan set [" + bankIndex + "][" + padIndex + "]");
                        string response = "pan set";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                        //continue;
                    }
                    else if (dataRecieved.Length >= startingPointHeader.Length && dataRecieved.Substring(0, startingPointHeader.Length).Equals(startingPointHeader))
                    {
                        float startingPoint;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        startingPoint = float.Parse(fileParams[3]);
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("starting point retrieved: " + startingPoint);

                        if (!ReferenceEquals(samplerModel.song.banks[bankIndex - 1].pads[padIndex], null))
                           // samplerModel.song.banks[bankIndex - 1].pads[padIndex].startPoint = startingPoint;
                            samplerModel.song.banks[bankIndex - 1].pads[padIndex].startPoint = new TimeSpan((long)(startingPoint/0.0000001));

                        //---write back the text to the client---
                        Console.WriteLine("starting point set [" + bankIndex + "][" + padIndex + "]");
                        string response = "starting point set";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                    }
                    else if (dataRecieved.Length >= endingPointHeader.Length && dataRecieved.Substring(0, endingPointHeader.Length).Equals(endingPointHeader))
                    {
                        float endingPoint;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        endingPoint = float.Parse(fileParams[3]);
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("ending point retrieved: " + endingPoint);

                        if (!ReferenceEquals(samplerModel.song.banks[bankIndex - 1].pads[padIndex], null))
                           // samplerModel.song.banks[bankIndex - 1].pads[padIndex].endPoint = endingPoint;
                            samplerModel.song.banks[bankIndex - 1].pads[padIndex].endPoint = new TimeSpan((long)(endingPoint/0.0000001));

                        //---write back the text to the client---
                        Console.WriteLine("ending point set [" + bankIndex + "][" + padIndex + "]");
                        string response = "ending point set";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                    }
                    else if (dataRecieved.Length >= triggerModeHeader.Length && dataRecieved.Substring(0, triggerModeHeader.Length).Equals(triggerModeHeader))
                    {
                        bool triggerMode;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        triggerMode = bool.Parse(fileParams[3]);
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("trigger mode retrieved: " + triggerMode.ToString());

                        if (!ReferenceEquals(samplerModel.song.banks[bankIndex - 1].pads[padIndex], null))
                            samplerModel.song.banks[bankIndex - 1].pads[padIndex].triggerMode = triggerMode;

                        //---write back the text to the client---
                        Console.WriteLine("Trigger mode set [" + bankIndex + "][" + padIndex + "]");
                        string response = "trigger mode set";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                    }
                    else if (dataRecieved.Length >= pitchHeader.Length && dataRecieved.Substring(0, pitchHeader.Length).Equals(pitchHeader))
                    {
                        float pitch;
                        int bankIndex, padIndex;
                        string[] fileParams = dataRecieved.Split(' ');

                        //set filename
                        bankIndex = Int16.Parse(fileParams[1]);
                        padIndex = Int16.Parse(fileParams[2]);
                        pitch = float.Parse(fileParams[3]);
                        Console.WriteLine("bank # retrieved: " + bankIndex);
                        Console.WriteLine("pad # retrieved: " + padIndex);
                        Console.WriteLine("pitch retrieved: " + pitch);

                        if (!ReferenceEquals(samplerModel.song.banks[bankIndex - 1].pads[padIndex], null))
                            samplerModel.song.banks[bankIndex - 1].pads[padIndex].pitch = pitch;

                        //---write back the text to the client---
                        Console.WriteLine("pitch set [" + bankIndex + "][" + padIndex + "]");
                        string response = "pitch set";
                        nwStream.Write(Encoding.ASCII.GetBytes(response), 0, Encoding.ASCII.GetBytes(response).Length);
                    }

                    client.GetStream().Flush();
                }
            }

            client.Close();
        }

        private void CreateSamplerDirectory()
        {
            //establish a directory and path for this file
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string completePath = System.IO.Path.Combine(directory, "Audio Mobile", _songName);

            //first create folder for song
            if (System.IO.Directory.Exists(completePath)) 
            {
                int count = 1;
                while (System.IO.Directory.Exists(completePath))
                {
                    //  clearFolder(completePath);
                    //instead of clearing the folder, make a new one
                    _songName = _songName + count.ToString();
                    completePath = System.IO.Path.Combine(directory, "Audio Mobile", _songName);
                    count++;
                }
            }

            try
            {
                System.IO.Directory.CreateDirectory(completePath);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.Error.WriteLine(e.Message);
            }

            //then a folder for each bank
            for (int i = 0; i < 3; i++)
            {
                string bankPath = System.IO.Path.Combine(completePath, i.ToString());
                if (System.IO.Directory.Exists(bankPath)) { clearFolder(bankPath); }
                else
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(bankPath);
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                }

                for (int j = 0; j < 8; j++)
                {
                    string padPath = System.IO.Path.Combine(bankPath, j.ToString());
                    if (System.IO.Directory.Exists(padPath)) { clearFolder(padPath); }
                    else
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(padPath);
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            Console.Error.WriteLine(e.Message);
                        }
                    }

                }


            }
        }

        private string GetFilePath(string _songName, int bankIndex, int padIndex)
        {

            //establish a directory and path for this file
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string songFolder = _songName;
            string completePath = System.IO.Path.Combine(directory, "Audio Mobile", songFolder, (bankIndex - 1).ToString(), padIndex.ToString());

            return completePath;
        }

        //public ISampleProvider RetrievePadAudio(int bank, int pad)
            public OffsetSampleProvider RetrievePadAudio(int bank, int pad)
        {
            if (samplerModel.song.banks[bank].pads[pad].
                audioFile[samplerModel.song.banks[bank].pads[pad]._currentReaderIndex] == null)
                return null;
            else
            {
                //need to add volume/processing for audio file
                //return samplerModel.song.banks[bank].pads[pad].audioFile.ToSampleProvider() ;
                //return (OffsetSampleProvider)samplerModel.song.banks[bank].pads[pad].audioFile.ToSampleProvider();

                var result = new OffsetSampleProvider(samplerModel.song.banks[bank].pads[pad].
                    audioFile[samplerModel.song.banks[bank].pads[pad]._currentReaderIndex].ToSampleProvider());
                // increment  pad reader index
                samplerModel.song.banks[bank].pads[pad]._currentReaderIndex = 
                    samplerModel.song.banks[bank].pads[pad]._currentReaderIndex ==
                    (samplerModel.song.banks[bank].pads[pad]._numberOfReaders - 1) ? 0 : samplerModel.song.banks[bank].pads[pad]._currentReaderIndex + 1;
                return result;
            }

            
        }

        public ISampleProvider AttemptSupplySample(bool stereo, bool loop, string fileName)
        {
            //first check if this file has been added yet
            if (SamplerManager.Instance.ContainsFile(fileName))
            {
                ISampleProvider file = SamplerManager.Instance.GetFile(fileName);
                if (!ReferenceEquals(null, file))
                {
                    return SamplerManager.Instance.SupplySample(stereo, loop, file);
                }
                else return null;
            }
            else
            {
                Console.WriteLine("File " + fileName + " not found.");
                return null;
            }
        }

        private void clearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                fi.Delete();
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                clearFolder(di.FullName);
                di.Delete();
            }
        }
    }
}
