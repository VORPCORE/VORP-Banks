using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Bank : BaseScript
    {
        private string _name;
        private Dictionary<Tuple<string,int>, BankUser> _bankUsers = new Dictionary<Tuple<string,int>, BankUser>();

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public Dictionary<Tuple<string,int>, BankUser> BankUsers
        {
            get => _bankUsers;
            set => _bankUsers = value;
        }

        public Bank(string name)
        {
            _name = name;
            API.RegisterCommand("Comprobar", new Action<dynamic, dynamic, dynamic>(async (x, y, z) =>
            {
                Task<bool> result = CheckAndRegister("steam:11000011062b830", "BlackWater");
                await result;
            }), false);
        }

        public static bool IsUserConnected(string steamId)
        {
            PlayerList playerList = new PlayerList();
            foreach (Player player in playerList)
            {
                if (("steam:" + player.Identifiers["steam"]) == steamId)
                {
                    return true;
                }
            }

            return false;
        }

        public Tuple<string,int> GetUserTuple(Player player)
        {
            string steam = "steam:" + player.Identifiers["steam"];
            dynamic UserCharacter = Server.CORE.getUser(int.Parse(player.Handle)).getUsedCharacter;
            int charidentifier = UserCharacter.charIdentifier;
            Tuple<string, int> auxtuple = new Tuple<string, int>(steam, charidentifier);
            return auxtuple;
        }

        public int GetCharacterId()
        {
            dynamic UserCharacter = Server.CORE.getUser(int.Parse(player.Handle)).getUsedCharacter;
            return charidentifier = UserCharacter.charIdentifier;
        }

        public bool Transference(Player playerSend, string toSteamId, double gold, double money, bool instant, string subject)
        {
            string steam = "steam:" + playerSend.Identifiers["steam"];
            bool done = false;
            double auxmoney = money;
            if (instant) auxmoney = money + LoadConfig.Config["transferenceCost"].ToObject<double>();
            if (_bankUsers[GetUserTuple(playerSend)].SubMoney(auxmoney) && _bankUsers[GetUserTuple(playerSend)].SubGold(gold))
            {
                TransferenceC newTransference = new TransferenceC("steam:" + playerSend.Identifiers["steam"], toSteamId, money, gold, subject, Name, Name, LoadConfig.Config["time"].ToObject<int>(), this);
                if (instant)
                {
                    newTransference.MakeTransference();
                    playerSend.TriggerEvent("vorp:refreshBank", _bankUsers[GetUserTuple(playerSend)].Money,
                        _bankUsers[GetUserTuple(playerSend)].Gold);
                    return true;
                }
                else
                {
                    if (IsUserConnected(steam)) playerSend.TriggerEvent("vorp:refreshBank", _bankUsers[GetUserTuple(playerSend)].Money,
                         _bankUsers[GetUserTuple(playerSend)].Gold);
                    Server.TransferenceList.Add(newTransference);
                    return true;
                }
            }
            return false; 
        }

        public void Withdraw([FromSource] Player source, double money, double gold)
        {
            if (!_bankUsers.ContainsKey(GetUserTuple(source))) return;
            
            if (money > 0)
            {
                if (SubUserMoney("steam:" + source.Identifiers["steam"], money))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, money);
                    //Evento que actualiza el banco
                }
            }

            if (gold > 0)
            {
                if (SubUserGold("steam:" + source.Identifiers["steam"], gold))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1, gold);
                    //Evento que actualiza el banco
                }
            }
            source.TriggerEvent("vorp:refreshBank", _bankUsers[GetUserTuple(source)].Money,
                _bankUsers[GetUserTuple(source)].Gold);
        }

        public async Task<bool> CheckAndRegister(string steamId, string Name)
        {
            dynamic result = await Exports["ghmattimysql"].executeSync("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ? and charidentifier = ?",
                    new object[] { steamId, Name, GetCharacterId() });
            if (int.Parse(result.Count.ToString()) > 0)
            {
                return true;
            }
            else
            {
                dynamic result2 = await Exports["ghmattimysql"].executeSync("INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`,`charidentifier`) VALUES (?,?,?,?,?)",
                                      new object[] { Name, steamId, 0.0, 0.0 , GetCharacterId() });
                if (int.Parse(result2.affectedRows.ToString()) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public void Deposit([FromSource] Player source, double money, double gold)
        {
            TriggerEvent("vorp:getCharacter", int.Parse(source.Handle), new Action<dynamic>(async (user) =>
            {
                double newMoney = user.money - money;
                double newGold = user.gold - gold;
                string steamId = "steam:" + source.Identifiers["steam"];
                Task<bool> resultadoConsulta = CheckAndRegister(steamId, Name);
                await resultadoConsulta;
                if (resultadoConsulta.Result)
                {
                    if (newMoney >= 0)
                    {
                        TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 0, money);
                        AddUserMoney("steam:" + source.Identifiers["steam"], money);
                    }

                    if (newGold >= 0)
                    {
                        TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 1, gold);
                        AddUserGold("steam:" + source.Identifiers["steam"], gold);
                    }
                    source.TriggerEvent("vorp:refreshBank", _bankUsers[GetUserTuple(source)].Money,
                        _bankUsers[GetUserTuple(source)].Gold);
                }
            }));
        }

        public bool AddUser(BankUser newUser)
        {
            if (!_bankUsers.ContainsKey(newUser.Identifier))
            {
                _bankUsers.Add(newUser.Identifier, newUser);
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
        public bool AddUserMoney(string steamId, double money)
        {
            if (_bankUsers.ContainsKey(steamId))
            {
                return _bankUsers[steamId].AddMoney(money);
            }
            Exports["ghmattimysql"].execute(
                $"UPDATE bank_users SET money = money + ? WHERE identifier=? and name = ?",
                new object[] { money, steamId, _name }
            );
            if (IsUserConnected(steamId))
            {
                BankUser newUser = new BankUser(Name, steamId, money, 0.0);
                _bankUsers.Add(steamId, newUser);
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

        public bool AddUserGold(string steamId, double gold)
        {
            if (_bankUsers.ContainsKey(steamId))
            {
                return _bankUsers[steamId].AddGold(gold);
            }

            Exports["ghmattimysql"].execute(
                $"UPDATE bank_users SET gold = gold + ? WHERE identifier=? and name = ?",
                new object[] { gold, steamId, _name }
            );

            if (IsUserConnected(steamId))
            {
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
