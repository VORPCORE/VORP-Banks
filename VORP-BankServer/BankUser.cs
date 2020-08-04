using CitizenFX.Core;
using System.Collections.Generic;

namespace VORP_BankServer
{
    public class BankUser : BaseScript
    {
        private string _bank;
        private string _identifier;
        private double _money;
        private double _gold;

        private List<Transference> _transferences = new List<Transference>();
        public struct Transference
        {
            public string date;
            public double money;
            public string subject;
            public string operation;
            public double gold;
        }

        public List<Transference> Transferences
        {
            get => _transferences;
            set => _transferences = value;
        }

        public string Bank
        {
            get => _bank;
            set => _bank = value;
        }

        public string Identifier
        {
            get => _identifier;
            set => _identifier = value;
        }

        public double Money
        {
            get => _money;
            set
            {
                _money = value;
                Exports["ghmattimysql"].execute(
                    $"UPDATE bank_users SET money = ? WHERE identifier=? and name =?;",
                    new object[] { value, _identifier, _bank }
                );
            }
        }

        public double Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                Exports["ghmattimysql"].execute(
                    $"UPDATE bank_users SET gold = ? WHERE identifier=? and name =?;",
                    new object[] { value, _identifier, _bank }
                );
            }
        }

        public BankUser(string bank, string identifier, double money, double gold)
        {
            _bank = bank;
            _identifier = identifier;
            _money = money;
            _gold = gold;
        }

        public bool AddMoney(double money)
        {
            double newMoney = money + _money;
            Money = newMoney;
            if (Money == newMoney)
            {
                return true;
            }

            return false;
        }

        public bool AddGold(double gold)
        {
            double newGold = gold + _gold;
            Gold = newGold;
            if (Gold == newGold)
            {
                return true;
            }

            return false;
        }

        public bool SubMoney(double money)
        {
            double newMoney = _money - money;
            if (newMoney >= 0)
            {
                Money = newMoney;
                return true;
            }

            return false;
        }

        public bool SubGold(double gold)
        {
            double newGold = _gold - gold;
            if (newGold >= 0)
            {
                Gold = newGold;
                return true;
            }

            return false;

        }

    }
}