using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using VORP_BankClient;

/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_Bank
{
    class NuiControl : BaseScript
    {
        public NuiControl()
        {
            EventHandlers["vorp:refreshBank"] += new Action<double, double>(RefreshBank);
            EventHandlers["vorp:refreshTransactions"] += new Action<string,string>(RefreshTransactions);
            API.RegisterNuiCallbackType("Deposit");
            EventHandlers["__cfx_nui:Deposit"] += new Action<ExpandoObject>(Deposit);

            API.RegisterNuiCallbackType("Withdraw");
            EventHandlers["__cfx_nui:Withdraw"] += new Action<ExpandoObject>(Withdraw);

            API.RegisterNuiCallbackType("SearchUsers");
            EventHandlers["__cfx_nui:SearchUsers"] += new Action<ExpandoObject>(searchUsers);

            API.RegisterNuiCallbackType("Transfer");
            EventHandlers["__cfx_nui:Transfer"] += new Action<ExpandoObject>(SendTransfer);

            API.RegisterNuiCallbackType("NUIFocusOff");
            EventHandlers["__cfx_nui:NUIFocusOff"] += new Action<ExpandoObject>(NUIFocusOff);
        }

        private void RefreshTransactions(string transaction,string identifier){
            JObject data2 = new JObject();
            if (transaction != "[]")
            {
                JArray trans = new JArray();
                JArray transactions = JArray.Parse(transaction);
                foreach (var transactionit in transactions)
                {
                    JObject obj = new JObject();
                    obj.Add("date", transactionit["DATE_FORMAT(DATE, '%W %M %e %Y')"]);
                    obj.Add("money", transactionit["money"]);
                    obj.Add("gold", transactionit["gold"]);
                    obj.Add("msg", transactionit["reason"]);
                    if (transactionit["toIdentifier"].ToString() == identifier)
                    {
                        obj.Add("operation", "Received");
                    }
                    else
                    {
                        obj.Add("operation", "Sended");
                    }
                    trans.Add(obj);
                }
                data2.Add("action", "updateTransactions");
                data2.Add("transfers", trans);
                API.SendNuiMessage(data2.ToString());
            }
        }

        private void RefreshBank(double money, double gold)
        {
            JObject data = new JObject();
            data.Add("action", "updateNumbers");
            data.Add("money", money);
            data.Add("gold", gold);
            API.SendNuiMessage(data.ToString());
        }

        private void Deposit(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                double money = data["money"].ToObject<double>();
                double gold = data["gold"].ToObject<double>();

                TriggerServerEvent("vorp:bankDeposit", Client.UsedBank, money, gold);
                //uno de los dos o los dos pueden tener valor si no tuvieran devuelven 0 
            }
        }

        private void Withdraw(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                double money = data["money"].ToObject<double>();
                double gold = data["gold"].ToObject<double>();
                TriggerServerEvent("vorp:bankWithdraw", Client.UsedBank, money, gold);
                //uno de los dos o los dos pueden tener valor si no tuvieran devuelven 0 
            }
        }

        private void searchUsers(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                string name = data["name"].ToString();
                JObject sendData = new JObject();
                TriggerEvent("vorp:ExecuteServerCallBack", "searchUsers", new Action<dynamic>((args) =>
                {
                    sendData.Add("action", "showUsers");
                    JArray userList = new JArray();
                    foreach (var user in args)
                    {
                        JObject useraux = new JObject();
                        string resultname = user.firstname + " " + user.lastname;
                        useraux.Add("name", resultname);
                        useraux.Add("steam", user.identifier);
                        userList.Add(useraux);
                    }
                    sendData.Add("userList", userList);
                    API.SendNuiMessage(sendData.ToString());
                }), name);
            }
        }

        private void SendTransfer(ExpandoObject obj)
        {
            if (obj != null)
            {
                JObject data = JObject.FromObject(obj);
                string steamId = data["steam"].ToString();
                double money = double.Parse(data["money"].ToString());
                double gold = double.Parse(data["gold"].ToString());
                bool useInstantTax = data["instant"].ToObject<bool>();
                string subject = data["subject"].ToString();
                TriggerServerEvent("vorp:bankTrasference", steamId, money, gold, useInstantTax, Client.UsedBank, subject);
            }
        }

        private void NUIFocusOff(ExpandoObject obj)
        {
            if (obj != null)
            {
                API.SetNuiFocus(false, false);
            }
        }
    }
}
