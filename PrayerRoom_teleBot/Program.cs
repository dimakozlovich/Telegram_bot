using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;
using TimePeriodsLibary;

namespace PrayerRoom_teleBot
{
    class Program
    {

        private static string token = "5772565402:AAFK8DvPc65N60qVOiEvVpSW0dF_hBq3SV8";
        private static TelegramBotClient client;
        public static ListOfPeriod avaliable_Times = new ListOfPeriod();
        public static ListOfPeriod times = new ListOfPeriod();
        public static List<User> users = new List<User>();

        [Obsolete]
        static void Main(string[] args)
        {
            
            client = new TelegramBotClient(token);
            Settings();
            client.StartReceiving();
            client.OnMessage += On_bot_message;
            Console.ReadLine();
            client.StopReceiving();

        }

        [Obsolete]
        public static async void On_bot_message(object sender, MessageEventArgs e)
        {
            string message_1 = $"Привет,выбирай время для твоей молитвы{char.ConvertFromUtf32(0x1F64F)}";
            string message_2 = "";// TODO: text concelation
            string message_3 = "Регистрация прошла успешно, твоё время для молитвы :";
            string message_4 = "Регистрация успешно отменена";
            var msg = e.Message;
            Console.WriteLine(msg.Text+" "+msg.Chat.Id+" " + msg.Chat.FirstName +" "+ msg.Chat.LastName);

            if (Check_Message_Cancel(msg.Text) || times.listOfperiod.Count > 0)
            {
                switch (msg.Text)
                {
                
                case "/start":
                    if (checkUsers(msg.Chat.Id)) {
                        try {
                            await client.SendTextMessageAsync(msg.Chat.Id, message_2, replyMarkup: ButtonDell(findUser(msg.Chat.Id)));
                        }
                        catch
                        {

                        }
                    }
                    else
                    {

                        try
                        {
                            await client.SendTextMessageAsync(msg.Chat.Id, message_1, replyMarkup: GetButtons());
                        }
                        catch
                        {

                        }
                    }
                    break;
                case "Записаться Ещё":
                    await client.SendTextMessageAsync(msg.Chat.Id, message_1, replyMarkup: GetButtons());
                    break;
                    default:
                  
                    //Record        
                    for (int i = 0; i < times.listOfperiod.Count; i++)
                    {
                        var time = times.listOfperiod[i];
                        if (time.ToString() == msg.Text)
                        {
                            if (checkUsers(msg.Chat.Id))
                            {
                                var user = findUser(msg.Chat.Id);
                                user.addTime(time);
                                times.DellFromList(time);
                            }
                            else
                            {
                                var user = new User(msg.Chat.Id, time);
                                users.Add(user);
                                times.DellFromList(time);
                            }
                            var usersec = findUser(msg.Chat.Id);
                            string timeofUsersec = "";
                            foreach (var tmeUser in usersec.Times)
                            {
                                timeofUsersec += $"{Convert.ToString(tmeUser)} {char.ConvertFromUtf32(0x2757)}";
                            }
                            await client.SendTextMessageAsync(msg.Chat.Id, message_3 + timeofUsersec, replyMarkup: ButtonDell(findUser(msg.Chat.Id)));
                            try
                            {
                                await client.SendTextMessageAsync(-1001522555182, msg.Chat.FirstName + msg.Chat.LastName + " @" + msg.Chat.Username + "-" + timeofUsersec);
                            }
                            catch
                            {

                            }
                        }

                    }

                    //TODO: check for occupied place
                    // cancel
                    if (Check_Message_Cancel(msg.Text))
                    {
                            var dellTime = Check_Message_time(msg.Text);
                        if (Check_Message_time(msg.Text) != null&&CheckTime(findUser(msg.Chat.Id),dellTime))
                        {

                            try
                            {
                                bool costil = findUser(msg.Chat.Id).Times.Count > 1;
                                if (findUser(msg.Chat.Id).Times.Count > 1)
                                {
                                    findUser(msg.Chat.Id).DellTime(dellTime);
                                    times.AddInList(dellTime);
                                    await client.SendTextMessageAsync(msg.Chat.Id, message_4, replyMarkup: ButtonDell(findUser(msg.Chat.Id)));
                                }

                                else
                                {
                                    times.AddInList(dellTime);
                                        try
                                        {
                                            users.Remove(findUser(msg.Chat.Id));
                                        }
                                        catch
                                        {

                                        }
                                    await client.SendTextMessageAsync(msg.Chat.Id, message_4, replyMarkup: StartBot());
                                }
                                try
                                {
                                     await client.SendTextMessageAsync(-1001522555182, msg.Chat.FirstName + msg.Chat.LastName + " @" + msg.Chat.Username + " отменил(a) запись  "); 
                                }
                                catch { }
                            }
                            catch
                            {
                              //      await client.SendTextMessageAsync(-1001522555182, msg.Chat.FirstName + msg.Chat.LastName + " @" + msg.Chat.Username + " Exeption");
                            }

                        }


                    }
                    break;



                }
            }
            else
            {
                await client.SendTextMessageAsync(msg.Chat.Id, "К сожалению свободных мест нет, ждем Тебя в следующий раз");
            }


        }
     
