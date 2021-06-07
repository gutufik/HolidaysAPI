using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace HolidaysAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            var ApiKey = "";
            string country = "RU";
            string year = "2020";
            
            var url = $"https://holidayapi.com/v1/holidays?pretty&country={country}&year={year}&key={ApiKey}";

            var request = WebRequest.Create(url);

            var response = request.GetResponse();
            var httpStatusCode = (response as HttpWebResponse).StatusCode;

            if (httpStatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(httpStatusCode);
                return;
            }

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                var holidays = JsonConvert.DeserializeObject<Root>(result);
                foreach (Holiday hd in holidays.holidays)
                {
                    Console.WriteLine($"{hd.date} {hd.name}");
                }
                
            }

        }
    }
}
