using System;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace DevArena.ConsoleClient
{
    class Program
    {
        private static TokenClient _client;
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");

            Console.WriteLine("DevArena IdentityServer4 Demo For ResourceOwnerPassWord and ClientCredentials Client Grant type flows");

            var selection = "0";

            while (!selection.Equals("1") && !selection.Equals("2") && !selection.Equals("3"))
            {
                Console.WriteLine("Options:");
                Console.WriteLine("1. User login authentication                 (press l)");
                Console.WriteLine("2. Application / device authentication       (press 2)");
                Console.WriteLine("3. Exit                                      (press 3)");
                selection = Console.ReadLine();
                Console.WriteLine("\n");
            }

            if (selection == "1")
            {
                Console.WriteLine("=== User login authentication ===");
                Console.WriteLine("\n");

                Console.Write("Enter Username: ");
                var userName = Console.ReadLine();

                Console.Write("Enter Password: ");
                var userPass = Console.ReadLine();
                Console.WriteLine("\n");

                var tokenClientPass = new TokenClient(disco.TokenEndpoint, "console.clientid", "console.secret");
                
                var tokenResponsePass = await tokenClientPass.RequestResourceOwnerPasswordAsync(userName, userPass);

                if (tokenResponsePass.IsError)
                {
                    Console.WriteLine("Your authentication is NOT SUCCESSFUL.");
                    Console.WriteLine(tokenResponsePass.Error);
                    Console.WriteLine("\n");
                }
                else
                {
                    Console.WriteLine("Your authentication is SUCCESSFUL.");
                    Console.WriteLine(tokenResponsePass.Json);
                    Console.WriteLine("\n");

                    Console.WriteLine("Get users:");
                    Console.WriteLine("1. All users (Administrators only)");
                    Console.WriteLine("2. Guest and external users (Administrators and Guests)");
                    Console.WriteLine("3. Show tokens");
                    Console.WriteLine("4. Exit");

                    var access = "0";
                    access = Console.ReadLine();
                    HttpClient client;
                    if (access != "4")
                    {
                        while (access == "1" || access == "2" || access == "3")
                        {
                            if (access == "3")
                            {
                                var access_token = tokenResponsePass.AccessToken;
                                
                                
                                var httpC= new HttpClient();
                                httpC.BaseAddress = new Uri("http://localhost:5000/");
                                httpC.SetBearerToken(access_token);
                                var resp = await httpC.GetAsync("connect/userinfo");
                                //todo get content
                                
                                var id_token = tokenResponsePass.IdentityToken;

                                Console.WriteLine("Access_token: " + access_token);
                                Console.WriteLine("Identity_token: " + id_token);
                                Console.WriteLine();
                            }
                            else
                            {



                                client = new HttpClient();
                                client.BaseAddress = new Uri("http://localhost:53377/");
                                client.SetBearerToken(tokenResponsePass.AccessToken);
                                var response = await client.GetAsync(access == "1" ? "admin" : "guest");
                                Console.WriteLine("Response: " + response.StatusCode.ToString());

                                Console.WriteLine();
                                Console.WriteLine("Try another API resource:");
                                Console.WriteLine("1. All users (Administrators only)");
                                Console.WriteLine("2. Guest and external users (Administrators and Guests)");
                                Console.WriteLine("3. Exit");

                                
                            }
                            access = Console.ReadLine();
                        }
                    }
                }
                Console.ReadLine();
            }
            else if (selection == "2")
            {
                var tokenClientCreds = new TokenClient(disco.TokenEndpoint, "console.clientid", "console.secret");
                var tokenResponsePass = await tokenClientCreds.RequestClientCredentialsAsync("devarena.api.client_access");

                if (tokenResponsePass.IsError)
                    Console.WriteLine("Client console.clientid/console.secret Not Authorized");
                else
                {
                    Console.WriteLine("Client console.clientid/console.secret Successfully Authorized");
                    Console.WriteLine(tokenResponsePass.Json);
                }

                Console.WriteLine("Get users:");
                Console.WriteLine("1. All users (Administrators only)");
                Console.WriteLine("2. Guest and external users (Administrators and Guests)");
                Console.WriteLine("3. Exit");

                var access = "0";
                access = Console.ReadLine();
                HttpClient client;
                if (access != "3")
                {
                    while (access == "1" || access == "2")
                    {
                        client = new HttpClient();
                        client.BaseAddress = new Uri("http://localhost:53377/");
                        client.SetBearerToken(tokenResponsePass.AccessToken);
                        var response = await client.GetAsync(access == "1" ? "admin" : "guest");
                        Console.WriteLine("Response: " + response.StatusCode.ToString());

                        Console.WriteLine();
                        Console.WriteLine("Try another API resource:");
                        Console.WriteLine("1. All users (Administrators only)");
                        Console.WriteLine("2. Guest and external users (Administrators and Guests)");
                        Console.WriteLine("3. Exit");

                        access = Console.ReadLine();
                    }
                }

                Console.ReadLine();
            }

        }

        static void GetAdminAcces()
        {
        }
    }
}
