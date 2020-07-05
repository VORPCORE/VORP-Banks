using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORP_Bank
{
    class NuiControl:BaseScript
    {
        public NuiControl()
        {
            API.RegisterNuiCallbackType("Transfer");
            EventHandlers["__cfx_nui:Transfer"] += new Action<ExpandoObject>(Transfer);
        }

        private void Transfer(ExpandoObject obj)
        {
            if(obj != null)
            {
                
            }
        }
    }
}
