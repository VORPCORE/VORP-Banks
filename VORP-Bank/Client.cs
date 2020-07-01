using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using VORP_Bank;

namespace VORP_BankClient
{
    public class Main:BaseScript
    {
        private bool InBank = false;
        public Main()
        {
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
            EventHandlers["vorp:giveUserInfo"] += new Action<double,double>(getUserInfo);
            Tick += onBank;
        }

        private async void OnClientResourceStart(string resourceName)
        {
            if (API.GetCurrentResourceName() != resourceName) return;
            API.SetNuiFocus(false, false);
            API.SendNuiMessage("{\"action\": \"hide\"}");
            Debug.WriteLine("Loading banks where user is registered");
        }


        [Tick]
        private async Task onBank()
        {
            Vector3 playerCoords = API.GetEntityCoords(API.PlayerPedId(), true, true);
            foreach (KeyValuePair<string,Vector3> util in Utils.bankPositions)
            {
                if (API.GetDistanceBetweenCoords(util.Value.X, util.Value.Y, util.Value.Z, playerCoords.X,
                    playerCoords.Y, playerCoords.Z, false) <= 1.0f)
                {
                    await Utils.DrawTxt("Presiona para hablar con el bankero", 0.5f, 0.9f, 0.7f, 0.7f, 355, 255, 255, 255,
                        true, true);
                    if (API.IsControlJustPressed(2, 0xD9D0E1C0))
                    {
                        await OpenBank(util.Key);
                    }
                }
            }
        }

        private async Task OpenBank(string bank)
        {
            API.SetNuiFocus(true,true);
            API.SendNuiMessage("{\"action\":\"display\"}");
            InBank = true;
            TriggerServerEvent("vorp:retrieveUserInfo",bank);
        }

        private void getUserInfo(double gold, double money)
        {

        }

        private async Task CloseBank()
        {
            API.SetNuiFocus(false,false);
            API.SendNuiMessage("{\"action\": \"hide\"}");
        }

    }

}
