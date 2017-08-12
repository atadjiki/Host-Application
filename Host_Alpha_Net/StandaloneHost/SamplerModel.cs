using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneHost
{
    /*
     * An object that represents an instance of a Sampler song on the Sampler app.
     * Each Sampler song contains an array of banks, which in turn contain pads. 
     * Each pad contains a struct of settings as well as a corresponding audio file
     */
    public class SamplerModel
    {
        public Song song;
        public string songName; 
        public int numberOfBanks;
        public int _numberOfReaders = 5;

        public SamplerModel(string _songName, int _numberOfBanks)
        {
            song = new Song();
            songName = _songName;
            numberOfBanks = _numberOfBanks;
        }
    }

    public class Song
    {
        public Bank[] banks = new Bank[3];
    }

    public class Bank
    {
        public Pad[] pads = new Pad[8];
    }

    public class Pad
    {
        public string fileName;
        public WaveFileReader[] audioFile;
        public int _numberOfReaders = 5;
        public int _currentReaderIndex = 0;
        //public NAudio.Wave.SampleProviders.OffsetSampleProvider

        public TimeSpan startPoint;
        
        //public float startPoint = 0;
        public TimeSpan endPoint;
        //public float endPoint = -1;

        public float volume = 1.0f;
        /*
         * trigger mode = true { play through mode}
         * false { stop and play mode }
         */
        public bool triggerMode = false;
        public float playRate;
        public float pan = 0.5f;
        public float pitch = 0.0f;
        public bool stereo = true;

        public Pad() { }
        public Pad(string _fileName)
        {
            fileName = _fileName;
            audioFile = new WaveFileReader[_numberOfReaders];
            for (int i = 0; i < _numberOfReaders; i++)
            {
                audioFile[i] = new WaveFileReader(_fileName);
                endPoint = new TimeSpan((long)(audioFile[i].TotalTime.TotalSeconds / 0.0000001));
            }
                
        }
        //filter
        //looping
        //bool loopmode;
    }
}
