using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class BankUser
    {
        private string _identifier;
        private double _gold;
        private double _money;

        public BankUser(string identifier,double gold,double money)
        {
            this._gold = gold;
            this._identifier = identifier;
            this._money = money;
        }

        public string Identifier
        {
            get => _identifier;
            set
            {
                if (value != null)
                {
                    _identifier = value;
                }
            }
        }

        public double GetMoney(){
            return this._money;
        }

        public void SetMoney(double money){
            this._money = money;
        }

        public double GetGold(){
            return this._gold;
        }

        public void SetGold(double gold){
            this._gold = gold;
        }
    }
}
