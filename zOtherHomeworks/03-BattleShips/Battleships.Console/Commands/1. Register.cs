using System.Collections.Generic;
using System.Net.Http;
using Battleships.Console.DTO;
using Newtonsoft.Json;

namespace Battleships.Console.Commands
{
    public class Register
    {
        private const string RegisterEndpoint = "http://localhost:62859/api/account/register";

        public static async void User(string email, string password, string confirmPassword)
        {
            var httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", email),
                new KeyValuePair<string, string>("Password", password),
                new KeyValuePair<string, string>("ConfirmPassword", confirmPassword)
            });

            var response = await httpClient.PostAsync(RegisterEndpoint, content);

            System.Console.WriteLine(response.StatusCode + " " + response.Content.ReadAsStringAsync().Result);

            System.Console.WriteLine();
            System.Console.WriteLine("Enter the next command:");
            System.Console.WriteLine();
        }
    }
}