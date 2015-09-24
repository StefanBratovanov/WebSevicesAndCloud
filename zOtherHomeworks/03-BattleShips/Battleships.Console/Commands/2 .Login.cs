using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

namespace Battleships.Console.Commands
{
    using System;
    public class Login
    {
        private const string LoginEndpoint = "http://localhost:62859/token";
        protected internal static string Token = "";
        public static async void User(string username, string password)
        {
            var httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Username", username),
                new KeyValuePair<string, string>("Password", password),
                new KeyValuePair<string, string>("Grant_type", "password")
            });

            var response = await httpClient.PostAsync(LoginEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();

                var tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                var token = tokenDictionary["access_token"];
                Token += token;
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                
                Console.WriteLine("Successfull login.");
                System.Console.WriteLine();
                Console.WriteLine("Enter the next command:");
                System.Console.WriteLine();
            }
            else{
                
                Console.WriteLine("The username or password is incorrect.");
            }
        }

        //internal static string GetToken()
        //{
        //    return Token;
        //}
    }
}