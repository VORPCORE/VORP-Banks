using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VORP_Bank
{
    class NuiControl:BaseScript
    {
        public NuiControl()
        {
            API.RegisterNuiCallbackType("Deposit");
            EventHandlers["__cfx_nui:Deposit"] += new Action<ExpandoObject>(Deposit);

            API.RegisterNuiCallbackType("Withdraw");
            EventHandlers["__cfx_nui:Withdraw"] += new Action<ExpandoObject>(Deposit);

            API.RegisterNuiCallbackType("searchUsers");
            EventHandlers["__cfx_nui:searchUsers"] += new Action<ExpandoObject>(searchUsers);

            API.RegisterNuiCallbackType("SendTransfer");
            EventHandlers["__cfx_nui:SendTransfer"] += new Action<ExpandoObject>(SendTransfer);

            API.RegisterNuiCallbackType("NUIFocusOff");
            EventHandlers["__cfx_nui:NUIFocusOff"] += new Action<ExpandoObject>(NUIFocusOff);
        }

        private void Deposit(ExpandoObject obj)
        {
            if(obj != null)
            {
                JObject data = JObject.FromObject(obj);
                Debug.WriteLine(data.ToString());
                double money = data["money"].ToObject<double>();
                double gold = data["gold"].ToObject<double>();
                
                //uno de los dos o los dos pueden tener valor si no tuvieran devuelven 0 
            }
        }

        private void Withdraw(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                Debug.WriteLine(data.ToString());
                double money = data["money"].ToObject<double>();
                double gold = data["gold"].ToObject<double>();

                //uno de los dos o los dos pueden tener valor si no tuvieran devuelven 0 

            }
        }

        private void searchUsers(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                Debug.WriteLine(data.ToString());
                string name = data["name"].ToString();

                //minimo te devuelve 3 caracteres esta bloqueado desde el html para evitar que no escriban nada
                //cuando a ti te llege esto deberas tu enviarle la lista de ususarios que empiezen por esto

            }
        }

        private void SendTransfer(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                Debug.WriteLine(data.ToString());
                string steamId = data["steam"].ToString();
                double money = data["money"].ToObject<double>();
                double gold = data["gold"].ToObject<double>();
                bool useInstantTax = data["isntant"].ToObject<bool>();
            }
        }

        private void NUIFocusOff(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                Debug.WriteLine(data.ToString());
                double money = data["money"].ToObject<double>();
                double gold = data["gold"].ToObject<double>();
                //uno de los dos o los dos pueden tener valor si no tuvieran devuelven 0 
            }
        }
    }
}
