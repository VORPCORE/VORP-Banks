using CitizenFX.Core;

namespace VORP_BankServer
{
    public class BankUser:BaseScript
    {
        private string _bank;
        private string _identifier;
        private double _money;
        private double _gold;

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
                    new object[]{value,_identifier,_bank}
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
                    new object[]{value,_identifier,_bank}
                );
            }
        }

        public BankUser(string bank,string identifier,double money,double gold)
        {
            _bank = bank;
            _identifier = identifier;
            _money = money;
            _gold = gold;
        }
        
        
    }
}