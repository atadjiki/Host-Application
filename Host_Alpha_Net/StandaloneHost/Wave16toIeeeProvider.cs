﻿using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandaloneHost
{
    /*
     * Via NAudio Tutorial by Mark Heath 
     * http://markheath.net/post/convert-16-bit-pcm-to-ieee-float
     */
    /// <summary>
    /// Converts 16 bit PCM to IEEE float, optionally adjusting volume along the way
    /// </summary>
    public class Wave16toIeeeProvider : IWaveProvider
    {
        private IWaveProvider sourceProvider;
        private readonly WaveFormat waveFormat;
        private volatile float volume;
        private byte[] sourceBuffer;

        /// <summary>
        /// Creates a new Wave16toIeeeProvider
        /// </summary>
        /// <param name="sourceStream">the source stream</param>
        /// <param name="volume">stream volume (1 is 0dB)</param>
        /// <param name="pan">pan control (-1 to 1)</param>
        public Wave16toIeeeProvider(IWaveProvider sourceProvider)
        {
            if (sourceProvider.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
                throw new ApplicationException("Only PCM supported");
            if (sourceProvider.WaveFormat.BitsPerSample != 16)
                throw new ApplicationException("Only 16 bit audio supported");

            waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(Mixer.sampleRate, Mixer.channels);

            this.sourceProvider = sourceProvider;
            this.volume = 1.0f;
        }

        /// <summary>
        /// Helper function to avoid creating a new buffer every read
        /// </summary>
        byte[] GetSourceBuffer(int bytesRequired)
        {
            if (sourceBuffer == null || sourceBuffer.Length < bytesRequired)
            {
                sourceBuffer = new byte[bytesRequired];
            }
            return sourceBuffer;
        }

        /// <summary>
        /// Reads bytes from this wave stream
        /// </summary>
        /// <param name="destBuffer">The destination buffer</param>
        /// <param name="offset">Offset into the destination buffer</param>
        /// <param name="numBytes">Number of bytes read</param>
        /// <returns>Number of bytes read.</returns>
        public int Read(byte[] destBuffer, int offset, int numBytes)
        {
            int sourceBytesRequired = numBytes / 2;
            byte[] sourceBuffer = GetSourceBuffer(sourceBytesRequired);
            int sourceBytesRead = sourceProvider.Read(sourceBuffer, offset, sourceBytesRequired);
            WaveBuffer sourceWaveBuffer = new WaveBuffer(sourceBuffer);
            WaveBuffer destWaveBuffer = new WaveBuffer(destBuffer);

            int sourceSamples = sourceBytesRead / 2;
            int destOffset = offset / 4;
            for (int sample = 0; sample < sourceSamples; sample++)
            {
                destWaveBuffer.FloatBuffer[destOffset++] = (sourceWaveBuffer.ShortBuffer[sample] / 32768f) * volume;
            }

            return sourceSamples * 4;
        }

        /// <summary>
        /// <see cref="IWaveProvider.WaveFormat"/>
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        /// <summary>
        /// Volume of this channel. 1.0 = full scale
        /// </summary>
        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }
    }
}
