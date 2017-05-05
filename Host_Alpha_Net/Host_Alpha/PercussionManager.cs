using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Host_Alpha
{
    public class PercussionManager
    {
        private static volatile PercussionManager instance;
        private static object syncRoot = new Object();
        private WaveFileReader hi_Shk1;
        private WaveFileReader hi_Shk2;
        private WaveFileReader dp_1;
        
        
        public enum ShakerType { Forward, Back };
        public enum DoublePadType { PadOne, PadTwo };
        public enum DoublePadVelocityType { High, Middle, Low };
        public enum HiHatType { Open, Closed };

        private PercussionManager() 
        {
            //hi_Shk1 = new WaveFileReader(Host_Alpha.Properties.Resources.Hi_Shk1);
            //hi_Shk2 = new WaveFileReader(Host_Alpha.Properties.Resources.Hi_Shk2);
        }
        public static PercussionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PercussionManager();
                    }
                }

                return instance;
            }
        }

        public ISampleProvider SupplyShaker(ShakerType type)
        {

            if (type.Equals(ShakerType.Forward))
                return new WaveFileReader(Host_Alpha.Properties.Resources.Hi_Shk1).ToSampleProvider();
            else if (type.Equals(ShakerType.Back))
                return new WaveFileReader(Host_Alpha.Properties.Resources.Hi_Shk2).ToSampleProvider();
            else return null;
        }

        public ISampleProvider SupplyDoublePad(DoublePadType type, DoublePadVelocityType velocity)
        {
            WaveFileReader waveFileReader;
            TimeSpan totalTimeSpan;

            if (type.Equals(DoublePadType.PadOne)) //retrieve corresponding audio file 
                waveFileReader = new WaveFileReader(Host_Alpha.Properties.Resources.DP_1);
            else if (type.Equals(DoublePadType.PadTwo))
                waveFileReader = new WaveFileReader(Host_Alpha.Properties.Resources.DP_2);
            else return null;
           // waveFileReader = new WaveFileReader(Host_Alpha.Properties.Resources.test);

            

            totalTimeSpan = waveFileReader.TotalTime; //get length of audio file

            //adjust starting point of file accordingly 
            if (velocity.Equals(DoublePadVelocityType.High))
            {
                return waveFileReader.ToSampleProvider();
                
            }
            else if (velocity.Equals(DoublePadVelocityType.Middle))
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, (int)(totalTimeSpan.TotalSeconds * 0.70));
                return waveFileReader.ToSampleProvider().Skip(timeSpan);
            }
            else if (velocity.Equals(DoublePadVelocityType.Low))
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, (int)(totalTimeSpan.TotalSeconds * 0.40));
                return waveFileReader.ToSampleProvider().Skip(timeSpan);
            }
            else return null;

        }

        internal ISampleProvider SupplyDoublePad(DoublePadVelocityType velocity, WaveFileReader file)
        {
            TimeSpan totalTimeSpan;

            totalTimeSpan = file.TotalTime; //get length of audio file

            //adjust starting point of file accordingly 
            if (velocity.Equals(DoublePadVelocityType.High))
            {
                return file.ToSampleProvider();

            }
            else if (velocity.Equals(DoublePadVelocityType.Middle))
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, (int)(totalTimeSpan.TotalSeconds * 0.70));
                return file.ToSampleProvider().Skip(timeSpan);
            }
            else if (velocity.Equals(DoublePadVelocityType.Low))
            {
                TimeSpan timeSpan = new TimeSpan(0, 0, (int)(totalTimeSpan.TotalSeconds * 0.40));
                return file.ToSampleProvider().Skip(timeSpan);
            }
            else return null; 
        }

        public ISampleProvider SupplyHiHat(HiHatType type, WaveFileReader file)
        {
            if (type.Equals(HiHatType.Open))
                return new WaveFileReader(Host_Alpha.Properties.Resources.Ab2openhh).ToSampleProvider();
            else if (type.Equals(HiHatType.Closed))
                return new WaveFileReader(Host_Alpha.Properties.Resources.Ab2closedhh).ToSampleProvider();
            else return null;
        }
    }
}
