using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//public enum SceneType
//{
//    None = -1,
//    Process = 0,
//    Manager = 1,
//    Realm = 2,
//    Gate = 3,
//    Http = 4,
//    Location = 5,
//    Map = 6,
//    Router = 7,
//    RouterManager = 8,
//    Robot = 9,
//    BenchmarkClient = 10,
//    BenchmarkServer = 11,
//    Benchmark = 12,

//    // 客户端Model层
//    Client = 31,
//    Current = 34,
//}

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
