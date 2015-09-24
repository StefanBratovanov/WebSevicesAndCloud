using System.Collections.Generic;
using System.Net.Http;
using Battleships.Console.DTO;
using Battleships.Data;
using Newtonsoft.Json;

namespace Battleships.Console.Commands
{
    public class JoinGame
    {
        private const string JoinGameEndPoint = "http://localhost:62859/api/games/join";

        public static async void Join(string id)
        {
            //In order to join the created game: 
            //open a new terminal, 
            //register a new user, 
            //log in with the new user,
            //and join the game. 

            var httpClient = new HttpClient();

            var token = Login.Token;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("GameId", id),
            });

            var response = await httpClient.PostAsync(JoinGameEndPoint, content);

            var message = response.Content.ReadAsStringAsync().Result;
            if (response.IsSuccessStatusCode)
            {
                System.Console.WriteLine("You joined the game!");
                System.Console.WriteLine();
                System.Console.WriteLine("Lets play!");
                System.Console.WriteLine();
            }
            else
            {
                var result = JsonConvert.DeserializeObject<GetMessageDto>(message);
                System.Console.WriteLine(response.StatusCode + " - " + result.Message);
                System.Console.WriteLine("Try again to join other users game:");
                System.Console.WriteLine();
            }
        }
    }
}