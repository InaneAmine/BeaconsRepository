using System;

namespace EstimoteSDK.Windows
{
    /// <summary>
    /// Exception occured when parsing or assembling Bluetooth Beacons.
    /// </summary>
    class BeaconException : Exception
    {
        public BeaconException() { }
        public BeaconException(string message) : base(message) { }
        public BeaconException(string message, Exception inner) : base(message, inner) { }
    }
}
