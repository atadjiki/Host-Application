using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_standalone
{
    static class Launcher
    {
        //initialize synth and sampler forms
        //there should only ever be one instance of these open
        public static bool synthOpen;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //make sure to initialize ASIO4ALL
            StandaloneHost.Mixer.Instance.GetInputs();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new mainMenu());

        }

        public static void LaunchSynth()
        {
            if (synthOpen)
            {
                //only allow one instance
                MessageBox.Show("Only one instance of Synth allowed.","Standalone Host" );
            }
            else
            {

                synthOpen = true;
                Synth synth = new Synth();
                synth.Show();
            }

        }

        internal static void CloseSynth()
        {
            synthOpen = false;
        }
    }
}

