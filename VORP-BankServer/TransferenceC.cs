using CitizenFX.Core;
using System;
using Newtonsoft.Json;
namespace VORP_BankServer
{
    public class TransferenceC : BaseScript
    {
        private string _fromIdentifier;
        private string _toIdentifier;
        private int _fromCharId;
        private int _toCharId;
        private double _money;
        private double _gold;
        private string _subject;
        private string _bankTo;
        private string _bank;
        private int _time;
        private Bank _banco;

        public Bank Banco
        {
            get => _banco;
            set => _banco = value;
        }

        public string FromIdentifier
        {
            get => _fromIdentifier;
            set => _fromIdentifier = value;
        }

        public int Time
        {
            get => _time;
            set => _time = value;
        }

        public string BankTo
        {
            get => _bankTo;
            set => _bankTo = value;
        }

        public string Bank
        {
            get => _bank;
            set => _bank = value;
        }

        public string Subject
        {
            get => _subject;
            set => _subject = value;
        }

        public double Gold
        {
            get => _gold;
            set => _gold = value;
        }

        public double Money
        {
            get => _money;
            set => _money = value;
        }

        public string ToIdentifier
        {
            get => _toIdentifier;
            set => _toIdentifier = value;
        }

        public int ToCharId
        {
            get => _toCharId;
            set => _toCharId = value;
        }

        public int FromCharId
        {
            get => _fromCharId;
            set => _fromCharId = value;
        }

        public TransferenceC(string fromIdentifier, int fromCharId, string toIdentifier, int toCharId, double money, double gold,
            string subject, string bankto, string bank, int time, Bank banco)
        {
            FromIdentifier = fromIdentifier;
            FromCharId = fromCharId;
            ToIdentifier = toIdentifier;
            ToCharId = toCharId;
            Money = money;
            Gold = gold;
            Subject = subject;
            BankTo = bankto;
            Bank = bank;
            Time = time;
            Banco = banco;
        }

        public int GetCharacterId(Player source)
        {
            dynamic UserCharacter = Server.CORE.getUser(int.Parse(source.Handle)).getUsedCharacter;
            return UserCharacter.charIdentifier;
        }

        public void RestTime()
        {
            Time = Time - 1;
        }

        private async void RefreshClientTransactions(){
            PlayerList pl = new PlayerList();
            Player player1 = null;
            Player player2 = null;
            int playerIndex = 1;
            foreach(Player player in pl){
                if("steam:"+player.Identifiers["steam"] == FromIdentifier || "steam:"+player.Identifiers["steam"] == ToIdentifier){
                    switch(playerIndex){
                        case 1:
                            player1 = player;
                        break;
                        case 2:
                            player2 = player;
                        break;
                    }
                }
            }
            if(player1 != null){
                //Aqui se hace la busqueda en base de datos para enviarsela
                string identifier = "steam:" + player1.Identifiers["steam"];
                
                dynamic result = await Exports["ghmattimysql"].executeSync("SELECT DATE_FORMAT(DATE, '%W %M %e %Y'),money,gold,reason,toIdentifier FROM transactions WHERE (fromIdentifier = ? OR toIdentifier = ?) AND (fromcharid = ? OR tocharid = ?)",
                    new object[] { identifier, identifier, GetCharacterId(player1), GetCharacterId(player1) });
                string str = JsonConvert.SerializeObject(result);
                player1.TriggerEvent("vorp:refreshTransactions",str,identifier);
            }
            if(player2 != null){
                string identifier = "steam:" + player2.Identifiers["steam"];
                dynamic result = await Exports["ghmattimysql"].executeSync("SELECT DATE_FORMAT(DATE, '%W %M %e %Y'),money,gold,reason,toIdentifier FROM transactions WHERE (fromIdentifier = ? OR toIdentifier = ?) AND (fromcharid = ? OR tocharid = ?)",
                    new object[] { identifier, identifier, GetCharacterId(player2), GetCharacterId(player2) });
                string str = JsonConvert.SerializeObject(result);
                player2.TriggerEvent("vorp:refreshTransactions",str,identifier);
            }
        }

        public async void MakeTransference()
        {
            Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ? AND charidentifier = ? AND `name` = ?",
            new object[] { ToIdentifier, Banco.Name, ToCharId },
            new Action<dynamic>(async(result) =>
            {
                if (result != null)
                {
                    if (result.Count <= 0)
                    {

                        await Exports["ghmattimysql"].executeSync(
                            "INSERT INTO bank_users (`name`,`identifier`,`charidentifier`,`money`,`gold`) VALUES (?,?,?,?,?)",
                            new object[] { Bank, ToIdentifier, ToCharId, Money, Gold });
                            
                        await Exports["ghmattimysql"].executeSync(
                            "INSERT INTO transactions (`bank`,`fromIdentifier`,`fromcharid`,`toIdentifier`,`tocharid`,`money`,`gold`,`date`,`reason`,`bankto`) VALUES (?,?,?,?,?,?,?,CURDATE(),?,?)",
                            new object[] { Bank, FromIdentifier, FromCharId, ToIdentifier, ToCharId, Money, Gold, Subject, BankTo });
                           
                    }
                    else
                    {
                        await Exports["ghmattimysql"].executeSync(
                            "UPDATE bank_users SET money = money+? , gold = gold+?  WHERE `identifier` = ? AND `charidentifier` = ? AND `name` = ?",
                            new object[] { Money, Gold, ToIdentifier, ToCharId, Banco.Name });
                        await Exports["ghmattimysql"].executeSync(
                            "INSERT INTO transactions (`bank`,`fromIdentifier`,`fromcharid`,`toIdentifier`,`tocharid`,`money`,`gold`,`date`,`reason`,`bankto`) VALUES (?,?,?,?,?,?,?,CURDATE(),?,?)",
                            new object[] { Bank, FromIdentifier, FromCharId, ToIdentifier, ToCharId, Money, Gold, Subject, BankTo });
                    }
                    RefreshClientTransactions();
                }
            }));
        }
    }
}