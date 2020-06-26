using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace VORP_BankServer
{
    class Database:BaseScript
    {
        public static Dictionary<string,Bank> Banks = new Dictionary<string, Bank>();
        
        //TODO Get all users in each bank and create dictionary bank using database
        public Database(){
            //Create each bank
            Exports["ghmattimysql"].execute("SELECT * FROM banks", new Action<dynamic>((result) =>{
                if(result != null){
                    foreach(var bank in result){
                        Banks.Add(bank.name,new Bank(bank.name,bank.money,bank.gold));
                    }
                }
            }));

            if(Banks.Count > 0){
                Exports["ghmattimysql"].execute("SELECT * FROM bank_users", new Action<dynamic>((result) =>{
                if(result != null){
                    foreach(var user in result){
                        if (Banks.ContainsKey(user["name"]))
                        {
                            Bank aux = Banks[user["name"]];
                            aux.addUser(new BankUser(user.identifier,double.Parse(user.gold), double.Parse(user.money)));
                        }
                    }
                }
                }));
            }

        }

    }
}
