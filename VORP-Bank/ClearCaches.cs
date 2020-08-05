using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using static CitizenFX.Core.Native.API;
namespace VORP_Bank
{
    class ClearCaches : BaseScript
    {
        public ClearCaches()
        {
            EventHandlers["onResourceStop"] += new Action<string>(OnResourceStop);
        }

        private void OnResourceStop(string resource)
        {
            if (GetCurrentResourceName() != resource) return;
            foreach(int blip in Utils.Blips)
            {
                int _blip = blip;
                RemoveBlip(ref _blip);
            }

            foreach(int npc in Utils.Peds)
            {
                int _ped = npc;
                DeletePed(ref _ped);
            }

        }
    }
}
