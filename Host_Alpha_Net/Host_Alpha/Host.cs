using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Threading;
using System.IO;

namespace Host_Alpha 
{
    //  TODO: moving forward,
    //          for our intents and purposes,
    //              it probably won't be neccessary to set a limit on the number of phones which can connect to the host...
    //  TODO:   Once we get a concrete plan for a class hierarchy we need to make a new solution....

    public class Host
    {
        // TODO: we are going to need arrays of UdpClients for sending and responding to phones after they have connected to the host.
        //              all responders will listen on port _numberOfConnections + _startingPortNumber
        
        [STAThread]
        static void Main(string[] args)
        {
            Manager manager = new Manager();
            Mixer.Instance.GetInputs();
            ClearTransferData();
            manager.StartHost();
            
            Console.Read();
        }

        private static void ClearTransferData()
        {
            //establish a directory and path for this file
            string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string completePath = System.IO.Path.Combine(directory, "Audio Mobile");
            if(System.IO.Directory.Exists(completePath))
                clearFolder(completePath);
        }
        private static void clearFolder(string FolderName)
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


        /// <summary>
        /// Start the sound engine, currently an ASIO instance.
        /// </summary>
        //private void initSoundEngine()
        //{
        //    try{    _soundEngine = new AsioOut("ASIO4ALL v2");  }
        //    catch(Exception){   Console.WriteLine("ERROR: Failed to initialize sound engine."); }
        //}
    
}
