using Host_Alpha;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI_Alpha
{
    public partial class ControlPanel : Form
    {
        private Slot currentSlot = null;
        public ControlPanel()
        {
            InitializeComponent();
            deviceList.Text = "Devices";
            deviceVolume.Visible = false;
            devicePan.Visible = false;
        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {

        }

        private void ControlPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            Mixer.Instance.Dispose();
        }

        
        private void masterVolume_ValueChanged(object sender, EventArgs e)
        {
            SetMasterVolume((float) masterVolume.Value);
            
        }

        private void masterPan_ValueChanged(object sender, EventArgs e)
        {
            SetMasterPan((float)masterPan.Value);
        }

        private void deviceList_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Current Polling " + Mixer.Instance.GetSlots().Count + " slot(s)");

            if (devicePan.Visible == false || deviceVolume.Visible == false)
            {
                devicePan.Visible = true;
                deviceVolume.Visible = true;
            }
            int index = deviceList.SelectedIndex;

            //retrieve selected slot
            if(Mixer.Instance.GetSlots().Count > index && !ReferenceEquals(Mixer.Instance.GetSlots()[index], null))
                currentSlot = Mixer.Instance.GetSlots()[index];
        }

        private void deviceVolume_ValueChanged(object sender, EventArgs e)
        {
            SetDeviceVolume((float)deviceVolume.Value);
        }

        private void devicePan_ValueChanged(object sender, EventArgs e)
        {
            SetDevicePan((float)devicePan.Value);
        }

        public static void SetMasterVolume(float volume)
        {
            Mixer.Instance.SetMasterVolume(volume / 100);
        }

        public static void SetMasterPan(float pan)
        {
            Mixer.Instance.SetMasterPan(pan / 100);
        }

        private void SetDeviceVolume(float volume)
        {
            Mixer.Instance.SetVolumeValue((volume / 100), currentSlot);
        }

        private void SetDevicePan(float pan)
        {
            Mixer.Instance.SetPanValue((pan / 100), currentSlot);
        }

        private void deviceList_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Current Polling " + Mixer.Instance.GetSlots().Count + " slot(s)");
            Console.WriteLine("Device List Contains " + deviceList.Items.Count + " items");

            List<Slot> dataSource = Mixer.Instance.slots.ToList();
            deviceList.DataSource = dataSource;
            deviceList.DisplayMember = "_type";
            deviceList.ValueMember = "_type";
            deviceList.Refresh();
            deviceList.Invalidate();
            
        }
    }
}
