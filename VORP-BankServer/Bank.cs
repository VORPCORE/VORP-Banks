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

        public int GetCharacterId(Player source)
        {
            dynamic UserCharacter = Server.CORE.getUser(int.Parse(source.Handle)).getUsedCharacter;
            return UserCharacter.charIdentifier;
        }

        public bool Transference(Player playerSend, string toSteamId, int charid, double gold, double money, bool instant, string subject)
        {
            string steam = "steam:" + playerSend.Identifiers["steam"];
            bool done = false;
            double auxmoney = money;
            if (instant) auxmoney = money + LoadConfig.Config["transferenceCost"].ToObject<double>();
            if (_bankUsers[GetUserTuple(playerSend)].SubMoney(auxmoney) && _bankUsers[GetUserTuple(playerSend)].SubGold(gold))
            {
                TransferenceC newTransference = new TransferenceC("steam:" + playerSend.Identifiers["steam"], GetCharacterId(playerSend), toSteamId, charid, money, gold, subject, Name, Name, LoadConfig.Config["time"].ToObject<int>(), this);
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
                if (SubUserMoney(GetUserTuple(source), money))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, money);
                    //Evento que actualiza el banco
                }
            }

            if (gold > 0)
            {
                if (SubUserGold(GetUserTuple(source), gold))
                {
                    TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1, gold);
                    //Evento que actualiza el banco
                }
            }
            source.TriggerEvent("vorp:refreshBank", _bankUsers[GetUserTuple(source)].Money,
                _bankUsers[GetUserTuple(source)].Gold);
        }

        public async Task<bool> CheckAndRegister(string steamId, string Name,Player source)
        {
            dynamic result = await Exports["ghmattimysql"].executeSync("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ? and charidentifier = ?",
                    new object[] { steamId, Name, GetCharacterId(source) });
            if (int.Parse(result.Count.ToString()) > 0)
            {
                return true;
            }
            else
            {
                dynamic result2 = await Exports["ghmattimysql"].executeSync("INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`,`charidentifier`) VALUES (?,?,?,?,?)",
                                      new object[] { Name, steamId, 0.0, 0.0 , GetCharacterId(source) });
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

        public async void Deposit([FromSource] Player source, double money, double gold)
        {
            dynamic UserCharacter = Server.CORE.getUser(int.Parse(source.Handle)).getUsedCharacter;
            double newMoney = UserCharacter.money - money;
            double newGold = UserCharacter.gold - gold;
            string steamId = "steam:" + source.Identifiers["steam"];
            Task<bool> resultadoConsulta = CheckAndRegister(steamId, Name, source);
            await resultadoConsulta;
            if (resultadoConsulta.Result)
            {
                if (newMoney >= 0)
                {
                    UserCharacter.removeCurrency(0, money);
                    AddUserMoney(GetUserTuple(source), money);
                }

                if (newGold >= 0)
                {
                    UserCharacter.removeCurrency(1, gold);
                    AddUserGold(GetUserTuple(source), gold);
                }
                source.TriggerEvent("vorp:refreshBank", _bankUsers[GetUserTuple(source)].Money,
                    _bankUsers[GetUserTuple(source)].Gold);
            }
        }

        public bool AddUser(BankUser newUser)
        {
            Tuple<string, int> aux = new Tuple<string, int>(newUser.Identifier, newUser.CharIdentifier);
            if (!_bankUsers.ContainsKey(aux))
            {
                _bankUsers.Add(aux, newUser);
                return true;
            }

            return false;
        }

        public void RemoveUser(string identifier)
        {
            List<Tuple<string, int>> auxiliar = new List<Tuple<string, int>>();
            foreach(KeyValuePair<Tuple<string,int>,BankUser> aux in _bankUsers)
            {
                if(aux.Key.Item1 == identifier)
                {
                    auxiliar.Add(aux.Key);
                }
            }
            
            foreach(Tuple<string,int> ident in auxiliar)
            {
                _bankUsers.Remove(ident);
            }
        }

        //If user is in bank it return user else it returns null value
        public BankUser GetUser(string identifier,int charidentifier)
        {
            Tuple<string, int> aux = new Tuple<string, int>(identifier, charidentifier);
            if (_bankUsers.ContainsKey(aux))
            {
                return _bankUsers[aux];
            }

            return null;
        }

        //Pensar que puede ser que le este dando pasta a otra persona y que haya que registrarla en el banco pero no en la bd y viceversa
        public bool AddUserMoney(Tuple<string,int> aux,double money)
        {
            if (_bankUsers.ContainsKey(aux))
            {
                return _bankUsers[aux].AddMoney(money);
            }
            Exports["ghmattimysql"].execute(
                $"UPDATE bank_users SET money = money + ? WHERE identifier=? and name = ? and charidentifier = ?",
                new object[] { money, aux.Item1, _name,aux.Item2 }
            );
            if (IsUserConnected(aux.Item1))
            {
                BankUser newUser = new BankUser(Name, aux.Item1, aux.Item2 ,money, 0.0);
                _bankUsers.Add(aux, newUser);
                return true;
            }
            return true;
        }

        public bool SubUserMoney(Tuple<string,int> aux ,double money)
        {
            if (_bankUsers.ContainsKey(aux))
            {
                return _bankUsers[aux].SubMoney(money);
            }

            return false;
        }

        public bool AddUserGold(Tuple<string,int> aux, double gold)
        {
            if (_bankUsers.ContainsKey(aux))
            {
                return _bankUsers[aux].AddGold(gold);
            }

            Exports["ghmattimysql"].execute(
                $"UPDATE bank_users SET gold = gold + ? WHERE identifier=? and name = ? and charidentifier = ?",
                new object[] { gold, aux.Item1, _name,aux.Item2 }
            );

            if (IsUserConnected(aux.Item1))
            {
                BankUser newUser = new BankUser(Name, aux.Item1,aux.Item2, 0.0, gold);
                _bankUsers.Add(aux, newUser);
            }
            return true;
        }

        public bool SubUserGold(Tuple<string,int> aux, double money)
        {
            if (_bankUsers.ContainsKey(aux))
            {
                return _bankUsers[aux].SubGold(money);
            }

            return false;
        }
    }
}
