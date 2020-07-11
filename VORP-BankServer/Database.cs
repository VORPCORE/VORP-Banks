using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
/*PROPERTY OF KLC_BY AVILILLA*/
namespace VORP_BankServer
{
    class Database:BaseScript
    {
        public static Dictionary<string,Bank> Banks = new Dictionary<string, Bank>();
        
        //TODO Get all users in each bank and create dictionary bank using database
        public Database(){
            //Create each bank
            LoadDatabase();
        }

        private void LoadDatabase()
        {
            Delay(3000);
            Exports["ghmattimysql"].execute("SELECT * FROM banks", new Action<dynamic>((result) =>{
                if(result != null){
                    foreach(var bank in result){
                        Banks.Add(bank.name.ToString(),new Bank(bank.name.ToString()));
                        Debug.WriteLine($"Added :{bank.name} with {bank.money} money and {bank.gold} gold");
                    }
                    if (Banks.Count > 0)
                    {
                        Delay(500);
                        Exports["ghmattimysql"].execute("SELECT * FROM bank_users", new Action<dynamic>((aresult) => {
                            if (aresult != null)
                            {
                                foreach (dynamic user in aresult)
                                {
                                    if (user != null)
                                    {
                                        if (Banks.ContainsKey(user.name.ToString()))
                                        {
                                            Bank aux = Banks[user.name.ToString()];
                                            aux.AddUser(new BankUser(user.identifier.ToString(),double.Parse(user.gold.ToString()), double.Parse(user.money.ToString()),aux.GetName()));
                                        }
                                    }
                                }
                            }
                        }));
                    }
                }
            }));
        }
    }
}
