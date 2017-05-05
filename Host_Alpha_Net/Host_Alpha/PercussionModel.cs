using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host_Alpha
{
    /* An object that represents an instance of a Percussion app
     * The percussion app contains audio files for the Shaker, Rotator, Double Pad and Hi Hat. 
     */
    public class PercussionModel
    {
        public int _numberOfReaders = 5;
        public int _currentDP1ReaderIndex = 0;
        public int _currentDP2ReaderIndex = 0;

        public WaveFileReader hi_shk1;
        public WaveFileReader hi_shk2;
        public WaveFileReader[] dp_1;
        public WaveFileReader[] dp_2;
        public WaveFileReader hho;
        public WaveFileReader hhc;

        public List<string> fileSlots = new List<string> { "hi_shk1", "hi_shk2", "dp_1", "dp_2", "hh_open", "hh_closed"};

        /*
         * Initialize audio files to default
         * until user manually syncs custom files
         */
        public PercussionModel()
        {
            hi_shk1 = new WaveFileReader(Host_Alpha.Properties.Resources.Hi_Shk1);
            hi_shk2 = new WaveFileReader(Host_Alpha.Properties.Resources.Hi_Shk2);

            dp_1 = new WaveFileReader[_numberOfReaders];
            dp_2 = new WaveFileReader[_numberOfReaders];

            for (int i = 0; i < _numberOfReaders; i++ )
            {
                dp_1[i] = new WaveFileReader(Host_Alpha.Properties.Resources.DP_1);
                dp_2[i] = new WaveFileReader(Host_Alpha.Properties.Resources.PSC_Analogue_Snare_11);
            }
                //dp_1 = new WaveFileReader(Host_Alpha.Properties.Resources.DP_1);
                //dp_2 = new WaveFileReader(Host_Alpha.Properties.Resources.DP_2);

                hho = new WaveFileReader(Host_Alpha.Properties.Resources.Dol_H1);
            hhc = new WaveFileReader(Host_Alpha.Properties.Resources.Cln_H3);
        }

        public void SetFile(string fileSlot, WaveFileReader reader)
        {
            //shaker
            if (fileSlot.Equals("hi_shk1"))
            {
                hi_shk1 = reader;
            }
            else if (fileSlot.Equals("hi_shk2"))
            {
                hi_shk2 = reader;
            }

            //double pad
            else if (fileSlot.Equals("dp_1"))
            {
                for (int i = 0; i < _numberOfReaders; i++)
                {
                    dp_1[i] = reader;
                }

                    //dp_1 = reader;
            } 
            else if (fileSlot.Equals("dp_2"))
            {
                for (int i = 0; i < _numberOfReaders; i++)
                {
                    dp_2[i] = reader;
                }

                //dp_2 = reader;
            }

            //hi hat
            else if(fileSlot.Equals("hh_open"))
            {
                hho = reader;
            }
            else if(fileSlot.Equals("hh_closed"))
            {
                hhc = reader;
            }
        }

        internal bool ContainsFileSlot(string fileSlot)
        {
            return fileSlots.Contains(fileSlot);
        }
    }
}
