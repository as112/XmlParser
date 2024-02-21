using System.Xml.Serialization;

namespace XmlProcessor.Core
{
    [XmlRoot("InstrumentStatus")]
    public class InstrumentStatus
    {
        [XmlElement("PackageID")]
        public required string PackageID { get; set; }

        [XmlElement("DeviceStatus")]
        public List<DeviceStatus>? DeviceStatuses { get; set; }
    }
}
