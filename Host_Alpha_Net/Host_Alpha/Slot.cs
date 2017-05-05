using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Host_Alpha
{
    /*
     * A base class for SamplerSlot, MelodySlot, PercussionSlot
     */
    public class Slot
    {
        /// <summary>interface used to communicate with manager instance </summary>
        internal AudioListener listener;
        /// <summary>used for the final step of the UDP handshake between the host and the phone</summary>
        internal UdpClient _sendingSocket = null;
        /// <summary>   melody, percussion, or sampler  </summary>
        public string _type { get; set; }
        public void SetAudioListener(AudioListener _listener) { listener = _listener; }
        public void PollConnection()
        {
            //if the client connected has dropped, 
            //alert the mixer to remove the client
            if (!_sendingSocket.Client.Connected)
            {
                listener.OnDisconnect(this);
            }
        }
    }
}
