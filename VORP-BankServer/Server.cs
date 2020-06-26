using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace VORP_BankServer
{
    public class Main:BaseScript
    {
        public Main()
        {
            EventHandlers["vorp:addMoney"] += new Action<Player,string,double>(addMoney);
            EventHandlers["vorp:subMoney"] += new Action<Player,string,double>(subMoney);
            EventHandlers["vorp:addGold"] += new Action<Player,string,double>(addGold);
            EventHandlers["vorp:subGold"] += new Action<Player,string,double>(subGold);
            EventHandlers["vorp:registerUserInBank"] += new Action<Player,string>(registerUserInBank);
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
                Database.Banks[bank].addUserMoney(identifier,cuantity);
            }
        }

        private void subMoney([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].subUserMoney(identifier,cuantity);
            }
        }

        private void addGold([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].addUserGold(identifier,cuantity);
            }
        }

        private void subGold([FromSource]Player source,string bank,double cuantity){
            string identifier = "steam:" + source.Identifiers["steam"];
            if(Database.Banks.ContainsKey(bank)){
                Database.Banks[bank].subUserGold(identifier,cuantity);
            }
        }
    }
}