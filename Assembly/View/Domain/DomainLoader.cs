using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [DomainLoader]
    public class DomainLoader : IDomainLoader
    {
        public void Load()
        {
            Game.LoadDomain(new ClientDomain() { DomainID = 0 });
            Game.LoadDomain(new TestDomain() { DomainID = 1 });
        }
    }

    [DomainLoader(1)]
    public class DomainLoader2 : IDomainLoader
    {
        public void Load()
        {
            Game.LoadDomain(new TestDomain() { DomainID = 0 });
        }
    }
}
