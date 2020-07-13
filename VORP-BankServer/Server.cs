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
            EventHandlers["vorp:registerUserInBank"] += new Action<Player,string>(registerUserInBank);
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
        }

        private void registerUserInBank([FromSource]Player source,string bank){
            string identifier = "steam:"+source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].AddNewUser(identifier);
            }
        }

        private void subMoney([FromSource] Player source, string bank, double cuantity,string type)
        {
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if (Database.Banks[bank].UserExist(identifier)){
                    if (Database.Banks[bank].SubUserMoney(identifier, cuantity))
                    {
                        if (type == "withdraw") TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, cuantity);
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }else{
                    Database.Banks[bank].AddNewUser(identifier);
                    if (Database.Banks[bank].SubUserMoney(identifier, cuantity))
                    {
                        if (type == "withdraw") TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 0, cuantity);
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }  
            }
        }
        
        private void subGold([FromSource] Player source, string bank, double cuantity,string type)
        {
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if (Database.Banks[bank].UserExist(identifier)){
                    if (Database.Banks[bank].SubUserGold(identifier, cuantity))
                    {
                        if(type =="withdraw")TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1,cuantity);
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }else{
                    Database.Banks[bank].AddNewUser(identifier);
                    if(Database.Banks[bank].SubUserGold(identifier,cuantity))
                    {
                        if(type =="withdraw")TriggerEvent("vorp:addMoney", int.Parse(source.Handle), 1, cuantity);
                    }
                    source.TriggerEvent("vorp:refreshBank",Database.Banks[bank].GetUser(identifier).Money,
                        Database.Banks[bank].GetUser(identifier).Gold);
                }  
            }
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