using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    //Client  
    //Game.Root -> ClientProcess -> CurrentProcess


    //Server
    //Game.Root -> GateProcess -> CurrentProcess
    //Game.Root -> LocationProcess -> CurrentProcess
    //Game.Root -> MapProcess -> CurrentProcess

    public enum ProcessType
    {
        //Common
        Root,//only one

        //Client
        Client,
        Current,


        //Server
        Gate,
        Location,
        Map,
        Http,
        Realm,
    }
}
