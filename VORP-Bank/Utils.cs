using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace VORP_Bank
{
    public class Utils:BaseScript
    {
        //Posicion 
        public static Dictionary<string,Vector3> bankPositions= new Dictionary<string,Vector3>()
        {
            {"valentine",new Vector3(20.0f,20.3f,40.3f)}
        };

        public Utils(){}
    }
}
