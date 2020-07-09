using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace VORP_BankServer
{
    public class Server:BaseScript
    {
        public Server()
        {
            EventHandlers["vorp:addMoney"] += new Action<Player,string,double>(addMoney);
            EventHandlers["vorp:addGold"] += new Action<Player,string,double>(addGold);
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
                        userCallback.Add("money",Database.Banks[args].GetUser(identifier).GetMoney());
                        userCallback.Add("gold",Database.Banks[args].GetUser(identifier).GetGold());
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

            Delay(100);

            TriggerEvent("vorp:addNewCallBack", "bankSubMoney", new Action<int, CallbackDelegate, dynamic>((source, cb, args) => {
                PlayerList pl = new PlayerList();
                Player p = pl[source];
                string identifier = "steam:" + p.Identifiers["steam"];
                if (Database.Banks.ContainsKey(args.bank))
                {
                    if (Database.Banks[args.bank].UserExist(identifier))
                    {
                        cb(Database.Banks[args.bank].SubUserMoney(identifier, args.cuantity));
                    }
                    cb(false);

                }
                else
                {
                    cb(false);
                }
            }));

            Delay(100);

            TriggerEvent("vorp:addNewCallBack", "bankSubGold", new Action<int, CallbackDelegate, dynamic>((source, cb, args) => {
                PlayerList pl = new PlayerList();
                Player p = pl[source];
                string identifier = "steam:" + p.Identifiers["steam"];
                if (Database.Banks.ContainsKey(args.bank))
                {
                    if (Database.Banks[args.bank].UserExist(identifier))
                    {
                        cb(Database.Banks[args.bank].SubUserGold(identifier, args.cuantity));
                    }
                    cb(false);

                }
                else
                {
                    cb(false);
                }
            }));
        }

        private void registerUserInBank([FromSource]Player source,string bank){
            string identifier = "steam:"+source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].AddNewUser(identifier);
            }
        }

        private void addMoney([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if(Database.Banks[bank].UserExist(identifier)){
                    Database.Banks[bank].AddUserMoney(identifier,cuantity);
                }else{
                    Database.Banks[bank].AddNewUser(identifier);
                    Database.Banks[bank].AddUserMoney(identifier,cuantity);
                }  
            }
        }

        private void addGold([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if(Database.Banks[bank].UserExist(identifier)){
                    Database.Banks[bank].AddUserGold(identifier,cuantity);
                }else{
                    Database.Banks[bank].AddNewUser(identifier);
                    Database.Banks[bank].AddUserGold(identifier,cuantity);
                }  
            }
        }
    }
}