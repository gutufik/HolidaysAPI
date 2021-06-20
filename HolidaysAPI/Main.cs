using System;
using System.IO;
using Telegram.Bot;
using Newtonsoft.Json;
using System.Net;

namespace TelegramBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramBotClient bot = new TelegramBotClient(File.ReadAllText(@"..\..\..\bot.txt"));

            bot.OnMessage += (s, arg) =>
            {
                if (arg.Message.Text == @"/start")
                {
                    bot.SendTextMessageAsync(arg.Message.Chat.Id, "Введите дату в формате: \"день месяц\"" +
                        "Пример - 6 10 (Шестое октября). Ответом будет ближайший следующий праздник в 2020 году");
                }
                else
                {
                    Console.WriteLine(GetHolidays(arg.Message.Text));
                    bot.SendTextMessageAsync(arg.Message.Chat.Id, GetHolidays(arg.Message.Text));
                }
            };
            bot.StartReceiving();
            Console.ReadKey();
        }
        public static string GetHolidays(string date)
        {
            var ApiKey = File.ReadAllText(@"..\..\..\holidays.txt");
            string country = "RU";
            string year = "2020";
            try
            {
                string month = date.Split(' ')[1];
                string day = date.Split(' ')[0];
                var url = $"https://holidayapi.com/v1/holidays?upcoming&pretty&country={country}&year={year}&key={ApiKey}&language=RU&month={month}&day={day}";
                string holiday = "";
                var request = WebRequest.Create(url);

                var response = request.GetResponse();
                var httpStatusCode = (response as HttpWebResponse).StatusCode;

                if (httpStatusCode != HttpStatusCode.OK)
                {
                    Console.WriteLine(httpStatusCode);
                    return "Error";
                }

                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();
                    var holidays = JsonConvert.DeserializeObject<Root>(result);
                    foreach (Holiday hd in holidays.holidays)
                    {
                        holiday += $"{hd.date} {hd.name}";
                    }
                    return holiday;
                }
            }
            catch 
            {
                return "Неверный формат ввода";   
            }
        }
    }
}