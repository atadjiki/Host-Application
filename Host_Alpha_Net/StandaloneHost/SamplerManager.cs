using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneHost
{
    //1: toggle back and forth between playing a stereo wav file in either stereo or mono.
    //   sometimes making a stereo file mono makes it easier to pan.
    //2: for each pad in a sample bank I want 3 filter parameters. Frequency, Resonance, 
    //   and to toggle between low pass and high pass filtering.
    //3: for each pad It would be nice if we could toggle loop mode, said looping will not be 
    //   tied to any sort of sequencer. I'm saying we could drop in some drony shit and just have it loop.
    //4: is there a way to play sounds backwards without rendering a new file?
    public class SamplerManager
    {
        private static volatile SamplerManager instance;
        private static object syncRoot = new Object();
        private Dictionary<string, WaveFileReader> receivedFiles = new Dictionary<string, WaveFileReader>();
        
        private SamplerManager() { }
        public static SamplerManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new SamplerManager();
                    }
                }

                return instance;
            }
        }


        public ISampleProvider SupplySample(bool stereo, bool loop, ISampleProvider input)
        {
            if (stereo) input.ToStereo();
            else input.ToMono();
            return input;
        }

        public void AddFile(string fileName, WaveFileReader file)
        {
            if (receivedFiles.Keys.Contains(fileName))
                return;
            receivedFiles.Add(fileName, file);

        }

        public ISampleProvider GetFile(string fileName)
        {
            if(ContainsFile(fileName))
            {
                return receivedFiles[fileName].ToSampleProvider();
            }
            else
            {
                return null;
            }
        }

        public bool ContainsFile(string fileName)
        {
            return receivedFiles.Keys.Contains(fileName);
        }

    }
}
