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
            //RegisterEvents();
        }

        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            string steamId = player.Identifiers["steam"];
            foreach (KeyValuePair<string, Bank> bank in Database.Banks)
            {
                //bank.Value.DeleteUser(steamId);
            }
        }

        private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason,
            dynamic deferrals)
        {
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