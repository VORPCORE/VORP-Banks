using System;
using CitizenFX.Core;

namespace VORP_BankServer
{
    public class TransferenceC:BaseScript
    {
        private string _fromIdentifier;
        private string _toIdentifier;
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
        

        public TransferenceC(string fromIdentifier, string toIdentifier, double money, double gold,
            string subject, string bankto, string bank, int time,Bank banco)
        {
            FromIdentifier = fromIdentifier;
            ToIdentifier = toIdentifier;
            Money = money;
            Gold = gold;
            Subject = subject;
            BankTo = bankto;
            Bank = bank;
            Time = time;
            Banco = banco;
        }

        public void RestTime()
        {
            Time = Time - 1;
        }

        public void MakeTransference()
        {
            Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ?",
            new object[] {ToIdentifier, Banco.Name},
            new Action<dynamic>((result) =>
            {
                if (result != null)
                {
                    if (result.Count <= 0)
                    {
                        Debug.WriteLine("Entro a registrarlo porque es nuevo");
                        
                        Exports["ghmattimysql"].execute(
                            "INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`) VALUES (?,?,?,?)",
                            new object[] {Bank, ToIdentifier, Money, Gold},
                            new Action<dynamic>((result2) =>
                            {
                                Exports["ghmattimysql"].execute(
                                    "INSERT INTO transactions (`bank`,`fromIdentifier`,`toIdentifier`,`money`,`gold`,`date`,`reason`,`bankto`) VALUES (?,?,?,?,?,CURDATE(),?,?)",
                                    new object[] {Bank, FromIdentifier,ToIdentifier, Money, Gold,Subject,BankTo},
                                    new Action<dynamic>((result4) =>
                                    {
                                    }));
                            }));
                    }
                    else
                    {
                        Exports["ghmattimysql"].execute(
                            "UPDATE bank_users SET money = money+? , gold = gold+?  WHERE `identifier` = ? AND `name` = ?",
                            new object[] {Money,Gold,ToIdentifier, Banco.Name}, new Action<dynamic>((result3) =>
                            {
                                
                            }));
                        Exports["ghmattimysql"].execute(
                            "INSERT INTO transactions (`bank`,`fromIdentifier`,`toIdentifier`,`money`,`gold`,`date`,`reason`,`bankto`) VALUES (?,?,?,?,?,CURDATE(),?,?)",
                            new object[] {Bank, FromIdentifier,ToIdentifier, Money, Gold,Subject,BankTo},
                            new Action<dynamic>((result2) =>
                            {
                            }));
                    }
                }
            }));
        }
        

        // public void MakeTransference(Player playerSend)
        // {
        //     PlayerList playerList = new PlayerList();
        //     Player from = playerSend;
        //     Player to = null;
        //     foreach (Player player in playerList)
        //     {
        //         if ("steam:"+player.Identifiers["steam"] == ToIdentifier) to = player;
        //     }
        //     bool done = false;
        //     bool done2 = true;
        //     if (Money > 0.0)
        //     {
        //         done = Banco.SubUserMoney(FromIdentifier, Money);
        //         if (done)
        //         {
        //             Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ?",
        //                 new object[] {ToIdentifier, Banco.Name},
        //                 new Action<dynamic>((result) =>
        //                 {
        //                     if (result != null)
        //                     {
        //                         if (result.Count <= 0)
        //                         {
        //                             Debug.WriteLine("Entro a registrarlo porque es nuevo");
        //                             Exports["ghmattimysql"].execute(
        //                                 "INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`) VALUES (?,?,?,?)",
        //                                 new object[] {Bank, ToIdentifier, 0.0, 0.0},
        //                                 new Action<dynamic>((result2) =>
        //                                 {
        //                                     if (result2 != null)
        //                                     {
        //                                         Debug.WriteLine("Lo he registrado en bd");
        //                                         done2 =  Banco.AddUserMoney(ToIdentifier, Money);
        //                                     }
        //                                 }));
        //                         }
        //                         else
        //                         {
        //                             done2 =  Banco.AddUserMoney(ToIdentifier,Money);
        //                         }
        //                     }
        //                 }));
        //         }
        //
        //         if (!done2)
        //         {
        //             Banco.AddUserMoney(FromIdentifier, Money);
        //         }
        //     }
        //
        //     done = false;
        //     done2 = true;
        //     if (Gold > 0.0)
        //     {
        //         done = Banco.SubUserGold(FromIdentifier, Gold);
        //         if (done)
        //         {
        //             Exports["ghmattimysql"].execute("SELECT * FROM bank_users WHERE identifier = ? AND `name` = ?",
        //                 new object[] {ToIdentifier, Banco.Name},
        //                 new Action<dynamic>((result) =>
        //                 {
        //                     if (result != null)
        //                     {
        //                         if (result.Count <= 0)
        //                         {
        //                             Debug.WriteLine("Entro a registrarlo porque es nuevo");
        //                             Exports["ghmattimysql"].execute(
        //                                 "INSERT INTO bank_users (`name`,`identifier`,`money`,`gold`) VALUES (?,?,?,?)",
        //                                 new object[] {Banco.Name, ToIdentifier, 0.0, 0.0},
        //                                 new Action<dynamic>((result2) =>
        //                                 {
        //                                     if (result2 != null)
        //                                     {
        //                                         Debug.WriteLine("Lo he registrado en bd");
        //                                         done2 =  Banco.AddUserGold(ToIdentifier, Gold);
        //                                     }
        //                                 }));
        //                         }
        //                         else
        //                         {
        //                             done2 =  Banco.AddUserGold(ToIdentifier, Gold);
        //                         }
        //                     }
        //                 }));
        //         }
        //         if (!done2) { Banco.AddUserGold(FromIdentifier, Gold);}
        //     }
        //
        //     if (from != null)
        //     {
        //         from.TriggerEvent("vorp:refreshBank",Banco.BankUsers["steam:"+from.Identifiers["steam"]].Money,
        //             Banco.BankUsers["steam:"+from.Identifiers["steam"]].Gold);
        //     }
        //
        //     if (to != null)
        //     {
        //         to.TriggerEvent("vorp:refreshBank",Banco.BankUsers["steam:"+to.Identifiers["steam"]].Money,
        //             Banco.BankUsers["steam:"+to.Identifiers["steam"]].Gold);
        //     }
        // }
    }
}