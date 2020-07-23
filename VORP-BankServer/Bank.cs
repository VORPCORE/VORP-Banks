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
        private string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }
        

        public Bank(string name)
        {
            this._name = name;
        }
    }
}
