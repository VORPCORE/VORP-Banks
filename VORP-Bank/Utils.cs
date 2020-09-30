using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VORP_Bank
{
    public class Utils : BaseScript
    {
        public static List<int> Blips = new List<int>();
        public static List<int> Peds = new List<int>();

        public Utils()
        {
            EventHandlers["onResourceStop"] += new Action<string>(s =>
            {
                if (API.GetCurrentResourceName() == s)
                {
                    foreach (int blip in Blips)
                    {
                        int _blip = blip;
                        API.RemoveBlip(ref _blip);
                    }
                    API.SetNuiFocus(false, false);
                }
            });
        }

        public static void CreateBlips(JToken banks)
        {
            foreach (JToken bank in banks)
            {
                int blip = Function.Call<int>((Hash)0x554D9D53F696D002, 1664425300,
                    bank["coords"]["x"].ToObject<float>(), bank["coords"]["y"].ToObject<float>(), bank["coords"]["z"].ToObject<float>());
                Function.Call((Hash)0x74F74D3207ED525C, blip, bank["blipHash"].ToObject<int>(), 1);
                Function.Call((Hash)0x9CB1A1623062F402, blip, bank["blipName"].ToString());
                Blips.Add(blip);
            }
        }

        public static async void CreatePeds(JToken banks)
        {
            foreach(JToken bank in banks)
            {
                uint HashPed = (uint)API.GetHashKey(bank["NPCModel"].ToString());
                int _PedBank = API.CreatePed(HashPed, bank["npcCoords"]["x"].ToObject<float>(), bank["npcCoords"]["y"].ToObject<float>(),
                    bank["npcCoords"]["z"].ToObject<float>(), bank["npcCoords"]["h"].ToObject<float>(), false, true, true, true);
                Function.Call((Hash)0x283978A15512B2FE, _PedBank, true);
                Peds.Add(_PedBank);
                API.SetEntityNoCollisionEntity(API.PlayerPedId(), _PedBank, false);
                API.SetEntityCanBeDamaged(_PedBank, false);
                API.SetEntityInvincible(_PedBank, true);
                await Delay(2000);
                API.FreezeEntityPosition(_PedBank, true);
                API.SetBlockingOfNonTemporaryEvents(_PedBank, true);
                API.SetModelAsNoLongerNeeded(HashPed);
            }
        }

        public static async Task DrawTxt(string text, float x, float y, float fontscale, float fontsize, int r, int g, int b, int alpha, bool textcentred, bool shadow)
        {
            long str = Function.Call<long>(Hash._CREATE_VAR_STRING, 10, "LITERAL_STRING", text);
            Function.Call(Hash.SET_TEXT_SCALE, fontscale, fontsize);
            Function.Call(Hash._SET_TEXT_COLOR, r, g, b, alpha);
            Function.Call(Hash.SET_TEXT_CENTRE, textcentred);
            if (shadow) { Function.Call(Hash.SET_TEXT_DROPSHADOW, 1, 0, 0, 255); }
            Function.Call(Hash.SET_TEXT_FONT_FOR_CURRENT_COMMAND, 1);
            Function.Call(Hash._DISPLAY_TEXT, str, x, y);
        }

        public static DateTime UnixTimestampToDateTime(double unixTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (long)(unixTime * TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, System.DateTimeKind.Utc);
        }
    }
}
