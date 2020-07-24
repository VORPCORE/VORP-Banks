using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    class Database:BaseScript
    {
        public static List<Player> _connectedPlayers = new List<Player>();
        public static Dictionary<string,Bank> Banks = new Dictionary<string, Bank>();

        //TODO Get all users in each bank and create dictionary bank using database
        public Database(){
            EventHandlers["playerConnecting"] += new Action<Player, string, dynamic, dynamic>(OnPlayerConnecting);
            EventHandlers["playerDropped"] += new Action<Player, string>(OnPlayerDropped);
            //Create each bank
            LoadDatabase();
        }

        private async void LoadDatabase()
        {
            await Delay(2000);
            Exports["ghmattimysql"].execute("SELECT * FROM banks", new Action<dynamic>((result) =>{
                Debug.WriteLine(result.Count.ToString());
                if(result != null){
                    foreach (var bank in result)
                    {
                        Banks.Add(bank.name.ToString(), new Bank(bank.name.ToString()));
                        Debug.WriteLine($"Added :{bank.name} with {bank.money} money and {bank.gold} gold");
                    }
                }
                PlayerList playerList = new PlayerList();
                foreach (Player player in playerList)
                {
                    LoadPlayer(player);
                }
            }));
        }
        
        private void OnPlayerDropped([FromSource] Player player, string reason)
        {
            
            string steamId = "steam:"+player.Identifiers["steam"];
            foreach (KeyValuePair<string, Bank> bank in Database.Banks)
            {
                bank.Value.RemoveUser(steamId);
            }
        }

        private void OnPlayerConnecting([FromSource] Player player, string playerName, dynamic setKickReason,
            dynamic deferrals)
        {
            LoadPlayer(player);
        }

        private void LoadPlayer([FromSource] Player player)
        {
            _connectedPlayers.Add(player);
            string steamId = "steam:"+player.Identifiers["steam"];
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
