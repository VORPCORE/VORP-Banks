using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Server:BaseScript
    {
        public Server()
        {
            EventHandlers["vorp:bankAddMoney"] += new Action<Player,string,double,string>(addMoney);
            EventHandlers["vorp:bankAddGold"] += new Action<Player,string,double,string>(addGold);
            EventHandlers["vorp:bankSubMoney"] += new Action<Player,string,double,string>(subMoney);
            EventHandlers["vorp:bankSubGold"] += new Action<Player,string,double,string>(subGold);
            EventHandlers["vorp:userTransference"] += new Action<Player,string,double,double,bool,string>(Transference);
            EventHandlers["vorp:registerUserInBank"] += new Action<Player,string>(registerUserInBank);
            Delay(100);
            RegisterEvents();
        }
        //Parte de las transferencias
        private void AddMoneyTransference(string steamId, double money, string usedBank,Player targetPlayer)
        {
            if (Database.Banks.ContainsKey(usedBank))
            {
                if (Database.Banks[usedBank].UserExist(steamId))
                {
                    Database.Banks[usedBank].GetUser(steamId).AddMoney(money);
                    if (targetPlayer != null)
                    {
                        targetPlayer.TriggerEvent("vorp:refreshBank",Database.Banks[usedBank].GetUser(steamId).Money,
                            Database.Banks[usedBank].GetUser(steamId).Gold);
                    }
                }
                else
                {
                    //Hay que crearlo en el banco y darle el dinero
                    Database.Banks[usedBank].AddNewUser(steamId);
                    Database.Banks[usedBank].AddUserMoney(steamId, money);
                    if (targetPlayer != null)
                    {
                        targetPlayer.TriggerEvent("vorp:refreshBank",Database.Banks[usedBank].GetUser(steamId).Money,
                            Database.Banks[usedBank].GetUser(steamId).Gold);
                    }
                }
            }
        }
        private void AddGoldTransference(string steamId, double gold, string usedBank,Player targetPlayer)
        {
            if (Database.Banks.ContainsKey(usedBank))
            {
                if (Database.Banks[usedBank].UserExist(steamId))
                {
                    Database.Banks[usedBank].GetUser(steamId).AddGold(gold);
                    if (targetPlayer != null)
                    {
                        targetPlayer.TriggerEvent("vorp:refreshBank",Database.Banks[usedBank].GetUser(steamId).Money,
                            Database.Banks[usedBank].GetUser(steamId).Gold);
                    }
                }
                else
                {
                    //Hay que crearlo en el banco y darle el dinero
                    Database.Banks[usedBank].AddNewUser(steamId);
                    Database.Banks[usedBank].AddUserGold(steamId, gold);
                    if (targetPlayer != null)
                    {
                        targetPlayer.TriggerEvent("vorp:refreshBank",Database.Banks[usedBank].GetUser(steamId).Money,
                            Database.Banks[usedBank].GetUser(steamId).Gold);
                    }
                }
            }
        }
        
        private void Transference([FromSource]Player source,string steamId,double money,double gold,bool instant,string usedBank)
        {
            //Quitarle el dinero al que envia la transferencia
            //Enviarselo a quien recibe la transferencia
            Debug.WriteLine(source.Handle);
            Debug.WriteLine(steamId);
            Debug.WriteLine(money.ToString());
            Debug.WriteLine(gold.ToString());
            Debug.WriteLine(instant.ToString());
            Debug.WriteLine(usedBank);
            PlayerList pl = new PlayerList();
            Player TargetPlayer = null; //Por si acaso esta mirando el banco que se actualice
            if (source.Identifiers["steam"] == steamId) return;
            foreach (Player target in pl)
            {
                if (target.Identifiers["steam"] == steamId)
                {
                    TargetPlayer = target;
                }
            }
            
            if(money >0)
            {
                if (subMoneyAux(source, usedBank, money, ""))
                {
                    AddMoneyTransference(steamId, money, usedBank, TargetPlayer);
                }
            }
            if (gold > 0)
            {
                if (subGoldAux(source, usedBank, gold, ""))
                {
                    AddGoldTransference(steamId,gold,usedBank,TargetPlayer);
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

        private void registerUserInBank([FromSource]Player source,string bank){
            string identifier = "steam:"+source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].AddNewUser(identifier);
            }
        }
        //Truco de mierda no usar nunca me da hasta repelus el pensar que soy ingeniero y estoy haciendo semejante mierda;
        //Luego me partiré la cabeza para refactorizar y dejarlo todo mejor;
        //todo Refactorizar toda esta chapuza;
        private void subMoney([FromSource] Player source, string bank, double cuantity, string type)
        {
            subMoneyAux(source, bank, cuantity, type);
        }
        private bool subMoneyAux([FromSource] Player source, string bank, double cuantity,string type)
        {
            bool result = false;
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if (Database.Banks[bank].UserExist(identifier)){
                    if (Database.Banks[bank].SubUserMoney(identifier, cuantity))
                    {
                        if (type == "withdraw") TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, cuantity);
                        result = true;
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }else{
                    Database.Banks[bank].AddNewUser(identifier);
                    if (Database.Banks[bank].SubUserMoney(identifier, cuantity))
                    {
                        if (type == "withdraw") TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, cuantity);
                        result = true;
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }  
            }

            return result;
        }
        
        //La misma mierda
        private void subGold([FromSource] Player source, string bank, double cuantity, string type)
        {
            subGoldAux(source, bank, cuantity, type);
        }
        
        private bool subGoldAux([FromSource] Player source, string bank, double cuantity,string type)
        {
            bool result = false;
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if (Database.Banks[bank].UserExist(identifier)){
                    if (Database.Banks[bank].SubUserGold(identifier, cuantity))
                    {
                        if(type =="withdraw")TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1,cuantity);
                        result = true;
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }else{
                    Database.Banks[bank].AddNewUser(identifier);
                    if(Database.Banks[bank].SubUserGold(identifier,cuantity))
                    {
                        if(type =="withdraw")TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1, cuantity);
                        result = true;
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }  
            }

            return result;
        }

        private void addMoney([FromSource]Player source,string bank,double cuantity,string type){
            string identifier = "steam:" + source.Identifiers["steam"];
            TriggerEvent("vorp:getCharacter", int.Parse(source.Handle), new Action<dynamic>((user) =>
            {
                if(user.money >= cuantity)
                {
                    if (Database.Banks.ContainsKey(bank))
                    {
                        if (Database.Banks[bank].UserExist(identifier))
                        {   
                            if (Database.Banks[bank].AddUserMoney(identifier, cuantity))
                            {
                                if (type == "deposit") TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 0, cuantity);
                            }
                            source.TriggerEvent("vorp:refreshBank", Database.Banks[bank].GetUser(identifier).Money,
                                Database.Banks[bank].GetUser(identifier).Gold);
                        }
                        else
                        {
                            Database.Banks[bank].AddNewUser(identifier);
                            if (Database.Banks[bank].AddUserMoney(identifier, cuantity))
                            {
                                if (type == "deposit") TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 0, cuantity);
                            }
                            source.TriggerEvent("vorp:refreshBank", Database.Banks[bank].GetUser(identifier).Money,
                                Database.Banks[bank].GetUser(identifier).Gold);
                        }
                    }
                }
            }));
            
        }

        private void addGold([FromSource]Player source,string bank,double cuantity,string type){
            string identifier = "steam:" + source.Identifiers["steam"];
            TriggerEvent("vorp:getCharacter", int.Parse(source.Handle), new Action<dynamic>((user) =>
            {
                if (user.gold >= cuantity)
                {
                    if (Database.Banks.ContainsKey(bank))
                    {
                        if (Database.Banks[bank].UserExist(identifier))
                        {
                            if(Database.Banks[bank].AddUserGold(identifier, cuantity))
                            {
                                if (type == "deposit") TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 1, cuantity);
                            }
                            source.TriggerEvent("vorp:refreshBank", Database.Banks[bank].GetUser(identifier).Money,
                                Database.Banks[bank].GetUser(identifier).Gold);
                        }
                        else
                        {
                            Database.Banks[bank].AddNewUser(identifier);
                            if (Database.Banks[bank].AddUserGold(identifier, cuantity))
                            {
                                if (type == "deposit") TriggerEvent("vorp:removeMoney", int.Parse(source.Handle), 1, cuantity);
                            }
                            source.TriggerEvent("vorp:refreshBank", Database.Banks[bank].GetUser(identifier).Money,
                                Database.Banks[bank].GetUser(identifier).Gold);
                        }
                    }
                }
            }));
            
        }
    }
}