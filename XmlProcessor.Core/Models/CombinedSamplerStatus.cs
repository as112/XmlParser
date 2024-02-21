﻿using System.Xml.Serialization;

namespace XmlProcessor.Core
{
    [XmlRoot("CombinedSamplerStatus")]
    public class CombinedSamplerStatus : IStatus
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

        [XmlElement("Status")]
        public int Status { get; set; }

        [XmlElement("Vial")]
        public string Vial { get; set; }

        [XmlElement("Volume")]
        public int Volume { get; set; }

        [XmlElement("MaximumInjectionVolume")]
        public int MaximumInjectionVolume { get; set; }

        [XmlElement("RackL")]
        public string RackL { get; set; }

        [XmlElement("RackR")]
        public string RackR { get; set; }

        [XmlElement("RackInf")]
        public int RackInf { get; set; }

        [XmlElement("Buzzer")]
        public bool Buzzer { get; set; }

        public CombinedSamplerStatus()
        {
            _id = Guid.NewGuid();
        }

        public override bool Equals(object? obj)
        {
            return obj is CombinedSamplerStatus status &&
                   ModuleState == status.ModuleState;
        }
    }
}
