using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SignalRBasicAuth.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                string userName = args[0];
                string password = args[1];

                var connection = new HubConnectionBuilder()
                    .WithUrl("https://localhost:5001/EchoHub", options =>
                    {
                        options.Headers.Add("Authorization", "Basic " + 
                            Convert.ToBase64String(Encoding.UTF8.GetBytes(userName + ":" + password)));
                    })
                    .Build();

                connection.On<string, string>("ReceiveMessage", (user, message) =>
                {
                    Console.WriteLine($"Reply from server - user: {user}  message: {message}");
                });

                await connection.StartAsync();

                string message = "Hello";
                while(message != "quit")
                {
                    await connection.InvokeAsync("SendMessage", userName, message);

                    Console.WriteLine("Enter a message to send (or type \"quit\" to exit)");
                    message = Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
