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
            foreach (Player player in Server._connectedPlayers)
            {
                if (player.Identifiers["steam"] == steamId)
                {
                    return true;
                }
            }

            return false;
        }

        //PRE: identifier not null
        //POST: return isRegistered and register it on database if not
        private bool IsUserRegistered(string identifier)
        {
            bool registered = false;
            Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ?",
                new object[] {identifier,Name},
                new Action<dynamic>((result) =>
                {
                    if (result.Count > 0)
                    {
                        registered = true;
                    }
                }));
            if (!registered)
            {
                Exports["ghmattimysql"].execute("INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`) VALUES (?,?,?,?)",
                    new object[] {Name,identifier,0.0,0.0},
                    new Action<dynamic>((result) =>
                    {
                        if (result.Count <= 0)
                        {
                            registered = false;
                        }
                        else
                        {
                            registered = true;
                        }
                    }));
            }
            return registered;
        }

        public bool Transference(string fromSteamId, string toSteamId, double gold, double money)
        {
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

            return done;
        }

        public void Withdraw([FromSource]Player source, double money, double gold)
        {
            if (money > 0)
            {
                if (SubUserMoney(source.Identifiers["steam"], money))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, money);
                    //Evento que actualiza el banco
                }
            }

            if (gold > 0)
            {
                if (SubUserGold(source.Identifiers["steam"], gold))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1, gold);
                    //Evento que actualiza el banco
                }
            }
        }

        public void Deposit([FromSource]Player source, double money, double gold)
        {
            TriggerEvent("vorp:getCharacter", int.Parse(source.Handle), new Action<dynamic>((user) =>
            {
                double newMoney = user.money - money;
                double newGold = user.gold - gold;
                if (newMoney >= 0)
                {
                    TriggerEvent("vorp:removeMoney", source, 0, money);
                    AddUserMoney(source.Identifiers["steam"], money);
                }

                if (newGold >= 0)
                {
                    TriggerEvent("vorp:removeMoney", source, 1, gold);
                    AddUserGold(source.Identifiers["steam"], gold);
                }
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

            if (IsUserRegistered(steamId))
            {
                Exports["ghmattimysql"].execute(
                    $"UPDATE bank_users SET money = money + ? WHERE identifier=? and name = ?",
                    new object[]{money,steamId,_name}
                );
                if (IsUserConnected(steamId))
                {
                    BankUser newUser = new BankUser(Name,steamId,money,0.0);
                    _bankUsers.Add(steamId,newUser);
                }
                return true;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error registering user with Steam Id {steamId}");
            Console.ForegroundColor = ConsoleColor.White;
            return false;
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

            if (IsUserRegistered(steamId))
            {
                Exports["ghmattimysql"].execute(
                    $"UPDATE bank_users SET gold = gold + ? WHERE identifier=? and name = ?",
                    new object[]{gold,steamId,_name}
                );
                if (IsUserConnected(steamId))
                {
                    BankUser newUser = new BankUser(Name,steamId,0.0,gold);
                    _bankUsers.Add(steamId,newUser);
                }
                return true;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error registering user with Steam Id {steamId}");
            Console.ForegroundColor = ConsoleColor.White;
            return false;
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
