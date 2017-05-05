using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host_Alpha
{
    //An interface which allows Slots to interact with the Mixer without having to make their own instance. 
    //The Manager class implements this interface and is set as the AudioListener for all slots. 
    //
    public interface AudioListener
    { 
        void OnPlayWave(MixingSampleProvider input, Slot slot);
        void OnPlayWave(MixingSampleProvider input);
        void OnStopWave(MixingSampleProvider input);

        void OnPlaySample(PercussionManager.ShakerType type, Slot slot);
        void OnPlaySample(PercussionManager.ShakerType type);
        void OnPlaySample(WaveFileReader file);
        void OnStopSample(WaveFileReader file);

        void OnPlaySample(PercussionManager.DoublePadType type, PercussionManager.DoublePadVelocityType velocity, Slot slot);
        void OnPlaySample(PercussionManager.DoublePadType type, PercussionManager.DoublePadVelocityType velocity);
        void OnPlaySample(PercussionManager.DoublePadVelocityType velocity, WaveFileReader file);

        void OnPlaySample(PercussionManager.HiHatType type, WaveFileReader file);

        void OnConnect(Slot slot);
        void OnDisconnect(Slot slot);
    }
}
