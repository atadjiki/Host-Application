using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using NAudio.Dsp;

namespace Host_Alpha//OscillatorLib 
{
    /*
     * An object used by the host to manage incoming requests from the Melody App.
     * The oscillator object actually contains a primary and secondary oscillator
     * which can both play simultaneously. Each oscillator generates a signal 
     * of a certain wavetype (sin, square, saw, triangle), and sends the signal 
     * through a chain of effects processing before playing it. Signals are first
     * sent through a Low Pass Filter and then through an ADSR envelope before 
     * being added to a mixer. 
     */
    public class Oscillator
    {
        //private WaveOut waveOut; //channel for audio output
        //private DirectSoundOut waveOut; //channel for audio output
        private MixingSampleProvider mixingSampleProvider; //manages input from both oscillators
        private SignalGenerator primarySignalGenerator; //generates signals of a given wave type
        private SignalGenerator secondarySignalGenerator;
        private SignalGenerator LFSignalGenerator; //low frequency oscillator for modulation
        private SignalGeneratorType currentPrimaryWaveType = SignalGeneratorType.Triangle; //a wave type that can be changed by the host
        private SignalGeneratorType currentSecondaryWaveType = SignalGeneratorType.Square; //corresponds with the SignalGeneratorType enum
       // private SignalGeneratorType LFOWaveType = SignalGeneratorType.Square;

        private int _oscIndex = 0;

        // 5 for now.
        private int _polyPhony = 5;

        private AdsrSampleProvider[] adsr1Array = null;
        private AdsrSampleProvider[] adsr2Array = null;

        private LowPassFilterSampleProvider[] lpf1Array = null;
        private LowPassFilterSampleProvider[] lpf2Array = null;

        private SignalGenerator[] sigGen1Array = null;
        private SignalGenerator[] sigGen2Array = null;
        
        public enum OscillatorIndex { Primary, Secondary, LFO }; // an enum for specifying oscillator when calling certain methods
        public enum ADSR { Attack, Decay, Sustain, Release }

        public static int sampleRate = 44100; //default values
        public static float amplitude = 0.25f;
        public static int channels = 1;
        private float cutoffFrequency = 300; // ? not sure if this is a good cut off?

        //adsr default values
        private float primaryAttack = 0.01f;
        private float primaryDecay = 0.0f;
        private float primarySustain;
        private float primaryRelease = 0.3f;

        private float secondaryAttack = 0.01f;
        private float secondaryDecay = 0.0f;
        private float secondarySustain;
        private float secondaryRelease = 0.3f;

        private float primaryGain = 1.0f; //use SignalGenerator's gain field instead of output's
        private float secondaryGain = 1.0f;

        private double primaryOctaveDetune = 0;
        private double secondaryOctaveDetune = -1;

        private bool primaryOscillator = true; //by default, only the primary oscillator is active on initialization
        private bool secondaryOscillator = false;
        //private bool LFO = false;
        //private double LFOfrequency = 5;

        /// <summary>
        /// For our current implementation to work,
        /// Every call to StartWave() must be followed by a call to StopWave(),
        ///     Otherwise we experience corrupted playback.
        /// In the event of rapid play signals from the phone,
        /// It is possible for StartWave to be called Twice before StopWave is called.
        /// This flag attempts to remedy this scenario.
        /// </summary>
        private bool isStopped = true;

        public Oscillator(int SampleRate, float Amplitude)
        {
            sampleRate = SampleRate;
            amplitude = Amplitude;
            Initialize();
        }

        public Oscillator()
        {
            Initialize();
        }

        private void Initialize()
        {
            //initialize mixer to take inputs
            mixingSampleProvider = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels));
            mixingSampleProvider.ReadFully = true;

            //  TODO: this is a major issue!
            //waveOut.DesiredLatency = 150;

            //initialize variables
            primarySustain = amplitude;
            secondarySustain = amplitude;

            //setup signal generators
            primarySignalGenerator = new SignalGenerator(sampleRate, channels);
            primarySignalGenerator.Type = currentPrimaryWaveType;

            secondarySignalGenerator = new SignalGenerator(sampleRate, channels);
            secondarySignalGenerator.Type = currentSecondaryWaveType;

            adsr1Array = new AdsrSampleProvider[_polyPhony];
            adsr2Array = new AdsrSampleProvider[_polyPhony];

            sigGen1Array = new SignalGenerator[_polyPhony];
            sigGen2Array = new SignalGenerator[_polyPhony];

            lpf1Array = new LowPassFilterSampleProvider[_polyPhony]; 
            lpf2Array = new LowPassFilterSampleProvider[_polyPhony];

