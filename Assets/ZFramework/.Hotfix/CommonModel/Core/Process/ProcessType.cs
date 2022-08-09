using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ZFramework
{
    //commong
    //Game.Root   only one

    //Client  
    //Game.Root -> ClientProcess -> components


    //Server
    //Game.Root -> GateProcess -> components
    //          -> LocationProcess -> components
    //          -> MapProcess -> components

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
