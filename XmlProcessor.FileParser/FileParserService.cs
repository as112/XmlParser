using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Text.Json;
using XmlProcessor.Core;
using System.Net.NetworkInformation;
using System.Linq.Expressions;

namespace XmlProcessor.FileParser
{
    public class FileParserService
    {
        private string? _xmlDirectory;
        private readonly ILogger _logger;
        private readonly RabbitMqService _mqService;

        public FileParserService(IConfiguration configuration, ILogger logger, RabbitMqService mqService)
        {
            _xmlDirectory = configuration.GetValue<string>("xmlDirecrory") ?? string.Empty;
            _logger = logger;
            _mqService = mqService;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            DirectoryInfo dir = new DirectoryInfo(_xmlDirectory);

            
            if (!Directory.Exists(_xmlDirectory))
            {
                _logger.Log(LogLevel.Error, $"{_xmlDirectory} not found");
                return;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    FileInfo[] files = dir.GetFiles().Where(e => e.Extension == ".xml").ToArray();
                    foreach (FileInfo file in files)
                    {
                        Task task = Task.Run(() =>
                        {
                            var instrumentStatus = ProcessFile(file);
                            var json = JsonSerializer.Serialize(instrumentStatus);
                            _mqService.SendMessage(json);
                        });
                        tasks.Add(task);
                    }
                    await Task.WhenAll(tasks);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.Log(LogLevel.Error, ex.Message);
                }
            }
        }

        private InstrumentStatusDto? ProcessFile(FileInfo file)
        {
            var instrumentStatus = ParseXml<InstrumentStatus>(file.FullName) as InstrumentStatus;
            InstrumentStatusDto? instrumentStatusDto = null;
            if (instrumentStatus != null)
            {
                instrumentStatusDto = new InstrumentStatusDto { PackageID = instrumentStatus.PackageID, DeviceStatus = new() };

                foreach (var status in instrumentStatus.DeviceStatuses)
                {
                    if (status.ModuleCategoryID == "SAMPLER")
                    {
                        var deviceStatusDto = new DeviceStatusDto { ModuleCategoryID = status.ModuleCategoryID, RapidControlStatus = new() };
                        var rapidControlStatus = (IStatus)ParseXml<CombinedSamplerStatus>(status.RapidControlStatus);
                        rapidControlStatus.UpdateModuleState();
                        deviceStatusDto.RapidControlStatus.CombinedSamplerStatus = rapidControlStatus as CombinedSamplerStatus;
                        instrumentStatusDto.DeviceStatus.Add(deviceStatusDto);
                    }
                    else if (status.ModuleCategoryID == "QUATPUMP")
                    {
                        var deviceStatusDto = new DeviceStatusDto { ModuleCategoryID = status.ModuleCategoryID, RapidControlStatus = new() };
                        var rapidControlStatus = (IStatus)ParseXml<CombinedPumpStatus>(status.RapidControlStatus);
                        rapidControlStatus.UpdateModuleState();
                        deviceStatusDto.RapidControlStatus.CombinedPumpStatus = rapidControlStatus as CombinedPumpStatus;
                        instrumentStatusDto.DeviceStatus.Add(deviceStatusDto);
                    }
                    else if (status.ModuleCategoryID == "COLCOMP")
                    {
                        var deviceStatusDto = new DeviceStatusDto { ModuleCategoryID = status.ModuleCategoryID, RapidControlStatus = new() };
                        var rapidControlStatus = (IStatus)ParseXml<CombinedOvenStatus>(status.RapidControlStatus);
                        rapidControlStatus.UpdateModuleState();
                        deviceStatusDto.RapidControlStatus.CombinedOvenStatus = rapidControlStatus as CombinedOvenStatus;
                        instrumentStatusDto.DeviceStatus.Add(deviceStatusDto);
                    }
                    else
                    {
                        _logger.Log(LogLevel.Error, $"ModuleCategoryID has unknown value {status.ModuleCategoryID}. File {file.FullName}");
                    }
                }
            }
            return instrumentStatusDto;
        }

        private object ParseXml<T>(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            var obj = new Object();
            try
            {
                if (File.Exists(xml))
                {
                    using (FileStream fileStream = new FileStream(xml, FileMode.Open))
                    {
                        obj = serializer.Deserialize(fileStream);
                        _logger.Log(LogLevel.Information, $"File {xml} was parsed successfully");
                    }
                }
                else
                {
                    using (MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
                    {
                        obj = serializer.Deserialize(memoryStream);
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception($"File {xml} was not parsed. {ex.Message}");
            }
            return obj;
        }
    }
}
