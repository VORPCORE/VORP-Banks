using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Server : BaseScript
    {
        public static List<Player> _connectedPlayers = new List<Player>();
        public Server()
        {
            // EventHandlers["vorp:bankAddMoney"] += new Action<Player, string, double, string>(addMoney);
            // EventHandlers["vorp:bankAddGold"] += new Action<Player, string, double, string>(addGold);
            // EventHandlers["vorp:bankSubMoney"] += new Action<Player, string, double, string>(subMoney);
            // EventHandlers["vorp:bankSubGold"] += new Action<Player, string, double, string>(subGold);
            // EventHandlers["vorp:userTransference"] +=
            //     new Action<Player, string, double, double, bool, string, string>(Transference);
            // EventHandlers["vorp:registerUserInBank"] += new Action<Player, string>(registerUserInBank);
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            Delay(100);
            RegisterEvents();
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
            
            TriggerEvent("vorp:addNewCallBack","Deposit",new Action<int,CallbackDelegate,dynamic>((source, cb, args) =>
            {
                PlayerList pl = new PlayerList();
                Player p = pl[source];
                string identifier = "steam:" + p.Identifiers["steam"];
                
            }));
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            _connectedPlayers.Remove(player);
            string steamId = player.Identifiers["steam"];
            foreach (KeyValuePair<string, Bank> bank in Database.Banks)
            {
                bank.Value.RemoveUser(steamId);
            }
        }

        private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason,
            dynamic deferrals)
        {
            _connectedPlayers.Add(player);
            string steamId = player.Identifiers["steam"];
            Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ?", new object[] {steamId},
                new Action<dynamic>((aresult) =>
                {
                    if (aresult != null)
                    {
                        foreach (dynamic user in aresult)
                        {
                            if (user != null)
                            {
                                if (Database.Banks.ContainsKey(user.name.ToString()))
                                {
                                    Bank aux = Database.Banks[user.name.ToString()];
                                    aux.AddUser(new BankUser(aux.Name,
                                        user.identifier.ToString(),double.Parse(user.gold.ToString()), 
                                        double.Parse(user.money.ToString())));
                                }
                            }
                        }
                    }
                }));
        }
    }
}