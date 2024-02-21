using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlProcessor.Core
{
    public interface IStatus
    {
        public Guid Id { get; }
        string ModuleState { get; set; }
        public void UpdateModuleState()
        {
            var r = new Random();
            ModuleState = Enum.GetName(typeof(ModuleStates), r.Next(Enum.GetNames(typeof(ModuleStates)).Length)) ?? "ERROR";
        }
    }
    enum ModuleStates
    {
        Online,
        Offline,
        Run,
        NotReady
    }
}