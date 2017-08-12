using NAudio.Wave;
using NAudio.Dsp;

/*
 * Code taken from NAudio Github
 * https://github.com/naudio/NAudio/blob/master/NAudio/Wave/SampleProviders/AdsrSampleProvider.cs
 */
namespace StandaloneHost
{
    class AdsrSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly EnvelopeGenerator adsr;

        private float attack = 0.01f;
        private float decay = 0.5f;
        private float sustain = 0.01f;
        private float release = 0.3f;

        public AdsrSampleProvider(ISampleProvider source)
        {
            this.source = source;
            adsr = new EnvelopeGenerator();
            adsr.AttackRate = attack * WaveFormat.SampleRate;
            adsr.SustainLevel = sustain;
            adsr.DecayRate = decay * WaveFormat.SampleRate;
            adsr.ReleaseRate = release * WaveFormat.SampleRate;
            adsr.Gate(true);
        }

        public AdsrSampleProvider(ISampleProvider source, float Attack, float Decay, float Sustain, float Release)
        {
            this.source = source;
            adsr = new EnvelopeGenerator();
            adsr.AttackRate = Attack * WaveFormat.SampleRate;
            adsr.SustainLevel = Sustain;
            adsr.DecayRate = Decay * WaveFormat.SampleRate;
            adsr.ReleaseRate = Release * WaveFormat.SampleRate;
            adsr.Gate(true);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (adsr.State == EnvelopeGenerator.EnvelopeState.Idle) return 0; // we've finished
            var samples = source.Read(buffer, offset, count);
            for (int n = 0; n < samples; n++)
            {
                buffer[offset++] *= adsr.Process();
            }
            return samples;
        }

        public void Stop()
        {
            adsr.Gate(false);
        }

        public WaveFormat WaveFormat { get { return source.WaveFormat; } }
    }
}