using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Bank:BaseScript
    {
        private string _name;
        private Dictionary<string,BankUser> _bankUsers = new Dictionary<string, BankUser>();

        public string Name
        {
            get => _name;
            set => _name = value;
        }
        
        public Bank(string name)
        {
            this._name = name;
        }

        private bool IsUserConnected(string steamId)
        {
            PlayerList playerList = new PlayerList();
            foreach (Player player in playerList)
            {
                if (("steam:"+player.Identifiers["steam"]) == steamId)
                {
                    return true;
                    Debug.WriteLine("Esta conectado");
                }
            }
           
            return false;
        }

        //PRE: identifier not null
        //POST: return isRegistered and register it on database if not
        private void IsUserRegistered(string identifier,double money,double gold)
        {
            Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ?",
                new object[] {identifier,Name},
                new Action<dynamic>((result) =>
                {
                    if (result != null)
                    {
                        if (result.Count <= 0)
                        {
                            Debug.WriteLine("Entro a registrarlo porque es nuevo");
                            Exports["ghmattimysql"].execute("INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`) VALUES (?,?,?,?)",
                                new object[] {Name,identifier,0.0,0.0},
                                new Action<dynamic>((result2) =>
                                {
                                    if (result2 != null)
                                    {
                                        Debug.WriteLine("Lo he registrado en bd");
                                        Exports["ghmattimysql"].execute(
                                            $"UPDATE bank_users SET gold = gold + ? WHERE identifier=? and name = ?",
                                            new object[]{gold,identifier,_name}
                                        );
                                        Exports["ghmattimysql"].execute(
                                            $"UPDATE bank_users SET money = money + ? WHERE identifier=? and name = ?",
                                            new object[]{money,identifier,_name}
                                        );
                                    }
                                }));
                        }
                        else
                        {
                            Exports["ghmattimysql"].execute(
                                $"UPDATE bank_users SET gold = gold + ? WHERE identifier=? and name = ?",
                                new object[]{gold,identifier,_name}
                            );
                            Exports["ghmattimysql"].execute(
                                $"UPDATE bank_users SET money = money + ? WHERE identifier=? and name = ?",
                                new object[]{money,identifier,_name}
                            );
                        }
                    }
                }));
        }

        public bool Transference(string fromSteamId, string toSteamId, double gold, double money)
        {
            PlayerList playerList = new PlayerList();
            Player from = null;
            Player to = null;
            foreach (Player player in playerList)
            {
                if ("steam:"+player.Identifiers["steam"] == fromSteamId) from = player;
                if ("steam:"+player.Identifiers["steam"] == toSteamId) to = player;
            }
            bool done = false;
            if (money > 0.0)
            {
                done = SubUserMoney(fromSteamId, money);
                if (done) done = AddUserMoney(toSteamId, money);
                if (!done){ AddUserMoney(fromSteamId, money); return false;}
            }

            if (gold > 0.0)
            {
                done = SubUserGold(fromSteamId, gold);
                if (done) done = AddUserGold(toSteamId, gold);
                if (!done) {AddUserGold(fromSteamId, gold); return false; }
            }

            if (from != null)
            {
                from.TriggerEvent("vorp:refreshBank",_bankUsers["steam:"+from.Identifiers["steam"]].Money,
                    _bankUsers["steam:"+from.Identifiers["steam"]].Gold);
            }

            if (to != null)
            {
                to.TriggerEvent("vorp:refreshBank",_bankUsers["steam:"+to.Identifiers["steam"]].Money,
                    _bankUsers["steam:"+to.Identifiers["steam"]].Gold);
            }
            
            return done;
        }

        public void Withdraw([FromSource]Player source, double money, double gold)
        {
            if (money > 0)
            {
                if (SubUserMoney("steam:"+source.Identifiers["steam"], money))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, money);
                    //Evento que actualiza el banco
                }
            }

            if (gold > 0)
            {
                if (SubUserGold("steam:"+source.Identifiers["steam"], gold))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1, gold);
                    //Evento que actualiza el banco
                }
            }
            source.TriggerEvent("vorp:refreshBank",_bankUsers["steam:"+source.Identifiers["steam"]].Money,
                _bankUsers["steam:"+source.Identifiers["steam"]].Gold);
        }

        public void Deposit([FromSource]Player source, double money, double gold)
        {
            TriggerEvent("vorp:getCharacter", int.Parse(source.Handle), new Action<dynamic>((user) =>
            {
                double newMoney = user.money - money;
                double newGold = user.gold - gold;
                string steamId = "steam:" + source.Identifiers["steam"];
                if (newMoney >= 0)
                {
                    TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 0, money);
                    AddUserMoney("steam:"+source.Identifiers["steam"], money);
                }
                
                if (newGold >= 0)
                {
                    TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 1, gold);
                    AddUserGold("steam:"+source.Identifiers["steam"], gold);
                }
                source.TriggerEvent("vorp:refreshBank",_bankUsers[steamId].Money,
                    _bankUsers[steamId].Gold);
            }));
        }

        public bool AddUser(BankUser newUser)
        {
            if (!_bankUsers.ContainsKey(newUser.Identifier))
            {
                _bankUsers.Add(newUser.Identifier,newUser);
                return true;
            }

            return false;
        }

        public bool RemoveUser(string identifier)
        {
            if (_bankUsers.ContainsKey(identifier))
            {
                _bankUsers.Remove(identifier);
                return true;
            }

            return false;
        }

        //If user is in bank it return user else it returns null value
        public BankUser GetUser(string identifier)
        {
            if (_bankUsers.ContainsKey(identifier))
            {
                return _bankUsers[identifier];
            }

            return null;
        }

        //Pensar que puede ser que le este dando pasta a otra persona y que haya que registrarla en el banco pero no en la bd y viceversa
        public bool AddUserMoney(string steamId,double money)
        {
            if (_bankUsers.ContainsKey(steamId))
            {
                return _bankUsers[steamId].AddMoney(money);
            }


            IsUserRegistered(steamId,money,0.0);
            Debug.WriteLine("entro en dinero");
            if (IsUserConnected(steamId))
            {
                Debug.WriteLine("Usuario conectado");
                BankUser newUser = new BankUser(Name,steamId,money,0.0);
                _bankUsers.Add(steamId,newUser);
                return true;
            }
            return true;
        }

        public bool SubUserMoney(string steamId, double money)
        {
            if (_bankUsers.ContainsKey(steamId))
            {
                return _bankUsers[steamId].SubMoney(money);
            }

            return false;
        }
        
        public bool AddUserGold(string steamId,double gold)
        {
            if (_bankUsers.ContainsKey(steamId))
            {
                return _bankUsers[steamId].AddGold(gold);
            }

            IsUserRegistered(steamId,0.0,gold);
            Debug.WriteLine("entro en oro");
                
            if (IsUserConnected(steamId))
            {
                Debug.WriteLine("Usuario conectado");
                BankUser newUser = new BankUser(Name, steamId, 0.0, gold);
                _bankUsers.Add(steamId, newUser);
            }
            return true;
        }
        
        public bool SubUserGold(string steamId, double money)
        {
            if (_bankUsers.ContainsKey(steamId))
            {
                return _bankUsers[steamId].SubGold(money);
            }

            return false;
        }
    }
}
