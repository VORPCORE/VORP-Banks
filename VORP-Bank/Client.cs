using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using VORP_Bank;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankClient
{
    public class Client:BaseScript
    {
        private bool InBank = false;
        public static string UsedBank;
        public Client()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["vorp:giveUserInfo"] += new Action<double,double>(getUserInfo);
            Tick += OnBank;
        }

        private async void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;
            API.SetNuiFocus(false, false);
            API.SendNuiMessage("{\"action\": \"hide\"}");
            Debug.WriteLine("Loading banks where user is registered");
        }

        
        private async Task OnBank()
        {
            if (!GetConfig.IsLoaded) return;
            
            Vector3 playerCoords = API.GetEntityCoords(API.PlayerPedId(), true, true);
            foreach (JToken util in GetConfig.Config["Banks"])
            {
                if (API.GetDistanceBetweenCoords(util["coords"]["x"].ToObject<float>(), util["coords"]["y"].ToObject<float>(), util["coords"]["z"].ToObject<float>(), playerCoords.X,
                    playerCoords.Y, playerCoords.Z, false) <= 1.0f)
                {
                    await Utils.DrawTxt("Presiona ~e~Espacio~q~ para hablar con el bankero", 0.5f, 0.9f, 0.7f, 0.7f, 255, 255, 255, 255,
                        true, true);
                    if (API.IsControlJustPressed(2, 0xD9D0E1C0))
                    {
                        await OpenBank(util["name"].ToString(),util["hudName"].ToString());
                    }
                }
            }
        }

        private async Task OpenBank(string bank,string hudname)
        {
            InBank = true;
            TriggerEvent("vorp:ExecuteServerCallBack", "retrieveUserBankInfo", new Action<dynamic>((args) =>
            {
                Debug.WriteLine(bank);
                JObject data = new JObject();
                data.Add("action", "showAccount");
                data.Add("bank",hudname); //Normalmente cuando hagas el archivo de traduccion recuerda poner los nobmres de los bancos rollo Saint Denis Bank
                data.Add("money",double.Parse(args.money.ToString()));
                data.Add("gold", double.Parse(args.gold.ToString()));
                API.SendNuiMessage(data.ToString());
                API.SetNuiFocus(true, true);
                UsedBank = bank;
            }),bank);
        }

        private async void getUserInfo(double gold, double money)
        {
            API.SendNuiMessage($"");
        }

        private void CloseBank(ExpandoObject obj)
        {
            API.SetNuiFocus(false,false);
            API.SendNuiMessage("{\"action\": \"hide\"}");
        }

    }

}
