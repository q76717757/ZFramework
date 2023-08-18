using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    [DomainLoader]
    public class DomainLoader : IDomainLoader
    {
        public void Load()
        {
            var commands = Environment.GetCommandLineArgs();
            foreach (var item in commands)
            {
                Log.Info(item);
            }
            Game.LoadDomain(new GateProcess() { DomainID = 0 });
            Game.LoadDomain(new DaemonProcess() { DomainID = 1 });
            Game.LoadDomain(new SimpleProcess() { DomainID = 2 });
        }
    }
}
