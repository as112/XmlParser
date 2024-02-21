namespace XmlProcessor.Core
{
    public class InstrumentStatusDto
    {
        public required string PackageID { get; set; }
        public List<DeviceStatusDto>? DeviceStatus { get; set; }
    }
}
