using System;
using System.Collections.Generic;
using System.Net.Http;



namespace Battleships.ConsoleClient
{
    public class ConsoleClientMain
    {
        private static string token = string.Empty;

        static void Main(string[] args)
        {
            string line = Console.ReadLine();

            while (line != "end")
            {
                var splittedInput = line.Trim().Split(' ');

                var command = splittedInput[0];

                if (command == "register")
                {
                    RegisterUser(splittedInput);
                }
                else if (command == "login")
                {
                    LoginUser(splittedInput);
                }
                else if (command == "create-game")
                {
                    CreateGame();
                }
                else if (command == "join-game")
                {
                    JoinGame(splittedInput);
                }
                else if (command == "play")
                {
                    PlayGame(splittedInput);
                }

                line = Console.ReadLine();
            }
        }

        private static async void PlayGame(string[] splittedInput)
        {
            var gameId = splittedInput[1];
            var x = splittedInput[2];
            var y = splittedInput[3];
            var httpClient = new HttpClient();

            using (httpClient)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", gameId), 
                    new KeyValuePair<string, string>("PositionX", x), 
                    new KeyValuePair<string, string>("PositionY", y), 
                });
                var response = await httpClient.PostAsync("http://localhost:62859/api/Games/play", content);

                if (!response.IsSuccessStatusCode)
                {
                    var messageObj = response.Content.ReadAsAsync<ErrorDTO>().Result;
                    Console.WriteLine(messageObj.Message + "; Description: " + messageObj.ExceptionMessage);
                }
                else
                {
                    var gameid = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Game is running, id: " + gameid);
                }
            }
        }

        private static async void JoinGame(string[] splittedInput)
        {
            var gameId = splittedInput[1];
            var httpClient = new HttpClient();

            using (httpClient)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GameId", gameId),
                });
                var response = await httpClient.PostAsync("http://localhost:62859/api/Games/join", content);
           
                if (!response.IsSuccessStatusCode)
                {
                    var messageObj = response.Content.ReadAsAsync<ErrorDTO>().Result;
                    Console.WriteLine(messageObj.Message + "; Description: " + messageObj.ExceptionMessage);
                }
                else
                {
                    var gameid = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Game joined, id: " + gameid);
                }
            }
        }


        private static async void CreateGame()
        {
            var httpClient = new HttpClient();

            using (httpClient)
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("", ""), 
                });
                var response = await httpClient.PostAsync("http://localhost:62859/api/Games/create", content);

                if (!response.IsSuccessStatusCode)
                {
                    var messageObj = response.Content.ReadAsAsync<ErrorDTO>().Result;
                    Console.WriteLine(messageObj.Message + "; Description: " + messageObj.ExceptionMessage);
                }
                else
                {
                    var gameid = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine("Game craeted, id: " + gameid);
                }
            }
        }

        private static async void LoginUser(string[] splittedInput)
        {
            if (token != string.Empty)
            {
                Console.WriteLine("You are logged!");
                return;
            }

            var email = splittedInput[1];
            var password = splittedInput[2];

            var httpClient = new HttpClient();

            using (httpClient)
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", email),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", "password"),
                });

                var response = await httpClient.PostAsync("http://localhost:62859/token", content);
                if (!response.IsSuccessStatusCode)
                {
                    var messageObj = response.Content.ReadAsAsync<ErrorDTO>().Result;
                    Console.WriteLine(messageObj.Message + "; Description: " + messageObj.ExceptionMessage);
                }
                else
                {
                    var result = response.Content.ReadAsAsync<LoginDTO>().Result;
                    token = result.Access_token;
                    Console.WriteLine("Log successful");
                }
            }
        }

        private static async void RegisterUser(params string[] splittedInput)
        {
            if (token != string.Empty)
            {
                Console.WriteLine("You are logged!");
                return;
            }

            var email = splittedInput[1];
            var password = splittedInput[2];
            var confirmPassword = splittedInput[3];

            var httpClient = new HttpClient();

            using (httpClient)
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("email",email),
                    new KeyValuePair<string, string>("password",password), 
                    new KeyValuePair<string, string>("confirmpassword",confirmPassword), 
                });

                var response = await httpClient.PostAsync("http://localhost:62859/api/account/register", content);

                if (!response.IsSuccessStatusCode)
                {
                    // var messageObj = response.Content.ReadAsStringAsync().Result;

                    var messageObj = response.Content.ReadAsAsync<ErrorDTO>().Result;
                    Console.WriteLine(messageObj.Message + "; Description: " + messageObj.ExceptionMessage);
                    //   Console.WriteLine(messageObj);
                }
                else
                {
                    Console.WriteLine("Registration complete!");
                }
            }
        }
    }
}