            for (int i = 0; i < _polyPhony; i++ )
            {
                // primary
                sigGen1Array[i] = new SignalGenerator(sampleRate, channels);
                sigGen1Array[i].Type = currentPrimaryWaveType;
                sigGen1Array[i].Gain = primaryGain;
                lpf1Array[i] = new LowPassFilterSampleProvider(sigGen1Array[i].ToWaveProvider().ToSampleProvider());
                adsr1Array[i] = new AdsrSampleProvider(lpf1Array[i].ToWaveProvider().ToSampleProvider(), primaryAttack, primaryDecay, primarySustain, primaryRelease);
            
                // secondary
                sigGen2Array[i] = new SignalGenerator(sampleRate, channels);
                sigGen2Array[i].Type = currentSecondaryWaveType;
                sigGen2Array[i].Gain = secondaryGain;
                lpf2Array[i] = new LowPassFilterSampleProvider(sigGen2Array[i].ToWaveProvider().ToSampleProvider());
                adsr2Array[i] = new AdsrSampleProvider(lpf2Array[i].ToWaveProvider().ToSampleProvider(), secondaryAttack, secondaryDecay, secondarySustain, secondaryRelease);
            }

            //LFSignalGenerator = new SignalGenerator(sampleRate, channels);
            //LFSignalGenerator.Gain = 0.1;
            //LFSignalGenerator.Type = LFOWaveType ;
        }

        public void StartWave(Double frequency)
        { 
            // if this method is being called twice in a row before the call to StopWave...
            if(isStopped == false)
            {
                StopWave();
                Console.WriteLine("Consecutive StartWave call detected");
            }

            //if primary oscillator is active, chain effects 
            if (primaryOscillator)
            {
                //wrap signal generator in adsr envelope and add to mixer
                //order of processing is Signal -> LPF -> ADSR -> Mixer
                sigGen1Array[_oscIndex].Frequency = DetuneOctave(frequency, OscillatorIndex.Primary);
                lpf1Array[_oscIndex] = new LowPassFilterSampleProvider(sigGen1Array[_oscIndex].ToWaveProvider().ToSampleProvider());
                adsr1Array[_oscIndex] = new AdsrSampleProvider(sigGen1Array[_oscIndex].ToWaveProvider().ToSampleProvider(), primaryAttack, primaryDecay, primarySustain, primaryRelease);
                mixingSampleProvider.AddMixerInput(adsr1Array[_oscIndex]);

            } //same chaining process for secondary oscillator
            if (secondaryOscillator)
            {
                sigGen2Array[_oscIndex].Frequency = DetuneOctave(frequency, OscillatorIndex.Secondary);
                lpf2Array[_oscIndex] = new LowPassFilterSampleProvider(sigGen2Array[_oscIndex].ToWaveProvider().ToSampleProvider());
                adsr2Array[_oscIndex] = new AdsrSampleProvider(sigGen2Array[_oscIndex].ToWaveProvider().ToSampleProvider(), secondaryAttack, secondaryDecay, secondarySustain, secondaryRelease); 
                mixingSampleProvider.AddMixerInput(adsr2Array[_oscIndex]);
            }

            isStopped = false;

            //add LFO signalto mixer
            //if (LFO)
            //{
            //    LFSignalGenerator.Frequency = 5;
            //    OffsetSampleProvider offsetSampleProvider = new OffsetSampleProvider(LFSignalGenerator.ToWaveProvider().ToSampleProvider());
            //    mixingSampleProvider.AddMixerInput(offsetSampleProvider);
            //}
            // increment the play index upon each play
           // _oscIndex = _oscIndex == _polyPhony - 1 ? 0 : _oscIndex + 1;
        }

        public void StopWave()
        {

            //call stop method to let envelope know to begin wave decay process

            if(adsr1Array[_oscIndex] != null)
            {
                adsr1Array[_oscIndex].Stop();
            }
            if(adsr2Array[_oscIndex] != null)
            {
                adsr2Array[_oscIndex].Stop();
            }

            mixingSampleProvider.RemoveAllMixerInputs();

            // increment the array index upon each stop
           _oscIndex = _oscIndex == _polyPhony - 1 ? 0 : _oscIndex + 1;
           // Console.WriteLine(_oscIndex.ToString());

           isStopped = true;
        }

        public MixingSampleProvider PreparePlay(double frequency)
        {
            //if primary oscillator is active, chain effects 
            if (primaryOscillator)
            {
                //wrap signal generator in adsr envelope and add to mixer
                //order of processing is Signal -> LPF -> ADSR -> Mixer
                sigGen1Array[_oscIndex].Frequency = DetuneOctave(frequency, OscillatorIndex.Primary);
                lpf1Array[_oscIndex] = new LowPassFilterSampleProvider(sigGen1Array[_oscIndex].ToWaveProvider().ToSampleProvider());
                adsr1Array[_oscIndex] = new AdsrSampleProvider(sigGen1Array[_oscIndex].ToWaveProvider().ToSampleProvider(), primaryAttack, primaryDecay, primarySustain, primaryRelease);
                mixingSampleProvider.AddMixerInput(adsr1Array[_oscIndex]);

            } //same chaining process for secondary oscillator
            if (secondaryOscillator)
            {
                sigGen2Array[_oscIndex].Frequency = DetuneOctave(frequency, OscillatorIndex.Secondary);
                lpf2Array[_oscIndex] = new LowPassFilterSampleProvider(sigGen2Array[_oscIndex].ToWaveProvider().ToSampleProvider());
                adsr2Array[_oscIndex] = new AdsrSampleProvider(sigGen2Array[_oscIndex].ToWaveProvider().ToSampleProvider(), secondaryAttack, secondaryDecay, secondarySustain, secondaryRelease);
                mixingSampleProvider.AddMixerInput(adsr2Array[_oscIndex]);
            }

           // Console.WriteLine("Oscillator Passing " + mixingSampleProvider.MixerInputs.Count() + "inputs");

            return mixingSampleProvider;
        }

        public MixingSampleProvider PrepareStop()
        {
            if (adsr1Array[_oscIndex] != null)
            {
                adsr1Array[_oscIndex].Stop();
            }
            if (adsr2Array[_oscIndex] != null)
            {
                adsr2Array[_oscIndex].Stop();
            }

            mixingSampleProvider.RemoveAllMixerInputs();

            // increment the array index upon each stop
            _oscIndex = _oscIndex == _polyPhony - 1 ? 0 : _oscIndex + 1;
            Console.WriteLine(_oscIndex.ToString());

            return mixingSampleProvider;
        }

        public int DetuneOctave(double frequency, OscillatorIndex oscillator)
        {
            if (oscillator == OscillatorIndex.Primary)
            {
                return (int)(Math.Pow(2.0, primaryOctaveDetune) * frequency);
            }
            else if (oscillator == OscillatorIndex.Secondary)
            {
                return (int)(Math.Pow(2.0, secondaryOctaveDetune) * frequency);
            }
            return (int)frequency;
        }

        public void SetWaveType(SignalGeneratorType type, OscillatorIndex oscillator)
        {
            if (oscillator == OscillatorIndex.Primary)
            {
                currentPrimaryWaveType = type;
            }
            else if (oscillator == OscillatorIndex.Secondary)
            {
                currentSecondaryWaveType = type;
            }
            //else if (oscillator == OscillatorIndex.LFO)
            //{
            //    LFOWaveType = type;
            //}
        }

        public void SetVolume(float gain, OscillatorIndex oscillator)
        {
            if (oscillator == OscillatorIndex.Primary)
            {
                primaryGain = gain;
            }
            else if (oscillator == OscillatorIndex.Secondary)
            {
                secondaryGain = gain;
            }
        }

        public void SetOscillators(bool Active, OscillatorIndex oscillator)
        {
            if (oscillator == OscillatorIndex.Primary)
            {
                primaryOscillator = Active;
            }
            else if (oscillator == OscillatorIndex.Secondary)
            {
                secondaryOscillator = Active;
            }
            //else if (oscillator == OscillatorIndex.LFO)
            //{
            //    LFO = Active;
            //}
        }

        public void SetCutOffFreuency(int frequency)
        {
            cutoffFrequency = frequency;
        }


        //public void SetWaveOut(WaveOut newWaveOut)
        //public void SetWaveOut(DirectSoundOut newWaveOut)

        public void SetOctaveDetune(double amount, OscillatorIndex oscillator)
        {
            if (oscillator == OscillatorIndex.Primary)
            {
                primaryOctaveDetune = amount;
            }
            else if (oscillator == OscillatorIndex.Secondary)
            {
                secondaryOctaveDetune = amount;
            }
        }

        public int GetInputs()
        {
            return mixingSampleProvider.MixerInputs.ToArray().Length;
        }

        public void ConfigureADSR(ADSR adsr, OscillatorIndex oscillator, float newValue)
        {
            if (oscillator == OscillatorIndex.Primary)
            {
                if (adsr == ADSR.Attack)
                {
                    primaryAttack = newValue;
                }
                else if (adsr == ADSR.Decay)
                {
                    primaryDecay = newValue;
                }
                else if (adsr == ADSR.Sustain)
                {
                    primarySustain = newValue;
                }
                else if (adsr == ADSR.Release)
                {
                    primaryRelease = newValue;
                }
            }
            else if (oscillator == OscillatorIndex.Secondary)
            {
                if (adsr == ADSR.Attack)
                {
                    secondaryAttack = newValue;
                }
                else if (adsr == ADSR.Decay)
                {
                    secondaryDecay = newValue;
                }
                else if (adsr == ADSR.Sustain)
                {
                    secondarySustain = newValue;
                }
                else if (adsr == ADSR.Release)
                {
                    secondaryRelease = newValue;
                }
            }

        }

    }
}
