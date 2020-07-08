using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
namespace VORP_BankServer
{
    public class Bank:BaseScript
    {

        private Dictionary<string,BankUser> bankUsers = new Dictionary<string, BankUser>();
        private string name;
        private double money;
        private double gold;
      
        public Bank(string name, double money, double gold)
        {
            this.name = name;
            this.money = money;
            this.gold = gold;
        }

        public void showUsers(){
            foreach(KeyValuePair<string,BankUser> user in bankUsers){
                Debug.WriteLine(user.Key);
            }
        }

        public void addUser(BankUser user)
        {
            if (!this.bankUsers.ContainsKey(user.Identifier))
            {
                this.bankUsers.Add(user.Identifier, user);
            }
        }

        public void addNewUser(string identifier)
        {
            BankUser user = new BankUser(identifier,0.0,0.0);
            if (!this.bankUsers.ContainsKey(user.Identifier))
            {
                this.bankUsers.Add(identifier,user);
            }
        }

        public BankUser GetUser(string identifier){
            if(bankUsers.ContainsKey(identifier)){
                return bankUsers[identifier];
            }else{
                return null;
            }
        }

        public bool userExist(string identifier){
            return bankUsers.ContainsKey(identifier);
        }

        public bool addUserMoney(string identifier,double money){
            if(bankUsers.ContainsKey(identifier)){
                double newMoney = bankUsers[identifier].getMoney()+money;
                bankUsers[identifier].setMoney(newMoney);
                this.money+= money;
                return true;
            }else{
                bankUsers.Add(identifier,new BankUser(identifier,0,money));
                return true;
            }
        }

        public bool subUserMoney(string identifier,double money){
            if(bankUsers.ContainsKey(identifier)){
                double nowMoney = bankUsers[identifier].getMoney();
                if(nowMoney-money < 0.0){
                    return false;
                }else{
                    bankUsers[identifier].setMoney(nowMoney-money);
                    this.money-= money;
                    return true;
                }
            }else{
                return false;
            }
        }

        public bool addUserGold(string identifier,double gold){
            if(bankUsers.ContainsKey(identifier)){
                double newGold = bankUsers[identifier].getGold()+gold;
                bankUsers[identifier].setMoney(newGold);
                this.gold+= gold;
                return true;
            }else{
                bankUsers.Add(identifier,new BankUser(identifier,gold,0));
                return true;
            }
        }

        public bool subUserGold(string identifier,double gold){
            if(bankUsers.ContainsKey(identifier)){
                double nowGold = bankUsers[identifier].getGold();
                if(nowGold-gold < 0.0){
                    return false;
                }else{
                    bankUsers[identifier].setMoney(nowGold-gold);
                    this.gold-= gold;
                    return true;
                }
            }else{
                return false;
            }
        }

        public string getName()
        {
            return this.name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public double getMoney()
        {
            return this.money;
        }

        public void setMoney(double money)
        {
            this.money = money;
        }

        public double getGold()
        {
            return this.gold;
        }

        public void setGold(double gold)
        {
            this.gold = gold;
        }
    }
}
