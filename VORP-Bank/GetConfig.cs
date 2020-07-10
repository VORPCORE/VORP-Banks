using System;
using System.Collections.Generic;
using System.Dynamic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_Bank
{
    public class GetConfig:BaseScript
    {
        public static JObject Config = new JObject();
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();
        public static JArray PlayerWeapons = new JArray();

        public GetConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:SendConfig"] += new Action<string, ExpandoObject>(LoadDefaultConfig);
            EventHandlers[$"{API.GetCurrentResourceName()}:SendWeapons"] += new Action<string>(LoadPlayerWeapons);
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getConfig");
        }

        private void LoadDefaultConfig(string dc, ExpandoObject dl)
        {
            Config = JObject.Parse(dc);

            foreach (var l in dl)
            {
                Langs[l.Key] = l.Value.ToString();
            }

            weaponstore_init.InitStores();
        }

        private void LoadPlayerWeapons(string w)
        {
            PlayerWeapons = JArray.Parse(w);
        }

        public static void ForceLoadWeapons()
        {
            TriggerServerEvent($"{API.GetCurrentResourceName()}:getWeapons");
        }
    }
}