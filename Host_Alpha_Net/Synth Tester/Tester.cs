using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Host_Alpha;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;


namespace Synth_Tester
{
    /*
     * A class for testing audio without having to connect a device 
     */
    public class Tester
    {
        [STAThread]
        public static void Main(string[] args)
        {

            Oscillator oscillator = new Oscillator();
            oscillator.SetOscillators(false, Oscillator.OscillatorIndex.Secondary);

            Mixer.Instance.PlayWave(oscillator.PreparePlay(500.0));
            Mixer.Instance.StopWave(oscillator.PrepareStop());

            Oscillator oscillator2 = new Oscillator();
            oscillator.SetOscillators(true, Oscillator.OscillatorIndex.Secondary);

            Mixer.Instance.PlayWave(oscillator2.PreparePlay(700.0));
            Mixer.Instance.StopWave(oscillator2.PrepareStop());
          
            Console.WriteLine("Press Any Key To Iterate Frequencies");


            List<double> frequencies = new List<double>
            { 2000.0, 500.0, 550.0, 600.0, 650.0, 700.0, 750.0, 800.0, 850.0, 900.0, 950.0, 1000.0, 1050.0, 1100.0 };
            int index = 0;
            bool active = true;
            PercussionModel model = new PercussionModel();
          
            WaveFileReader input = new WaveFileReader(Host_Alpha.Properties.Resources.Ab2closedhh);

            Wave16toIeeeProvider ieeeprovider = new Wave16toIeeeProvider(input.ToSampleProvider().ToWaveProvider16());

            while (true)
            {
                if (index == 0)
                {

                    Mixer.Instance.PlaySample(ieeeprovider.ToSampleProvider());
                    input.Position = 0;
                   
                    //Mixer.Instance.PlaySample(PercussionManager.Instance.
                    //SupplyDoublePad(PercussionManager.DoublePadType.PadOne, PercussionManager.DoublePadVelocityType.Low));
                    index++;
                }
                if (index == 1)
                {
                    //Mixer.Instance.PlaySample(PercussionManager.Instance.
                    //SupplyDoublePad(PercussionManager.DoublePadType.PadOne, PercussionManager.DoublePadVelocityType.Middle));
                    //index++;
                    index++;
                }
                if (index == 2)
                {
                    //Mixer.Instance.PlaySample(PercussionManager.Instance.
                    //SupplyDoublePad(PercussionManager.DoublePadType.PadOne, PercussionManager.DoublePadVelocityType.High));
                    //index = 0;
                    index++;
                }
                

                //Mixer.Instance.PlaySample(PercussionManager.Instance.SupplyShaker(PercussionManager.ShakerType.Forward));
                //Mixer.Instance.StopWave(oscillator.PrepareStop());
                //Mixer.Instance.StopWave(oscillator2.PrepareStop());

                //Mixer.Instance.PlayWave(oscillator.PreparePlay(frequencies[index]));
                //if (index + 1 < frequencies.ToArray().Length)
                //    Mixer.Instance.PlayWave(oscillator2.PreparePlay(frequencies[index + 1]));
                //index++;
                //if (index == frequencies.Count)
                //{
                //    index = 0;
                //    frequencies.Reverse();
                //}
                //active = !active;
                //oscillator.SetOscillators(active, Oscillator.OscillatorIndex.LFO);
                Console.WriteLine("# of Inputs: " + Mixer.Instance.GetInputs());
                Console.ReadLine(); 
            }
        }
    }
}
