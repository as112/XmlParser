using System.Xml.Serialization;

namespace XmlProcessor.Core
{
    [XmlRoot("CombinedOvenStatus")]
    public class CombinedOvenStatus : IStatus
    {
        private readonly Guid _id;
        public Guid Id => _id;
        [XmlElement("ModuleState")]
        public string ModuleState { get; set; }

        [XmlElement("IsBusy")]
        public bool IsBusy { get; set; }

        [XmlElement("IsReady")]
        public bool IsReady { get; set; }

        [XmlElement("IsError")]
        public bool IsError { get; set; }

        [XmlElement("KeyLock")]
        public bool KeyLock { get; set; }

        [XmlElement("UseTemperatureControl")]
        public bool UseTemperatureControl { get; set; }

        [XmlElement("OvenOn")]
        public bool OvenOn { get; set; }

        [XmlElement("Temperature_Actual")]
        public double TemperatureActual { get; set; }

        [XmlElement("Temperature_Room")]
        public double TemperatureRoom { get; set; }

        [XmlElement("MaximumTemperatureLimit")]
        public int MaximumTemperatureLimit { get; set; }

        [XmlElement("Valve_Position")]
        public int ValvePosition { get; set; }

        [XmlElement("Valve_Rotations")]
        public int ValveRotations { get; set; }

        [XmlElement("Buzzer")]
        public bool Buzzer { get; set; }

        public CombinedOvenStatus()
        {
            _id = Guid.NewGuid();
        }

        public override bool Equals(object? obj)
        {
            return obj is CombinedOvenStatus status &&
                   ModuleState == status.ModuleState;
        }
    }
}
