using System.Xml.Serialization;
using XmlProcessor.Core.Models;

namespace XmlProcessor.Core
{
    public class DeviceStatus
    {

        [XmlElement("ModuleCategoryID")]
        public required string ModuleCategoryID { get; set; }

        [XmlElement("IndexWithinRole")]
        public int IndexWithinRole { get; set; }

        [XmlElement("RapidControlStatus")]
        public string? RapidControlStatus { get; set; }
       
    }
}
