using XmlProcessor.Core.Models;

namespace XmlProcessor.Core
{
    public class DeviceStatusDto
    {
        public required string ModuleCategoryID { get; set; }
        public int IndexWithinRole { get; set; }
        public RapidControlStatus? RapidControlStatus { get; set; }

    }
}
