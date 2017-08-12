using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace StandaloneHost
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
