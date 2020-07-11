using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class BankUser:BaseScript
    {
        private string _identifier;
        private double _gold;
        private double _money;
        private string _bank;

        public BankUser(string identifier,double gold,double money,string bank)
        {
            _gold = gold;
            _identifier = identifier;
            _money = money;
            _bank = bank;
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
        
        public double Gold
        {
            get => _gold;
            set { _gold = value; UpdateUser();}
        }

        public double Money
        {
            get => _money;
            set { _money = value; UpdateUser(); }
        }

        public void AddMoney(double money)
        {
            if (money >= 0)
            {
                _money += money;
                UpdateUser();
            }
        }

        public void AddGold(double gold)
        {
            if (gold >= 0)
            {
                _gold = gold;
                UpdateUser();
            }
        }

        public bool SubMoney(double money)
        {
            if (money >= 0)
            {
                if (_money >= (_money - money))
                {
                    _money -= money;
                    UpdateUser();
                    return true;
                }
                else return false;
            }
            else return false;
        }
        
        public bool SubGold(double gold)
        {
            if (gold >= 0)
            {
                if (_gold >= (_gold - gold))
                {
                    _gold -= gold;
                    UpdateUser();
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public void UpdateUser()
        {
            Exports["ghmattimysql"]
                .execute(
                    $"UPDATE bank_users SET money = '{_money}', gold = {_gold} WHERE identifier=? and name =?",
                    new[] { _identifier,_bank});
        }
    }
}
