using System.Xml.Serialization;

namespace XmlProcessor.Core.Models
{
    [XmlRoot("RapidControlStatus")]
    public class RapidControlStatus
    {
        private readonly Guid _id;
        public Guid Id => _id;

        [XmlElement("CombinedSamplerStatus")]
        public CombinedSamplerStatus? CombinedSamplerStatus { get; set; }

        [XmlElement("CombinedPumpStatus")]
        public CombinedPumpStatus? CombinedPumpStatus { get; set; }

        [XmlElement("CombinedOvenStatus")]
        public CombinedOvenStatus? CombinedOvenStatus { get; set;}

        public RapidControlStatus()
        {
            _id = Guid.NewGuid();
        }
    }
}
