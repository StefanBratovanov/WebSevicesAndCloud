using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Battleships.Data;
using Newtonsoft.Json;

namespace Battleships.ConsoleClient
{
    public class ConsoleClient
    {
        private static string currentUsser = "";
        private const string baseUrl = "http://localhost:62858/";
        private const string registerEndpoint = baseUrl + "api/account/register";
        private const string loginEndpoint = baseUrl + "Token";
        private const string createEndpoint = baseUrl + "api/games/create";
        private const string joinEndpoint = baseUrl + "api/games/join";
        private const string playEndpoint = baseUrl + "api/games/play";

        static void Main(string[] args)
        {

            while (true)
            {
                var commandArgs = CommandParser();
                switch (commandArgs[0])
                {
                    case "register":
                        RegisterCommand(commandArgs);
                        break;
                    case "login":
                        LoginCommand(commandArgs);
                        break;
                    case "create-game":
                        CreateCommand();
                        break;
                    case "join-game":
                        JoinCommand(commandArgs);
                        break;
                    case "play":
                        PlayCommand(commandArgs);
                        break;
                    case "turnoff":
                        return;
                        break;
                }
            }
        }

        private static async void PlayCommand(string[] args)
        {
            var httpClient = new HttpClient();
            using (httpClient)
            {
                httpClient.DefaultRequestHeaders.Add(
                 "Authorization", "Bearer " + currentUsser);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", args[1]),
                    new KeyValuePair<string, string>("PositionX", args[2]),
                    new KeyValuePair<string, string>("PositionY", args[3])                        
                });

                var response = await httpClient.PostAsync(playEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
            }
        }

        private static async void JoinCommand(string[] args)
        {
            var httpClient = new HttpClient();
            using (httpClient)
            {
                httpClient.DefaultRequestHeaders.Add(
                 "Authorization", "Bearer " + currentUsser);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", args[1]),
                });

                var response = await httpClient.PostAsync(joinEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        response.Content.ReadAsStringAsync().Result);
                }

                Console.WriteLine("Successfully join in game {0}!", response.Content.ReadAsStringAsync().Result);
            }
        }

        private static async void CreateCommand()
        {
            var httpClient = new HttpClient();
            using (httpClient)
            {
                httpClient.DefaultRequestHeaders.Add(
                 "Authorization", "Bearer " + currentUsser);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", "u"),
                });

                var response = await httpClient.PostAsync(createEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        response.Content.ReadAsStringAsync().Result);
                }

                Console.WriteLine("Game {0} successfully created in!", response.Content.ReadAsStringAsync().Result);
            }
        }

        private static async void LoginCommand(string[] args)
        {
            var httpClient = new HttpClient();
            using (httpClient)
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", args[1]),
                    new KeyValuePair<string, string>("password", args[2]),
                    new KeyValuePair<string, string>("grant_type", "password"),
                });

                var response = await httpClient.PostAsync(loginEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        response.Content.ReadAsStringAsync().Result);
                }

                var snn = JsonConvert.DeserializeObject<Snn>(response.Content.ReadAsStringAsync().Result);
                currentUsser = snn.Access_token;
                await SaveSnn(snn.Access_token, args[1]);
                //context.Users.FirstOrDefault(u => u.UserName == args[1]).Ssn = snn.Access_token;
                Console.WriteLine("User {0} successfully logged in!", args[1]);
            }
        }

        private static async Task SaveSnn(string snn, string username)
        {
            var httpClientSnn = new HttpClient();
            using (httpClientSnn)
            {

                var builder = new UriBuilder(baseUrl + "api/users/" + username + "/ssn");
                var fullEndpoint = builder.ToString();
                var content = new FormUrlEncodedContent(new[]
               {
                    new KeyValuePair<string, string>("ssn", snn)
                   
                });

                var response = await httpClientSnn.PutAsync(fullEndpoint, content);
            }
        }

        private static async void RegisterCommand(string[] args)
        {
            var httpClient = new HttpClient();

            using (httpClient)
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email", args[1]),
                    new KeyValuePair<string, string>("password", args[2]),
                    new KeyValuePair<string, string>("confirmPassword", args[3]),
                });

                var response = await httpClient.PostAsync(registerEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(
                        response.Content.ReadAsStringAsync().Result);
                }

                Console.WriteLine("User {0} successfully registered!", args[1]);
            }
        }

        private static string[] CommandParser()
        {
            var inputLIne = Console.ReadLine();
            var args = inputLIne.Split(' ');
            return args;
        }
    }

    public class Snn
    {
        public string Access_token { get; set; }
    }
}
