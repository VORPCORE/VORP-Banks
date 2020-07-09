using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace VORP_BankServer
{
    public class LoadConfig:BaseScript
    {
        public LoadConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:getConfig"] += new Action<Player>(getConfig);
            EventHandlers[$"{API.GetCurrentResourceName()}:getWeapons"] += new Action<Player>(getWeapons);
        }
    }
}