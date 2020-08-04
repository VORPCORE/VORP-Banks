using CitizenFX.Core;
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
        }
    }
}
