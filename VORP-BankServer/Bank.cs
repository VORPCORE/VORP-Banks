using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORP_BankServer
{
    public class Bank
    {
        private string name;
        private double money;
        private double gold;
        public Bank(string name, double money, double gold)
        {
            this.name = name;
            this.money = money;
            this.gold = gold;
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
    }
}
