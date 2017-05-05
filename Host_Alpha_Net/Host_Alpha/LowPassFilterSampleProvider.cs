using NAudio.Dsp;
using NAudio.Wave;

namespace Host_Alpha
{
    /*
     * Code written with the help of the NAudio Tutorial Videos on PluralSight
     * https://www.pluralsight.com/courses/audio-programming-naudio
     */
    public class LowPassFilterSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider source;
        private readonly BiQuadFilter filter;

        public LowPassFilterSampleProvider(ISampleProvider source)
        {
            this.source = source;
            filter = BiQuadFilter.LowPassFilter(source.WaveFormat.SampleRate, 4000, 0.7f);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var samples = source.Read(buffer, offset, count);
            for(var n = 0; n < samples; n++)
            {
                buffer[offset + n] = filter.Transform(buffer[offset + n]);
            }
            return samples;
        }

        public WaveFormat WaveFormat { get { return source.WaveFormat; } }
    }
}
