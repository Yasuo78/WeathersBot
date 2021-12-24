﻿using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using WeathersBot;

namespace WeatherBot
{
    class Program
    {
        private static string token { get; set; } = "5047782576:AAFh6FbamiARsWjwsb54rVSJBe9qqgtlys8";
        private static TelegramBotClient client;

        static string NameCity;
        static float tempOfCity;
        static string nameOfCity;

        static string answerOnWether;

        public static void Main(string[] args)
        {
            client = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };

            var me = client.GetMeAsync().Result;
            Console.WriteLine($"Bot_Id: {me.Id} \nBot_Name: {me.FirstName} ");

            client.OnMessage += Bot_OnMessage;
            client.StartReceiving();
            Console.ReadLine();
            client.StopReceiving();
        }

        private static async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            var message = e.Message;

            if (message.Type == MessageType.Text)
            {
                NameCity = message.Text;
                Weather(NameCity);
                Celsius(tempOfCity);
                await client.SendTextMessageAsync(message.Chat.Id, $"{answerOnWether} \n\nТемпература в {nameOfCity}: {Math.Round(tempOfCity)} °C");

                Console.WriteLine(message.Text);
            }
        }

        public static void Weather(string cityName)
        {
            try
            {
                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityName + "&unit=metric&appid=f1f20f83b39b498141c8e0c25270b1bb";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest?.GetResponse();
                string response;

                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    response = streamReader.ReadToEnd();
                }
                Wea therResponse weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(response);

                nameOfCity = weatherResponse.Name;
                tempOfCity = weatherResponse.Main.Temp - 273;
            }
            catch (System.Net.WebException)
            {
                Console.WriteLine("Возникло исключение");
                return;
            }
        }

        public static void Celsius(float celsius)
        {
            if (celsius <= 10)
                answerOnWether = "Сегодня прохладно!";
            else
                answerOnWether = "Сегодня жарко)";
        }
    }
}