using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    public class Bank:BaseScript
    {

        private Dictionary<string,BankUser> bankUsers = new Dictionary<string, BankUser>();
        private string _name;

        public Bank(string name)
        {
            this._name = name;
        }

        public double GetBankMoney()
        {
            double result = 0.0;
            foreach (KeyValuePair<string,BankUser> user_bank in bankUsers)
            {
                result += user_bank.Value.Money;
            }
            return result;
        }

        public double GetBankGold()
        {
            double result = 0.0;
            foreach (KeyValuePair<string,BankUser> user_bank in bankUsers)
            {
                result += user_bank.Value.Gold;
            }

            return result;
        }

        public void ShowUsers(){
            foreach(KeyValuePair<string,BankUser> user in bankUsers){
                Debug.WriteLine(user.Key);
            }
        }

        public void AddUser(BankUser user)
        {
            if (!bankUsers.ContainsKey(user.Identifier))
            {
                bankUsers.Add(user.Identifier, user);
            }
        }

        public void AddNewUser(string identifier)
        {
            BankUser user = new BankUser(identifier,0.0,0.0,_name,true);
            if (!this.bankUsers.ContainsKey(user.Identifier))
            {
                this.bankUsers.Add(identifier,user);
            }
        }

        public void DeleteUser(string identifier)
        {
            if (bankUsers.ContainsKey(identifier))
            {
                bankUsers.Remove(identifier);
            }
        }

        public BankUser GetUser(string identifier){
            if(bankUsers.ContainsKey(identifier)){
                return bankUsers[identifier];
            }else{
                return null;
            }
        }

        public bool UserExist(string identifier){
            return bankUsers.ContainsKey(identifier);
        }

        public bool AddUserMoney(string identifier,double money){
            if(bankUsers.ContainsKey(identifier)){
                bankUsers[identifier].AddMoney(money);
                return true;
            }else{
                bankUsers.Add(identifier,new BankUser(identifier,0,money,_name,true));
                return true;
            }
        }

        public bool SubUserMoney(string identifier,double money){
            if(bankUsers.ContainsKey(identifier))
            {
                return bankUsers[identifier].SubMoney(money);
            }else return false;
        }

        public bool AddUserGold(string identifier,double gold){
            if(bankUsers.ContainsKey(identifier)){
                bankUsers[identifier].AddGold(gold);
                return true;
            }else{
                bankUsers.Add(identifier,new BankUser(identifier,gold,0,_name,true));
                return true;
            }
        }

        public bool SubUserGold(string identifier,double gold){
            if(bankUsers.ContainsKey(identifier))
            {
                return bankUsers[identifier].SubGold(gold);
            }else return false;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetName(string name)
        {
            _name = name;
        }
    }
}
