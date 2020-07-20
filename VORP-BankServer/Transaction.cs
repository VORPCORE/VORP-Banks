using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VORP_BankServer
{
    public class Transaction
    {
        private string _bank;
        private string _toIdentifier;
        private string _fromIdentifier;
        private double _money;
        private double _gold;
        private string _date;
        private string _reason;
        private string _bankTo;
        private string _toName;
        private string _fromName;

        public Transaction(string bank,string toIdentifier,string fromIdentifier,double money, double gold,string date,string reason,string bankTo,string toName,string fromName)
        {
            _bank = bank;
            _toIdentifier = toIdentifier;
            _fromIdentifier = fromIdentifier;
            _money = money;
            _gold = gold;
            _date = date;
            _reason = reason;
            _bankTo = bankTo;
            _toName = toName;
            _fromName = fromName;
        }

        public string Bank
        {
            get => _bank;
            set => _bank = value;
        }

        public string ToIdentifier
        {
            get => _toIdentifier;
            set => _toIdentifier = value;
        }

        public string FromIdentifier
        {
            get => _fromIdentifier;
            set => _fromIdentifier = value;
        }

        public double Money
        {
            get => _money;
            set => _money = value;
        }

        public double Gold
        {
            get => _gold;
            set => _gold = value;
        }

        public string Date
        {
            get => _date;
            set => _date = value;
        }

        public string Reason
        {
            get => _reason;
            set => _reason = value;
        }

        public string BankTo
        {
            get => _bankTo;
            set => _bankTo = value;
        }

        public string ToName
        {
            get => _toName;
            set => _toName = value;
        }

        public string FromName
        {
            get => _fromName;
            set => _fromName = value;
        }
    }
}
