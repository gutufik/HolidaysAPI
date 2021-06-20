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
                    bot.SendTextMessageAsync(arg.Message.Chat.Id, "Start");
                }
                else
                {
                    Console.WriteLine(GetHolidays());
                    bot.SendTextMessageAsync(arg.Message.Chat.Id, GetHolidays());
                }
            };
            bot.StartReceiving();
            Console.ReadKey();
        }
        public static string GetHolidays()
        {
            var ApiKey = File.ReadAllText(@"..\..\..\holidays.txt");
            string country = "RU";
            string year = "2020";

            var url = $"https://holidayapi.com/v1/holidays?upcoming&pretty&country={country}&year={year}&key={ApiKey}&language=RU&month=6&day=20";
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
                    holiday += $"{hd.date} {hd.name}\n";
                }
                return holiday;
            }
        }
    }
}