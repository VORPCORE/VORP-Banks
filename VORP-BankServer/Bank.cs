using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORP_BankServer
{
    public class Bank
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

        public void addUser(BankUser user)
        {
            if (!this.bankUsers.ContainsKey(user.Identifier))
            {
                this.bankUsers.Add(user.Identifier,user);
            }
        }

        public bool addUserMoney(string identifier,double money){
            if(bankUsers.ContainsKey(identifier)){
                double newMoney = bankUsers[identifier].getMoney()+money;
                bankUsers[identifier].setMoney(newMoney);
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
