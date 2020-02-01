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
                    Console.WriteLine($"{user} says: {message}");
                });

                await connection.StartAsync();

                Console.WriteLine(
                    @"Instructions:
Type a message and hit <Enter> to message all users.
Type [userName] followed by a message to message specific user.
Type ""quit"" to exit.");

                string input = string.Empty;
                while(input != "quit")
                {
                    // check for direct user message
                    (string sendUser, string sendMessage) = 
                        (start: input.IndexOf('['), end: input.IndexOf(']')) switch
                        {
                            var (start, end) when
                                start == 0 && end > 0 => (input[1..end], input[(end + 1)..^0]),
                            _ => (string.Empty, input)
                        };

                    if(sendMessage == string.Empty)
                        await connection.InvokeAsync("Connect", userName);
                    else if (sendUser == string.Empty)
                        await connection.InvokeAsync("SendAll", userName, sendMessage);
                    else
                        await connection.InvokeAsync("SendUser", userName, sendUser, sendMessage);

                    Console.Write("Enter message (or \"quit\" to exit): ");
                    input = Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
