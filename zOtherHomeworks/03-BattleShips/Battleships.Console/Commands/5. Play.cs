using System.Collections.Generic;
using System.Net.Http;
using Battleships.Console.DTO;
using Newtonsoft.Json;

namespace Battleships.Console.Commands
{
    public class Play
    {
        private const string JoinGameEndPoint = "http://localhost:62859/api/games/join";

        public static async void Now(string id, string x, string y)
        {
            var httpClient = new HttpClient();

            var token = Login.Token;

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("GameId", id),
                new KeyValuePair<string, string>("PositionX", x),
                new KeyValuePair<string, string>("PositionY", y)
            });

            var response = await httpClient.PostAsync(JoinGameEndPoint, content);
            
            var message = response.Content.ReadAsStringAsync().Result;
            
            if (response.IsSuccessStatusCode)
            {
                System.Console.WriteLine("Great game!");
            }
            else
            {   
                var result = JsonConvert.DeserializeObject<GetMessageDto>(message);
                System.Console.WriteLine(response.StatusCode + " - " + result.Message);
            }
            


        }
        
    }
}