using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VORP_BankServer
{
    public class BankUser
    {
        private string identifier;
        private double gold;
        private double money;

        public BankUser(string identifier,double gold,double money)
        {
            this.gold = gold;
            this.identifier = identifier;
            this.money = money;
        }

        public string Identifier
        {
            get => identifier;
            set
            {
                if (value != null)
                {
                    identifier = value;
                }
            }
        }

        public double getMoney(){
            return this.money;
        }

        public void setMoney(double money){
            this.money = money;
        }

        public double getGold(){
            return this.gold;
        }

        public void setGold(double gold){
            this.gold = gold;
        }
    }
}
