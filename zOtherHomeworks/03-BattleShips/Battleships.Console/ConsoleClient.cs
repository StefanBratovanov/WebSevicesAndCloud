using System;
using Battleships.Console.Commands;

namespace Battleships.Console
{
    using System;
   
    class ConsoleClient
    {
        static void Main()
        {


            while (true)
            {
                var readLine = Console.ReadLine();
                var input = readLine.Split();
                try
                {
                    var command = input[0];
                    switch (command)
                    {
                        case  "register":
                            var email = input[1];
                            var password = input[2];
                            var confirmPassword = input[3];
                            
                            Register.User(email, password, confirmPassword);
                            break;
                        case "login":
                            var username = input[1];
                            var loginPass = input[2];
                            Login.User(username, loginPass);
                            break;
                        case "create-game":
                            CreateGame.Create();
                            break;
                        case "join-game":
                            var gameId = input[1];
                            JoinGame.Join(gameId);
                            break;
                        case "play":
                            var id = input[1];
                            var x = input[2];
                            var y = input[3];
                            Play.Now(id, x, y);
                            break;
                        default:
                             Console.WriteLine("Enter a valid command.");
                            break;

                    }
                    
                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("The parametars are invalide. Enter valid parametars:");
                }
            }
        }
    }
}
