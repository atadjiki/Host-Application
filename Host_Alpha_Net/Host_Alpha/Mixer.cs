using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Host_Alpha
{
    /*
     * A class that mixes all audio from multiple oscillators. 
     * Since all active oscillators must share the same output, 
     * it isn't possible for each of them to have their own instance of such. 
     * Instead, audio buffers are sent to the mixer which sends them to one place
     * 
     * The Mixer also handles .wav inputs. It is recommended to not exceed 10 connected devices
     * which can affect audio quality. 
     */

    public sealed class Mixer
    {

        private static volatile Mixer instance;
        private static object syncRoot = new Object();
     //   private WaveOutEvent waveOut;
        private AsioOut waveOut;
       // private DirectSoundOut waveOut;
        public static int sampleRate = 44100; //default values
        public static int channels = 1;
        private MixingSampleProvider mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
        private VolumeSampleProvider VSP;
        private PanningSampleProvider PSP;
        private float baseVolume = 1;
        
        private float basePan = 0;

        public HashSet<Slot> slots = new HashSet<Slot>();
        private Dictionary<Slot, float> panValues = new Dictionary<Slot, float>();
        private Dictionary<Slot, float> volumeValues = new Dictionary<Slot, float>();

        private Mixer() 
        {
            try
            {
                mixer.ReadFully = true;
                mixer.MixerInputEnded += OnInputEnded;
                VSP = new VolumeSampleProvider(mixer);
                VSP.Volume = baseVolume;
                PSP = new PanningSampleProvider(VSP);
                PSP.Pan = basePan;
               // waveOut = new WaveOutEvent();
               // waveOut = new DirectSoundOut(40);
        
                waveOut = new NAudio.Wave.AsioOut("ASIO4ALL v2");
                //waveOut = new AsioOut();
                waveOut.Init(PSP);
                waveOut.Play();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static Mixer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new Mixer();
                    }
                }

                return instance;
            }
        }

        
        public void PlayWave(MixingSampleProvider mixingSampleProvider, Slot slot)
        {
            Console.WriteLine("playing input wave");
            mixer.AddMixerInput(ApplyPanVolume(mixingSampleProvider, slot));
        }

        public void PlayWave(MixingSampleProvider mixingSampleProvider)
        {
            Console.WriteLine("playing input wave");
            mixer.AddMixerInput(mixingSampleProvider);
        }

        public void StopWave(MixingSampleProvider mixingSampleProvider)
        {
            Console.WriteLine("stopping input wave");
            if (mixer.MixerInputs.Contains(mixingSampleProvider))
            {
                mixer.RemoveMixerInput(mixingSampleProvider);
            }

        }

        public void StopWave(MixingSampleProvider mixingSampleProvider, Slot slot)
        {
            Console.WriteLine("stopping input wave");
            if (mixer.MixerInputs.Contains(mixingSampleProvider))
            {
                mixer.RemoveMixerInput(ApplyPanVolume(mixingSampleProvider, slot));
            }

        }

        public void PlaySample(ISampleProvider sampleProvider, Slot slot)
        {
            Console.WriteLine("playing input sample");
          //  mixer.AddMixerInput(ApplyPanVolume(sampleProvider, slot));  
            mixer.AddMixerInput(sampleProvider);
        }

        public void PlaySample(ISampleProvider sampleProvider)
        {
            Console.WriteLine("playing input sample");
            mixer.AddMixerInput(ProcessSample(sampleProvider));

           // mixer.AddMixerInput(sampleProvider);
        }

        public void PlaySample(bool sampler, ISampleProvider sampleProvider)
        {
            Console.WriteLine("playing input sample");
            mixer.AddMixerInput(ProcessSample(sampleProvider));
            SamplerSlot.lastPlayed = ProcessSample(sampleProvider);

            // mixer.AddMixerInput(sampleProvider);
        }

        public void StopSample(ISampleProvider sampleProvider)
        {
            if (mixer.MixerInputs.Contains(sampleProvider))
            {
                Console.WriteLine("stopping input sample");
                mixer.RemoveMixerInput(sampleProvider);
            }  
        }

        public int GetInputs()
        {
            return mixer.MixerInputs.ToArray().Length;
        }

        public void OnInputEnded(object sender, EventArgs e)
        {
            Console.WriteLine("Input Ended");
        }

        public WaveFormat GetFormat()
        {
            return mixer.WaveFormat;
        }

        public void SetMasterVolume(float volume)
        {
            Console.WriteLine("setting master volume to " + volume);
            VSP.Volume = volume;

        }

        public void SetMasterPan(float pan)
        {
            Console.WriteLine("setting master pan to " + pan);
            PSP.Pan = pan;
        }

        public void Dispose()
        {
            waveOut.Dispose();
        }

        public ISampleProvider ApplyPanVolume(ISampleProvider input, Slot slot)
        {
            //get the specified pan for this slot
            float pan = panValues[slot];
            float volume = volumeValues[slot];

            MixingSampleProvider result = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
            result.AddMixerInput(input);

            VolumeSampleProvider vsp = new VolumeSampleProvider(result);
            vsp.Volume = volume;

            PanningSampleProvider psp = new PanningSampleProvider(vsp);
            psp.Pan = pan;

            return result;
            
        }

        public void AddSlot(Slot slot)
        {
            Console.WriteLine("adding slot: " + slot._type);
            slots.Add(slot);
            panValues.Add(slot, basePan);
            volumeValues.Add(slot, baseVolume);
            Console.WriteLine("Current Polling " + Mixer.Instance.GetSlots().Count + " slot(s)");
        }

        public void RemoveSlot(Slot slot)
        {
            Console.WriteLine("removing slot: " + slot._type);
            slots.Remove(slot);
            panValues.Remove(slot);
            volumeValues.Remove(slot);
            Console.WriteLine("Current Polling " + Mixer.Instance.GetSlots().Count + " slot(s)");
        }

        public List<Slot> GetSlots()
        {
            return slots.ToList();
        }

        public void SetPanValue(float pan, Slot slot)
        {
            Console.WriteLine("setting device pan for " + slot._type + " to " + pan);
            panValues[slot] = pan;
        }

        public void SetVolumeValue(float volume, Slot slot)
        {
            Console.WriteLine("setting device volume for " + slot._type + " to " + volume);
            volumeValues[slot] = volume;
        }

        public ISampleProvider ProcessSample(ISampleProvider input)
        {
            Wave16toIeeeProvider ieeeprovider;
            if (!input.WaveFormat.Equals(Mixer.Instance.mixer.WaveFormat))
            {
                ieeeprovider = new Wave16toIeeeProvider(input.ToWaveProvider16());
                return ieeeprovider.ToSampleProvider();
            }
            else
            {
                return input;
            }
                 
        }
    }
}