        #region settings
        public static void Settings()
        {
            FillTimes(times);
            FillTimes(avaliable_Times);
        }
        public static bool CheckTime(User usercheck, TimePeriod delltime)
        {
            try
            {
                foreach (var objec in usercheck.Times)
                {
                    if (Convert.ToString(objec) == Convert.ToString(delltime))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public static void FillTimes(ListOfPeriod obj)
        {
            var begDataTimecos = new DateTime(2022, 11, 18, 22, 0, 0);
            var endDatetimecos = new DateTime(2022, 11, 18, 23, 0, 0);
            obj.FillList(begDataTimecos, endDatetimecos);
            var begDataTime = new DateTime(2022, 11, 19, 0, 0, 0);
            var endDatetime = new DateTime(2022, 11, 19, 19, 0, 0);
            obj.FillList(begDataTime, endDatetime);
        }
        #endregion
        #region Users
        public static User findUser(long id)
        {
            foreach (var user in users)
            {
                if (user.Id == id)
                {
                    return user;
                }
            }
            return null;
        }
        public static bool checkUsers(long id)
        {
           foreach(var user in users)
            {
                if (user.Id == id)
                {
                    return true;
                }
            }
            return false;
            //TODO
        }
        #endregion 
        public static bool Check_Message_Cancel(string msg)
        {
            var msgToCharrArray = msg.ToCharArray();
            string word_cancel = "";
            for(int i = 0; i < 8; i++)
            {
                try { word_cancel += Convert.ToString(msgToCharrArray[i]); }
                catch
                {

                }
            }
            if(word_cancel == "Отменить")
            {
                return true;
            }
            else
            {
                return false;
            }
          
        }
     
        public static TimePeriod Check_Message_time(string msg)
        {
            var msgToCharrArray = msg.ToCharArray();
            string period = "";
            for (int i = 10; i < msgToCharrArray.Length; i++)
            {
                period += Convert.ToString(msgToCharrArray[i]);
            }
            foreach(var avaTimes in avaliable_Times.listOfperiod)
            {
                string ToStringAva = avaTimes.ToString();
                if (ToStringAva == period)
                {
                    return avaTimes;
                }
            }
            return null;
            // TODO: VERIFITY
        }
        #region keyboards
        private static IReplyMarkup GetButtons()
        {
            List<string> timesToStrings = new List<string>();
            foreach(var time in times.listOfperiod)
            {
                timesToStrings.Add(time.ToString());
            }
            
            List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>();
            
            for (int i = 0; i <= timesToStrings.Count - 1; i++)
            {
                List<KeyboardButton> butt = new List<KeyboardButton>();
                butt.Add(new KeyboardButton { Text = timesToStrings[i] });
                keyboardButtons.Add(butt);
            }
            return new ReplyKeyboardMarkup
            {
                Keyboard = keyboardButtons
            };

        }
        private static IReplyMarkup ButtonDell(User user)
        {
            List<string> timeOfUser = new List<string>();
            foreach (var periods in user.Times)
            {
                timeOfUser.Add(periods.ToString());
            }

            List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>();
            for (int i = 0; i <= timeOfUser.Count - 1; i++)
            {
                List<KeyboardButton> butt = new List<KeyboardButton>();
                butt.Add(new KeyboardButton { Text ="Отменить: "+timeOfUser[i] });
                if (i == 0)
                {
                    butt.Add(new KeyboardButton { Text = "Записаться Ещё" });
                }
                
                keyboardButtons.Add(butt);
            }
            return new ReplyKeyboardMarkup { Keyboard = keyboardButtons};

        }
        private static IReplyMarkup StartBot()
        {
            List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>();
           
                List<KeyboardButton> butt = new List<KeyboardButton>();
                butt.Add(new KeyboardButton { Text = "/start" });
                keyboardButtons.Add(butt);
          
            return new ReplyKeyboardMarkup { Keyboard = keyboardButtons };
        }
        #endregion
    }
}
