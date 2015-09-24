using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Battleships.Data;

namespace Battleships.Console.Commands
{
    public class CreateGame
    {
        private const string CreateGameEndPoint = "http://localhost:62859/api/games/create";
        public static async void Create()
        {
            var httpClient = new HttpClient();

            var token = Login.Token;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(),
            });
            
            var response = await httpClient.PostAsync(CreateGameEndPoint, content);

            System.Console.WriteLine(response.StatusCode);
            System.Console.WriteLine("Check the game id.");
        }
    }
}