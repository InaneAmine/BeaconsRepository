using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstimoteSDK.Windows
{
    class TelemetryFrame : BeaconFrameBase
    {
        #region Constants

        // Constantes
        public const int ESTIMOTE_TELEMETRY_SUBFRAME_A = 0;
        public const int ESTIMOTE_TELEMETRY_SUBFRAME_B = 1;

        #endregion

        #region Fields

        private int _protocoleVersion;
        private string _shortIdentifier;
        private int _subFrameType;
        private string _acceleration;
        private bool _isMoving;
        private string _magneticField;
        private double _ambientLightLevel;
        private string _uptimeS;
        private float _temperature;
        private int _batteryVoltage;
        private byte _batteryLevel;
        private string _errorMessage;

        /// <summary>
        /// Protocole Version of the Estimote Telemetry.
        /// Telemetry protocol version ("0", "1", "2", etc.)
        /// </summary>
        public int ProtocoleVersion
        {
            get { return _protocoleVersion; }
            set
            {
                if (_protocoleVersion == value) return;
                _protocoleVersion = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Identifier of the Estimote Telemetry.
        /// bytes 1, 2, 3, 4, 5, 6, 7, 8 => first half of the identifier of the beacon
        /// </summary>
        public string ShortIdentifier
        {
            get { return _shortIdentifier; }
            set
            {
                if (_shortIdentifier == value) return;
                _shortIdentifier = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Protocole Version of the Estimote Telemetry.
        /// Telemetry protocol version ("0", "1", "2", etc.)
        /// </summary>
        public int SubFrameType
        {
            get { return _subFrameType; }
            set
            {
                if (_subFrameType == value) return;
                _subFrameType = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Acceleration of the Estimote Telemetry Beacon.
        /// </summary>
        public string Acceleration
        {
            get { return _acceleration; }
            set
            {
                if (_acceleration == value) return;
                _acceleration = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Movement State of the Estimote Telemetry Beacon.
        /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            set
            {
                if (_isMoving == value) return;
                _isMoving = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Magnetic Field of the Estimote Telemetry Beacon.
        /// </summary>
        public string MagneticField
        {
            get { return _magneticField; }
            set
            {
                if (_magneticField == value) return;
                _magneticField = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Ambient Light Level of the Estimote Telemetry Beacon.
        /// </summary>
        public double AmbientLightLevel
        {
            get { return _ambientLightLevel; }
            set
            {
                if (_ambientLightLevel == value) return;
                _ambientLightLevel = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Temperature of the Estimote Telemetry Beacon.
        /// </summary>
        public float Temperature
        {
            get { return _temperature; }
            set
            {
                if (_temperature == value) return;
                _temperature = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Uptime String of the Estimote Telemetry Beacon.
        /// </summary>
        public string UptimeS
        {
            get { return _uptimeS; }
            set
            {
                if (_uptimeS == value) return;
                _uptimeS = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Battery Voltage of the Estimote Telemetry Beacon.
        /// </summary>
        public int BatteryVoltage
        {
            get { return _batteryVoltage; }
            set
            {
                if (_batteryVoltage == value) return;
                _batteryVoltage = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Battery Level of the Estimote Telemetry Beacon.
        /// </summary>
        public byte BatteryLevel
        {
            get { return _batteryLevel; }
            set
            {
                if (_batteryLevel == value) return;
                _batteryLevel = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Error Message of the Estimote Telemetry Beacon.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (_errorMessage == value) return;
                _errorMessage = value;
                UpdatePayload();
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors
        public TelemetryFrame(byte protocoleVersion, string identifier, byte subFrameType, string acceleration, bool isMoving, string magneticField,
            double ambientLightLevel, string uptimeS, float temperature, int batteryVoltage, byte batteryLevel, string errorMessage)
        {
            _protocoleVersion = protocoleVersion;
            _shortIdentifier = identifier;
            _subFrameType = subFrameType;
            _acceleration = acceleration;
            _isMoving = isMoving;
            _magneticField = magneticField;
            _ambientLightLevel = ambientLightLevel;
            _uptimeS = uptimeS;
            _temperature = temperature;
            _batteryVoltage = batteryVoltage;
            _batteryLevel = batteryLevel;
            _errorMessage = errorMessage;
            UpdatePayload();
        }

        public TelemetryFrame(byte[] payload) : base(payload)
        {
            ParsePayload();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the current payload into the properties exposed by this class.
        /// Has to be called if manually modifying the raw payload.
        /// </summary>
        public void ParsePayload()
        {
            // byte 0, lower 4 bits => frame type, for Telemetry it's always 2 (i.e., 0b0010)
            var frameType = Payload[2] & BinaryToDecimal("00001111");
            var ESTIMOTE_FRAME_TYPE_TELEMETRY = 2;
            if (frameType != ESTIMOTE_FRAME_TYPE_TELEMETRY) { return; }

            // byte 0, upper 4 bits => Telemetry protocol version ("0", "1", "2", etc.)
            var newProtocolVersion = (Payload[2] & BinaryToDecimal("11110000")) >> 4;
            if (newProtocolVersion != ProtocoleVersion)
            {
                _protocoleVersion = newProtocolVersion;
                OnPropertyChanged(nameof(ProtocoleVersion));
            }
            // this parser only understands version up to 2
            // (but at the time of this commit, there's no 3 or higher anyway :wink:)
            if (newProtocolVersion > 2) { return; }

            // bytes 1, 2, 3, 4, 5, 6, 7, 8 => first half of the identifier of the beacon
            string newShortIdentifier = Payload[3].ToString("X2");
            for (int i = 4; i < 11; i++)
            {
                newShortIdentifier += Payload[i].ToString("X2");
            }

            if (newShortIdentifier != ShortIdentifier)
            {
                _shortIdentifier = newShortIdentifier;
                OnPropertyChanged(nameof(ShortIdentifier));
            }

            // byte 9, lower 2 bits => Telemetry subframe type
            // to fit all the telemetry data, we currently use two packets, "A" (i.e., "0")
            // and "B" (i.e., "1")
            var newSubFrameType = Payload[11] & BinaryToDecimal("00000011");
            if (newSubFrameType != SubFrameType)
            {
                _subFrameType = newSubFrameType;
                OnPropertyChanged(nameof(SubFrameType));
            }

            // ****************
            // * SUBFRAME "A" *
            // ****************
            if (newSubFrameType == ESTIMOTE_TELEMETRY_SUBFRAME_A)
            {

                // ***** ACCELERATION
                // byte 10 => acceleration RAW_VALUE on the X axis
                // byte 11 => acceleration RAW_VALUE on the Y axis
                // byte 12 => acceleration RAW_VALUE on the Z axis
                // RAW_VALUE is a signed (two's complement) 8-bit integer
                // RAW_VALUE * 2 / 127.0 = acceleration in "g-unit" (http://www.helmets.org/g.htm)
                float[] acceleration = {
                    (Payload[12] * 2) / (float)127.0,
                    (Payload[13] * 2) / (float)127.0,
                    (Payload[14] * 2) / (float)127.0 };
                string newAcceleration = "{" + acceleration[0] + "; " + acceleration[1] + "; " + acceleration[2] + "}";
                if (newAcceleration != Acceleration)
                {
                    _acceleration = newAcceleration;
                    OnPropertyChanged(nameof(Acceleration));
                }

                // ***** MOTION STATE
                // byte 15, lower 2 bits
                // 0b00 ("0") when not moving, 0b01 ("1") when moving
                bool newIsMoving = (Payload[17] & BinaryToDecimal("00000011")) == 1;
                if (newIsMoving != IsMoving)
                {
                    _isMoving = newIsMoving;
                    OnPropertyChanged(nameof(IsMoving));
                }

                // ***** MOTION STATE DURATION
                // byte 13 => "previous" motion state duration
                // byte 14 => "current" motion state duration
                // e.g., if the beacon is currently still, "current" will state how long
                // it's been still and "previous" will state how long it's previously been
                // in motion before it stopped moving
                //
                // motion state duration is composed of two parts:
                // - lower 6 bits is a NUMBER (unsigned 6-bit integer)
                // - upper 2 bits is a unit:
                //     - 0b00 ("0") => seconds
                //     - 0b01 ("1") => minutes
                //     - 0b10 ("2") => hours
                //     - 0b11 ("3") => days if NUMBER is < 32
                //                     if it's >= 32, then it's "NUMBER - 32" weeks
                //Debug.WriteLine("Previous motion state :");
                ParseMotionStateDuration(Payload[15]);
                //Debug.WriteLine("Current motion state :");
                ParseMotionStateDuration(Payload[16]);

                // ***** GPIO
                // byte 15, upper 4 bits => state of GPIO pins, one bit per pin
                // 0 = state "low", 1 = state "high"
                var pin0 = (Payload[17] & BinaryToDecimal("00010000")) >> 4;
                //Debug.WriteLine("Pin 0");
                StateOfGPIO(pin0);
                var pin1 = (Payload[17] & BinaryToDecimal("00100000")) >> 5;
                //Debug.WriteLine("Pin 1");
                StateOfGPIO(pin1);
                var pin2 = (Payload[17] & BinaryToDecimal("01000000")) >> 6;
                //Debug.WriteLine("Pin 2");
                StateOfGPIO(pin2);
                var pin3 = (Payload[17] & BinaryToDecimal("10000000")) >> 7;
                //Debug.WriteLine("Pin 3");
                StateOfGPIO(pin3);

                // ***** ERROR CODES
                if (newProtocolVersion == 2)
                {
                    // in protocol version "2"
                    // byte 15, bits 2 & 3
                    // bit 2 => firmware error
                    // bit 3 => clock error (likely, in beacons without Real-Time Clock, e.g.,
                    //                      Proximity Beacons, the internal clock is out of sync)
                    var hasFrimwareError = (Payload[17] & BinaryToDecimal("00000100")) >> 2 == 1;
                    var hasClockError = (Payload[17] & BinaryToDecimal("00001000")) >> 3 == 1;
                    if (hasFrimwareError || hasClockError)
                    {
                        string msgError = "Firmware & clock error";
                        if (msgError != ErrorMessage)
                        {
                            _errorMessage = msgError;
                            OnPropertyChanged(nameof(ErrorMessage));
                        }
                    }
                    //Debug.WriteLine("ERROR 2:  firmware & clock error ");
                }
                else if (newProtocolVersion == 1)
                {
                    // in protocol version "1"
                    // byte 16, lower 2 bits
                    // bit 0 => firmware error
                    // bit 1 => clock error
                    var hasFrimwareError = (Payload[18] & BinaryToDecimal("00000001")) == 1;
                    var hasClockError = (Payload[18] & BinaryToDecimal("00000010")) >> 1 == 1;
                    if (hasFrimwareError || hasClockError)
                    {
                        string msgError = "Firmware & clock error";
                        if (msgError != ErrorMessage)
                        {
                            _errorMessage = msgError;
                            OnPropertyChanged(nameof(ErrorMessage));
                        }
                    }
                    //Debug.WriteLine("ERROR 1:  firmware & clock error ");

                }
                else if (newProtocolVersion == 0)
                {
                    // in protocol version "0", error codes are in subframe "B" instead
                }

                // ***** ATMOSPHERIC PRESSURE
                if (newProtocolVersion == 2)
                {
                    // added in protocol version "2"
                    // bytes 16, 17, 18, 19 => atmospheric pressure RAW_VALUE
                    // RAW_VALUE is an unsigned 32-bit integer, little-endian encoding,
                    //   i.e., least-significant byte comes first
                    //   e.g., if bytes are 16th = 0x12, 17th = 0x34, 18th = 0x56, 19th = 0x78
                    //         then the value is 0x78563412
                    // RAW_VALUE / 256.0 = atmospheric pressure in pascals (Pa)
                    // note that unlike what you see on the weather forecast, this value is
                    // not normalized to the sea level!
                    // * This parse is to add, I couldn't test it because we don't have the last Estimote Beacons
                }

            }
            // ****************
            // * SUBFRAME "B" *
            // ****************
            else if (newSubFrameType == ESTIMOTE_TELEMETRY_SUBFRAME_B)
            {
                //Debug.WriteLine("/*************** Frame B ****************/");


                // ***** MAGNETIC FIELD
                // byte 10 => normalized magnetic field RAW_VALUE on the X axis
                // byte 11 => normalized magnetic field RAW_VALUE on the Y axis
                // byte 12 => normalized magnetic field RAW_VALUE on the Z axis
                // RAW_VALUE is a signed (two's complement) 8-bit integer
                // RAW_VALUE * 2 / 128.0 = normalized value, between -1 and 1
                // the value will be 0 if the sensor hasn't been calibrated yet
                float[] magneticField = {
                    (Payload[12] * 2) / (float)128.0,
                    (Payload[13] * 2) / (float)128.0,
                    (Payload[14] * 2) / (float)128.0 };
                string newMagneticField = "{" + magneticField[0] + "; " + magneticField[1] + "; " + magneticField[2] + "}";
                if (newMagneticField != MagneticField)
                {
                    _magneticField = newMagneticField;
                    OnPropertyChanged(nameof(MagneticField));
                }


                // ***** AMBIENT LIGHT
                // byte 13 => ambient light level RAW_VALUE
                // the RAW_VALUE byte is split into two halves
                // pow(2, RAW_VALUE_UPPER_HALF) * RAW_VALUE_LOWER_HALF * 0.72 = light level in lux (lx)
                var ambientLighUpper = (Payload[15] & BinaryToDecimal("11110000")) >> 4;
                var ambientLightLower = Payload[15] & BinaryToDecimal("00001111");
                var newAmbientLightLevel = Math.Pow(2, ambientLighUpper) * ambientLightLower * 0.72;
                if (newAmbientLightLevel != AmbientLightLevel)
                {
                    _ambientLightLevel = newAmbientLightLevel;
                    OnPropertyChanged(nameof(AmbientLightLevel));
                }

                // ***** BEACON UPTIME
                // byte 14 + 6 lower bits of byte 15 (i.e., 14 bits total)
                // - the lower 12 bits (i.e., byte 14 + lower 4 bits of byte 15) are
                //   a 12-bit unsigned integer
                // - the upper 2 bits (i.e., bits 4 and 5 of byte 15) denote the unit:
                //   0b00 = seconds, 0b01 = minutes, 0b10 = hours, 0b11 = days
                var uptimeUnitCode = (Payload[17] & BinaryToDecimal("00110000")) >> 4;
                var uptimeUnit = "";
                switch (uptimeUnitCode)
                {
                    case 0: uptimeUnit = "seconds"; break;
                    case 1: uptimeUnit = "minutes"; break;
                    case 2: uptimeUnit = "hours"; break;
                    case 3: uptimeUnit = "days"; break;
                }
                var uptime = ((Payload[17] & BinaryToDecimal("00001111")) << 8) | Payload[16];
                string newUptimeS = uptime + " " + uptimeUnit;
                if (newUptimeS != UptimeS)
                {
                    _uptimeS = newUptimeS;
                    OnPropertyChanged(nameof(UptimeS));
                }

                // ***** AMBIENT TEMPERATURE
                // upper 2 bits of byte 15 + byte 16 + lower 2 bits of byte 17
                // => ambient temperature RAW_VALUE, signed (two's complement) 12-bit integer
                // RAW_VALUE / 16.0 = ambient temperature in degrees Celsius
                var temperatureRawValue = (
                    ((Payload[19] & BinaryToDecimal("00000011")) << 10) |
                    (Payload[18] << 2) |
                    ((Payload[17] & BinaryToDecimal("11000000")) >> 6));
                if (temperatureRawValue > 2047)
                {
                    temperatureRawValue = temperatureRawValue - 4096;
                }
                var newTemperature = temperatureRawValue / (float)16.0;
                if (newTemperature != Temperature)
                {
                    _temperature = newTemperature;
                    OnPropertyChanged(nameof(Temperature));
                }

                // ***** BATTERY VOLTAGE
                // upper 6 bits of byte 17 + byte 18 => battery voltage in mini-volts (mV)
                //                                      (unsigned 14-bit integer)
                // if all bits are set to 1, it means it hasn't been measured yet
                var newBatteryVoltage = ((Payload[20] << 6) | (Payload[19] & BinaryToDecimal("11111100")) >> 2);
                if (newBatteryVoltage == BinaryToDecimal("11111111111111")) /*Debug.WriteLine("Battery voltage is undefined");*/
                    if (newBatteryVoltage != BatteryVoltage)
                    {
                        _batteryVoltage = newBatteryVoltage;
                        OnPropertyChanged(nameof(BatteryVoltage));
                    }

                // ***** ERROR CODES
                // byte 19, lower 2 bits
                // see subframe A documentation of the error codes
                // starting in protocol version 1, error codes were moved to subframe A,
                // thus, you will only find them in subframe B in Telemetry protocol ver 0
                if (newProtocolVersion == 0)
                {
                    var hasFrimwareError = (Payload[21] & BinaryToDecimal("00000001")) == 1;
                    var hasClockError = (Payload[21] & BinaryToDecimal("00000010")) >> 1 == 1;
                    if (hasFrimwareError || hasClockError)
                    {
                        string msgError = "Firmware & clock error";
                        if (msgError != ErrorMessage)
                        {
                            _errorMessage = msgError;
                            OnPropertyChanged(nameof(ErrorMessage));
                        }
                    }
                    //Debug.WriteLine("ERROR 0:  firmware & clock error ");
                }

                // ***** BATTERY LEVEL
                // byte 19 => battery level, between 0% and 100%
                // if all bits are set to 1, it means it hasn't been measured yet
                // added in protocol version 1
                if (newProtocolVersion >= 1)
                {
                    var newBatteryLevel = Payload[21];
                    if (newBatteryLevel == BinaryToDecimal("11111111")) /*Debug.WriteLine("Battery voltage is undefined");*/
                        if (newBatteryVoltage != BatteryLevel)
                        {
                            _batteryLevel = newBatteryLevel;
                            OnPropertyChanged(nameof(BatteryLevel));
                        }
                }
            }
        }

        /// <summary>
        /// Convert Binary to Decimal.
        /// </summary>
        /// <param name="binary">a string variable of the binary</param>
        /// <returns></returns>
        public Int32 BinaryToDecimal(string binary)
        {
            var dec = 0;
            for (int i = 0; i < binary.Length; i++)
            {
                // we start with the least significant digit, and work our way to the left
                if (binary[binary.Length - i - 1] == '0') continue;
                dec += (int)Math.Pow(2, i);
            }
            return dec;
        }

        /// <summary>
        /// returing the state of the gpio knowing the byte
        /// </summary>
        /// <param name="bytes">byte of the gpio in the packet</param>
        public void StateOfGPIO(int bytes)
        {
            switch (bytes)
            {
                case 0:
                    //Debug.WriteLine("pin state us low");
                    break;
                case 1:
                    //Debug.WriteLine("pin state is high");
                    break;
                default:
                    //Debug.WriteLine("pin state is undefined");
                    break;
            }
        }

        /// <summary>
        /// a parseur to get the motion state duration
        /// </summary>
        /// <param name="bytes"></param>
        public void ParseMotionStateDuration(int bytes)
        {
            var number = bytes & BinaryToDecimal("00111111");
            var unitCode = (bytes = BinaryToDecimal("11000000")) >> 6;
            var unit = "";
            if (unitCode == 0)
            {
                unit = "seconds";
            }
            else if (unitCode == 1)
            {
                unit = "minutes";
            }
            else if (unitCode == 2)
            {
                unit = "hours";
            }
            else if (unitCode == 3 && number < 32)
            {
                unit = "days";
            }
            else
            {
                unit = "weeks";
                number = number - 32;
            }

            string msgError = "Number: " + number + "Unit: " + unit;
            if (msgError != ErrorMessage)
            {
                _errorMessage = msgError;
                OnPropertyChanged(nameof(ErrorMessage));
            }
            //Debug.WriteLine("Number: " + number + "Unit: " + unit);

        }

        /// <summary>
        /// Update the raw payload when properties have changed.
        /// </summary>
        private void UpdatePayload()
        {
            var header = BeaconFrameHelper.CreateTelemetryHeader(BeaconFrameHelper.TelemetryFrameType.EstimoteFrameType);
            using (var ms = new MemoryStream())
            {
                // Frame header
                ms.Write(header, 0, header.Length);
                // Protocole Version
                //ms.WriteByte(ProtocoleVersion);
                // Identifier
                var idBytes = new byte[ShortIdentifier.Length * sizeof(char)];
                Buffer.BlockCopy(ShortIdentifier.ToCharArray(), 0, idBytes, 0, idBytes.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(idBytes);
                ms.Write(idBytes, 0, idBytes.Length);
                // Save to payload (to direct array to prevent re-parsing and a potential endless loop of updating and parsing)
                _payload = ms.ToArray();
            }
        }

        /// <summary>
        /// Update the information stored in this frame with the information from the other frame.
        /// Useful for example when binding the UI to beacon information, as this will emit
        /// property changed notifications whenever a value changes - which would not be possible if
        /// you would overwrite the whole frame.
        /// </summary>
        /// <param name="otherFrame">Frame to use as source for updating the information in this beacon
        /// frame.</param>
        public override void Update(BeaconFrameBase otherFrame)
        {
            base.Update(otherFrame);
            ParsePayload();
        }

        /// <summary>
        /// Check if the contents of this frame are generally valid.
        /// Does not currently perform a deep analysis, but checks the header as well
        /// as the frame length.
        /// </summary>
        /// <returns>True if the frame is a valid Eddystone TLM frame.</returns>
        public override bool IsValid()
        {
            if (!base.IsValid()) return false;

            // 2 bytes ID: 9A FE
            // 1 byte frame type
            if (!Payload.IsTelemetryFrameType()) return false;

            // 1 byte version
            // 2 bytes battery voltage
            // 2 bytes beacon temperature
            // 4 bytes adv_cnt (AdvertisementFrameCount)
            // 4 bytes sec_cnt (TimeSincePowerUp)
            return Payload.Length == BeaconFrameHelper.TelemetryHeaderSize + 19;
        }

        #endregion
    }
}