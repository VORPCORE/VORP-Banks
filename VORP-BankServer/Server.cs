using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Server : BaseScript
    {
        
        public static List<TransferenceC> TransferenceList = new List<TransferenceC>();

        public Server()
        {
            EventHandlers["vorp:bankDeposit"] += new Action<Player,string,double,double>(Deposit);
            EventHandlers["vorp:bankWithdraw"] += new Action<Player,string,double,double>(Withdraw);
            EventHandlers["vorp:bankTrasference"] += new Action<Player,string,double,double,bool,string,string>(Transference);
            // EventHandlers["vorp:bankAddMoney"] += new Action<Player, string, double, string>(addMoney);
            // EventHandlers["vorp:bankAddGold"] += new Action<Player, string, double, string>(addGold);
            // EventHandlers["vorp:bankSubMoney"] += new Action<Player, string, double, string>(subMoney);
            // EventHandlers["vorp:bankSubGold"] += new Action<Player, string, double, string>(subGold);
            // EventHandlers["vorp:userTransference"] +=
            //     new Action<Player, string, double, double, bool, string, string>(Transference);
            // EventHandlers["vorp:registerUserInBank"] += new Action<Player, string>(registerUserInBank);
            Delay(100);
            RegisterEvents();
        }

        private void Transference([FromSource] Player player, string toSteamId, double money, double gold,bool instantTak,string usedBank,string subject)
        {
            if (Database.Banks.ContainsKey(usedBank))
            {
                Database.Banks[usedBank].Transference(player, toSteamId, gold, money,instantTak,subject);
            }
        }

        private async Task ExecuteTransaction()
        {
            await Delay(1000);
            foreach (TransferenceC transference in TransferenceList)
            {
                if (transference.Time == 0)
                {
                    //Aqui se haria la consulta de base de datos para hacer la transferencia
                    TransferenceList.Remove(transference);
                }
                else
                {
                    transference.RestTime();
                }
            }
        }
        
        
        private void RegisterEvents()
        {
            TriggerEvent("vorp:addNewCallBack", "retrieveUserBankInfo", new Action<int, CallbackDelegate, dynamic>((source, cb, args) => {
                PlayerList pl = new PlayerList();
                Player p = pl[source];
                string identifier = "steam:" + p.Identifiers["steam"];
                if (Database.Banks.ContainsKey(args))
                {
                    Dictionary<string,double> userCallback = new Dictionary<string, double>(); 
                    if (Database.Banks[args].GetUser(identifier) != null)
                    {
                        userCallback.Add("money",Database.Banks[args].GetUser(identifier).Money);
                        userCallback.Add("gold",Database.Banks[args].GetUser(identifier).Gold);
                        cb(userCallback);
                    }
                    else
                    {
                        userCallback.Add("money",0.0);
                        userCallback.Add("gold",0.0);
                        cb(userCallback);
                    }
                }
            }));
            TriggerEvent("vorp:addNewCallBack","searchUsers",new Action<int,CallbackDelegate,dynamic>(
                (source, cb, args) =>
                {
                    string argsres = args;
                    string[] name = argsres.Split(' ');
                    Exports["ghmattimysql"].execute("SELECT firstname,lastname,identifier FROM characters WHERE firstname LIKE ?", new string[] {"%"+name[0]+"%" } ,new Action<dynamic>((result) =>
                    {
                        if(result != null)
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
                Database.Banks[bank].Withdraw(source,money,gold);
            }
        }
        
        private void Deposit([FromSource] Player source, string bank, double money, double gold)
        {
            if (Database.Banks.ContainsKey(bank))
            {
                Database.Banks[bank].Deposit(source,money,gold);
            }
        }

        
    }
}