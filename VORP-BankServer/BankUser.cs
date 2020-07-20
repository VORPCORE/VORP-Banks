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
        private List<Transaction> _transactions;

        public BankUser(string identifier,double gold,double money,string bank,bool newWith)
        {
            _gold = gold;
            _identifier = identifier;
            _money = money;
            _bank = bank;
            if (newWith)
            {
                Exports["ghmattimysql"]
                    .execute(
                        $"INSERT INTO bank_users (name,identifier,money,gold) VALUES (?,?,?,?)",new object[] {_bank,_identifier,_money,_gold});
            }
            _transactions = new List<Transaction>();
        }

        public List<Transaction> Transactions
        {
            get => _transactions;
            set => _transactions = value;
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
            set { _gold = value;}
        }

        public double Money
        {
            get => _money;
            set { _money = value; }
        }

        public void AddMoney(double money)
        {
            if (money >= 0)
            {
                _money += money;
                UpdateUser(money,"money","add");
            }
        }

        public void AddGold(double gold)
        {
            if (gold >= 0)
            {
                _gold += gold;
                UpdateUser(gold,"gold","add");
            }
        }

        public bool SubMoney(double money)
        {
            if (money >= 0)
            {
                if (_money >= money)
                {
                    _money -= money;
                    UpdateUser(money,"money","sub");
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
                if (_gold >= gold)
                {
                    _gold -= gold;
                    UpdateUser(gold,"gold","sub");
                    return true;
                }
                else return false;
            }
            else return false;
        }

        public void UpdateUser(double cuantity,string type,string type2)
        {
            switch (type)
            {
                case "money":
                    switch (type2)
                    {
                        case "add":
                            Exports["ghmattimysql"]
                                .execute(
                                    $"UPDATE bank_users SET money = money+'{cuantity}' WHERE identifier=? and name =?;",
                                    new[] { _identifier,_bank});
                            break;
                        case "sub":
                            Exports["ghmattimysql"]
                                .execute(
                                    $"UPDATE bank_users SET money = money-'{cuantity}' WHERE identifier=? and name =?;",
                                    new[] { _identifier,_bank});
                            break;
                    }
                    break;
                case "gold":
                    switch (type2)
                    {
                        case "add":
                            Exports["ghmattimysql"]
                                .execute(
                                    $"UPDATE bank_users SET gold = gold+'{cuantity}' WHERE identifier=? and name =?;",
                                    new[] { _identifier,_bank});
                            break;
                        case "sub":
                            Exports["ghmattimysql"]
                                .execute(
                                    $"UPDATE bank_users SET gold = gold-'{cuantity}' WHERE identifier=? and name =?;",
                                    new[] { _identifier,_bank});
                            break;
                    } 
                    break;
                default:
                    Debug.WriteLine("Error al intentar setear en base de datos");
                    break;
            }
        }
    }
}
