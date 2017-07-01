using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteSDK.Windows
{
    /// <summary>
    /// Generic beacon frame class for a frame not known to this library.
    /// It provides access to the raw payload for other classes to analyze.
    /// </summary>
    public class UnknownBeaconFrame : BeaconFrameBase
    {
        public UnknownBeaconFrame(byte[] payload) : base(payload)
        {
        }
    }
}