using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Server : BaseScript
    {

        public static List<TransferenceC> TransferenceList = new List<TransferenceC>();
        public static dynamic CORE;
        public Server()
        {
            EventHandlers["vorp:bankDeposit"] += new Action<Player, string, double, double>(Deposit);
            EventHandlers["vorp:bankWithdraw"] += new Action<Player, string, double, double>(Withdraw);
            EventHandlers["vorp:bankTrasference"] += new Action<Player, string, double, double, bool, string, string>(Transference);
            Delay(100);
            RegisterEvents();
            Tick += ExecuteTransaction;
            TriggerEvent("getCore", new Action<dynamic>((core) =>
             {
                 CORE = core;
             }));
            API.RegisterCommand("BaseDatos", new Action<dynamic, dynamic, dynamic>(async (x, y, z) =>
            {
                dynamic result2 = await Exports["ghmattimysql"].executeSync("SELECT DATE_FORMAT(DATE, '%W %M %e %Y'),money,gold,reason,toIdentifier FROM transactions WHERE fromIdentifier = ? OR toIdentifier = ?",
                   new object[] { "steam:11000011062b830", "steam:11000011062b830" });
                string str = JsonConvert.SerializeObject(result2);
            }), false);
        }

        private void Transference([FromSource] Player player, string toSteamId, double money, double gold, bool instantTak, string usedBank, string subject)
        {
            if (Database.Banks.ContainsKey(usedBank))
            {
                bool TransferenceResult = Database.Banks[usedBank].Transference(player, toSteamId, gold, money, instantTak, subject);
                if (!TransferenceResult && instantTak)
                {
                    player.TriggerEvent("vorp:Tip", LoadConfig.Langs["InsuficientMoneyInstatnTrasference"], 2000);
                }
                else if (!TransferenceResult) {
                    player.TriggerEvent("vorp:Tip", LoadConfig.Langs["InsuficientMoneyTrasference"], 2000);
                }
            }
        }

        private async Task ExecuteTransaction()
        {
            await Delay(1000);
            List<TransferenceC> auxTransference = TransferenceList.ToList();
            for (int i = 0; i < auxTransference.Count; i++)
            {
                if (auxTransference[i].Time <= 0)
                {
                    auxTransference[i].MakeTransference();
                    TransferenceList.RemoveAt(i);
                }
                else
                {
                    auxTransference[i].RestTime();
                }
            }

        }


        private void RegisterEvents()
        {
            TriggerEvent("vorp:addNewCallBack", "retrieveUserBankInfo", new Action<int, CallbackDelegate, dynamic>(async (source, cb, args) =>
            {
                PlayerList pl = new PlayerList();
                Player p = pl[source];
                string identifier = "steam:" + p.Identifiers["steam"];
                dynamic result = await Exports["ghmattimysql"].executeSync("SELECT DATE_FORMAT(DATE, '%W %M %e %Y'),money,gold,reason,toIdentifier FROM transactions WHERE fromIdentifier = ? OR toIdentifier = ?",
                    new object[] { identifier, identifier });
                string str = JsonConvert.SerializeObject(result);
                if (Database.Banks.ContainsKey(args))
                {
                    Dictionary<string, dynamic> userCallback = new Dictionary<string, dynamic>();
                    if (Database.Banks[args].GetUser(identifier) != null)
                    {
                        userCallback.Add("money", Database.Banks[args].GetUser(identifier).Money);
                        userCallback.Add("gold", Database.Banks[args].GetUser(identifier).Gold);
                        userCallback.Add("transaction", str);
                        userCallback.Add("identifier", identifier);
                        cb(userCallback);
                    }
                    else
                    {
                        userCallback.Add("money", 0.0);
                        userCallback.Add("gold", 0.0);
                        userCallback.Add("transaction", str);
                        userCallback.Add("identifier", identifier);
                        cb(userCallback);
                    }
                }
            }));
            TriggerEvent("vorp:addNewCallBack", "searchUsers", new Action<int, CallbackDelegate, dynamic>(
                (source, cb, args) =>
                {
                    string argsres = args;
                    string[] name = argsres.Split(' ');
                    Exports["ghmattimysql"].execute("SELECT firstname,lastname,identifier FROM characters WHERE firstname LIKE ?", new string[] { "%" + name[0] + "%" }, new Action<dynamic>((result) =>
                         {
                             if (result != null)
                             {
                                 cb(result);
                             }
                         }));
                }));
        }

        private void Withdraw([FromSource] Player source, string bank, double money, double gold)
        {
            if (Database.Banks.ContainsKey(bank))
            {
                Database.Banks[bank].Withdraw(source, money, gold);
            }
        }

        private void Deposit([FromSource] Player source, string bank, double money, double gold)
        {
            if (Database.Banks.ContainsKey(bank))
            {
                Database.Banks[bank].Deposit(source, money, gold);
            }
        }
    }
}