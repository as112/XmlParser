using System.Xml.Serialization;

namespace XmlProcessor.Core
{
    [XmlRoot("CombinedPumpStatus")]
    public class CombinedPumpStatus :IStatus
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

        [XmlElement("Mode")]
        public string Mode { get; set; }

        [XmlElement("Flow")]
        public int Flow { get; set; }

        [XmlElement("PercentB")]
        public int PercentB { get; set; }

        [XmlElement("PercentC")]
        public int PercentC { get; set; }

        [XmlElement("PercentD")]
        public int PercentD { get; set; }

        [XmlElement("MinimumPressureLimit")]
        public int MinimumPressureLimit { get; set; }

        [XmlElement("MaximumPressureLimit")]
        public double MaximumPressureLimit { get; set; }

        [XmlElement("Pressure")]
        public int Pressure { get; set; }

        [XmlElement("PumpOn")]
        public bool PumpOn { get; set; }

        [XmlElement("Channel")]
        public int Channel { get; set; }

        public CombinedPumpStatus()
        {
            _id = Guid.NewGuid();
        }

        public override bool Equals(object? obj)
        {
            return obj is CombinedPumpStatus status &&
                   ModuleState == status.ModuleState;
        }
    }
}
