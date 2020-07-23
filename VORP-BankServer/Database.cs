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

        private async void LoadDatabase()
        {
            await Delay(2000);
            Exports["ghmattimysql"].execute("SELECT * FROM banks", new Action<dynamic>((result) =>{
                Debug.WriteLine(result.Count.ToString());
                if(result != null){
                    foreach (var bank in result)
                    {
                        Banks.Add(bank.name.ToString(), new Bank(bank.name.ToString()));
                        Debug.WriteLine($"Added :{bank.name} with {bank.money} money and {bank.gold} gold");
                    }
                }
            }));
        }
    }
}
