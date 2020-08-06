using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    class LoadConfig : BaseScript
    {
        public static JObject Config = new JObject();
        public static string ConfigString;
        public static Dictionary<string, string> Langs = new Dictionary<string, string>();
        public static string resourcePath = $"{API.GetResourcePath(API.GetCurrentResourceName())}";

        public static bool isConfigLoaded = false;

        public LoadConfig()
        {
            EventHandlers[$"{API.GetCurrentResourceName()}:getConfig"] += new Action<Player>(getConfig);

            LoadConfigAndLang();
        }

        private void LoadConfigAndLang()
        {
            if (File.Exists($"{resourcePath}/Config.json"))
            {
                ConfigString = File.ReadAllText($"{resourcePath}/Config.json", Encoding.UTF8);
                Config = JObject.Parse(ConfigString);
                if (File.Exists($"{resourcePath}/{Config["defaultlang"]}.json"))
                {
                    string langstring = File.ReadAllText($"{resourcePath}/{Config["defaultlang"]}.json", Encoding.UTF8);
                    Langs = JsonConvert.DeserializeObject<Dictionary<string, string>>(langstring);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{API.GetCurrentResourceName()}: Language {Config["defaultlang"]}.json loaded!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{API.GetCurrentResourceName()}: {Config["defaultlang"]}.json Not Found");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{API.GetCurrentResourceName()}: Config.json Not Found");
                Console.ForegroundColor = ConsoleColor.White;
            }
            isConfigLoaded = true;
            CreateOrUpdateDatabase();
            
        }
        private async void CreateOrUpdateDatabase(){
            await Delay(300);
            await Exports["ghmattimysql"].executeSync(LoadConfig.Config["banksSQL"].ToString());
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[VORP_BANKS]banks table loaded");
            await Exports["ghmattimysql"].executeSync(LoadConfig.Config["bankUsersSQL"].ToString());
            Console.WriteLine("[VORP_BANKS]user_banks table loaded");
            await Exports["ghmattimysql"].executeSync(LoadConfig.Config["bankTransactionsSQL"].ToString());
            Console.WriteLine("[VORP_BANKS]transactions table loaded");
            Console.ForegroundColor = ConsoleColor.White;
            // Exports["ghmattimysql"].execute("SELECT * FROM banks", new Action<dynamic>((result) =>
            // {
            //     if (result != null)
            //     {
                    
            //     }
            // }));
            foreach(JToken x in Config["Banks"]){
                Debug.WriteLine(x["name"].ToString());
                string name = x["name"].ToString();
                await Exports["ghmattimysql"].executeSync($"INSERT IGNORE INTO `banks` (`name`) VALUES ('{name}')");
            }
            TriggerEvent("banks_LoadDatabase");
            
        }

        private async void getConfig([FromSource] Player source)
        {
            source.TriggerEvent($"{API.GetCurrentResourceName()}:SendConfig", ConfigString, Langs);
        }
    }
}