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
            EventHandlers["vorp:subMoney"] += new Action<Player,string,double>(subMoney);
            EventHandlers["vorp:addGold"] += new Action<Player,string,double>(addGold);
            EventHandlers["vorp:subGold"] += new Action<Player,string,double>(subGold);
            EventHandlers["vorp:registerUserInBank"] += new Action<Player,string>(registerUserInBank);
            EventHandlers["vorp:retrieveUserInfo"] += new Action<Player>(retrieveUserInfo);
        }

        //Only register when insert money in order to
        private void retrieveUserInfo([FromSource]Player source){
            string identifier = "steam:"+source.Identifiers["steam"];
            Dictionary<string,Dictionary<string,double>> userInfo = new Dictionary<string,Dictionary<string,double>>();
            foreach (KeyValuePair<string,Bank> bank in Database.Banks)
            {
                if (bank.Value.userExist(identifier))
                {
                    userInfo.Add(bank.Key,new Dictionary<string, double>
                    {
                        {"money",bank.Value.GetUser(identifier).getMoney()},
                        {"gold",bank.Value.GetUser(identifier).getGold()}
                    });
                }
            }

            source.TriggerEvent("vorp:giveUserInfo",userInfo);
        }

        private void registerUserInBank([FromSource]Player source,string bank){
            string identifier = "steam:"+source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].addUser(identifier);
            }
        }

        private void addMoney([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if(Database.Banks[bank].userExist(identifier)){
                    Database.Banks[bank].addUserMoney(identifier,cuantity);
                }else{
                    Database.Banks[bank].addUser(identifier);
                    Database.Banks[bank].addUserMoney(identifier,cuantity);
                }  
            }
        }

        private void subMoney([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if(Database.Banks[bank].userExist(identifier))
                    Database.Banks[bank].subUserMoney(identifier,cuantity);
            }
        }

        private void addGold([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if(Database.Banks[bank].userExist(identifier)){
                    Database.Banks[bank].addUserGold(identifier,cuantity);
                }else{
                    Database.Banks[bank].addUser(identifier);
                    Database.Banks[bank].addUserGold(identifier,cuantity);
                }  
            }
        }

        private void subGold([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                if(Database.Banks[bank].userExist(identifier))
                    Database.Banks[bank].subUserGold(identifier,cuantity);
            }
        }
    }
}