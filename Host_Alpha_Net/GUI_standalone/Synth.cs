using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_standalone
{
    public partial class Synth : Form
    {


        enum note { c, cSharp, d, dSharp, e, f, fSharp, g, gSharp, a, aSharp, b };

        Dictionary<note, double> frequencies = new Dictionary<note,double>()
        {
             {note.c, 16.35},      {note.cSharp, 17.32}, {note.d, 18.35},       {note.dSharp, 19.45}, 
             {note.e, 20.60},      {note.f, 21.83},      {note.fSharp, 23.12},  {note.g, 24.50}, 
             {note.gSharp, 25.96}, {note.a, 27.50},      {note.aSharp, 29.14},  {note.b, 30.87}, 

        };

        StandaloneHost.Oscillator oscillator = new StandaloneHost.Oscillator();


        public Synth()
        {
            InitializeComponent();
            octaveList.SelectedIndex = 5;
            this.KeyPreview = true;
           // oscillator.SetOscillators(true, StandaloneHost.Oscillator.OscillatorIndex.Primary);
          //  oscillator.SetOscillators(true, StandaloneHost.Oscillator.OscillatorIndex.Secondary);
          //  oscillator.SetWaveType(NAudio.Wave.SampleProviders.SignalGeneratorType.Sin, StandaloneHost.Oscillator.OscillatorIndex.Primary);
           // oscillator.SetWaveType(NAudio.Wave.SampleProviders.SignalGeneratorType.SawTooth, StandaloneHost.Oscillator.OscillatorIndex.Primary);
        }

        private void Synth_FormClosed(object sender, FormClosedEventArgs e)
        {
            Launcher.CloseSynth();
        }

        private void playNote(int octave, note note)
        {
            //get frequency of note
            double frequency = frequencies[note];
            //convert by octave
            frequency = (frequency * (octave + 1) * 10);

            //play note
            var sample = oscillator.PreparePlay(frequency);
            StandaloneHost.Mixer.Instance.PlayWave(sample);

        }

        private void stopNote(int octave, note note)
        {
            //get frequency of note
            double frequency = frequencies[note];
            //convert by octave
            frequency = (frequency * (octave + 1) * 10);

            //stop note
            var sample = oscillator.PreparePlay(frequency);
            StandaloneHost.Mixer.Instance.StopWave(sample);

        }

        private void c1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.c);
        }

        private void c1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.c);
        }

        private void d1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.d);
        }
       
        private void d1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.d);
        }

        private void e1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.e);
        }

        private void e1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.e);
        }

        private void f1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.f);
        }

        private void f1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.f);
        }

        private void g1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.g);
        }

        private void g1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.g);
        }

        private void a1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.a);
        }

        private void a1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.a);
        }

        private void b1Button_MouseDown(object sender, MouseEventArgs e)
        {
            playNote(octaveList.SelectedIndex, note.b);
        }

        private void b1Button_MouseUp(object sender, MouseEventArgs e)
        {
            stopNote(octaveList.SelectedIndex, note.b);
        }

        private void Synth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                playNote(octaveList.SelectedIndex, note.c);
            else if (e.KeyCode == Keys.W)
                playNote(octaveList.SelectedIndex, note.cSharp);

            else if (e.KeyCode == Keys.S)
                playNote(octaveList.SelectedIndex, note.d);
            else if (e.KeyCode == Keys.E)
                playNote(octaveList.SelectedIndex, note.dSharp);

            else if (e.KeyCode == Keys.D)
                playNote(octaveList.SelectedIndex, note.e);
            else if (e.KeyCode == Keys.F)
                playNote(octaveList.SelectedIndex, note.f);
            else if (e.KeyCode == Keys.T)
                playNote(octaveList.SelectedIndex, note.fSharp);

            else if (e.KeyCode == Keys.G)
                playNote(octaveList.SelectedIndex, note.g);
            else if (e.KeyCode == Keys.Y)
                playNote(octaveList.SelectedIndex, note.gSharp);

            else if (e.KeyCode == Keys.H)
                playNote(octaveList.SelectedIndex, note.a);
            else if (e.KeyCode == Keys.U)
                playNote(octaveList.SelectedIndex, note.aSharp);
            else if (e.KeyCode == Keys.J)
                playNote(octaveList.SelectedIndex, note.b);
        }

        private void Synth_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A)
                stopNote(octaveList.SelectedIndex, note.c);
            else if (e.KeyCode == Keys.W)
                stopNote(octaveList.SelectedIndex, note.cSharp);

            else if (e.KeyCode == Keys.S)
                stopNote(octaveList.SelectedIndex, note.d);
            else if (e.KeyCode == Keys.E)
                stopNote(octaveList.SelectedIndex, note.dSharp);

            else if (e.KeyCode == Keys.D)
                stopNote(octaveList.SelectedIndex, note.e);
            else if (e.KeyCode == Keys.F)
                stopNote(octaveList.SelectedIndex, note.f);
            else if (e.KeyCode == Keys.T)
                stopNote(octaveList.SelectedIndex, note.fSharp);

            else if (e.KeyCode == Keys.G)
                stopNote(octaveList.SelectedIndex, note.g);
            else if (e.KeyCode == Keys.Y)
                stopNote(octaveList.SelectedIndex, note.gSharp);

            else if (e.KeyCode == Keys.H)
                stopNote(octaveList.SelectedIndex, note.a);
            else if (e.KeyCode == Keys.U)
                stopNote(octaveList.SelectedIndex, note.aSharp);
            else if (e.KeyCode == Keys.J)
                stopNote(octaveList.SelectedIndex, note.b);
        }

        



        //private void test_Click(object sender, EventArgs e)
        //{

        //    StandaloneHost.Oscillator oscillator = new StandaloneHost.Oscillator();

        //    StandaloneHost.Mixer.Instance.PlaySample(oscillator.PreparePlay(500));
        //}
    }
}
