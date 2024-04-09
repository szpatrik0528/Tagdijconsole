using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; // Make sure to install Newtonsoft.Json package via NuGet Package Manager

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Replace the URL with your REST API endpoint for fetching data from the 'users' table
            string usersApiUrl = "http://localhost/TagdijBackend/index.php?ugyfel";
            // Replace the URL with your REST API endpoint for fetching data from the 'befizetes' table
            string befizetesApiUrl = "http://localhost/TagdijBackend/index.php?befizetes";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Fetching users
                    HttpResponseMessage usersResponse = await client.GetAsync(usersApiUrl);
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        string usersResponseBody = await usersResponse.Content.ReadAsStringAsync();
                        var users = JsonConvert.DeserializeObject<List<User>>(usersResponseBody);

                        // Printing all names
                        Console.WriteLine("Felhasználók nevei:");
                        foreach (var user in users)
                        {
                            Console.WriteLine($"Név: {user.Nev}");
                        }

                        // Fetching befizetesek
                        HttpResponseMessage befizetesResponse = await client.GetAsync(befizetesApiUrl);
                        if (befizetesResponse.IsSuccessStatusCode)
                        {
                            string befizetesResponseBody = await befizetesResponse.Content.ReadAsStringAsync();
                            var befizetesek = JsonConvert.DeserializeObject<List<Befizetes>>(befizetesResponseBody);

                            // Find user with the highest sum of befizetes
                            var userWithHighestSum = befizetesek
                                .GroupBy(b => b.User)
                                .Select(g => new { User = g.Key, TotalBefizetes = g.Sum(b => b.Osszeg) })
                                .OrderByDescending(u => u.TotalBefizetes)
                                .FirstOrDefault();

                            if (userWithHighestSum != null)
                            {
                                Console.WriteLine($"Legnagyobb összegű befizetést tulajdonos: {userWithHighestSum.User}, Összeg: {userWithHighestSum.TotalBefizetes}");
                            }
                            else
                            {
                                Console.WriteLine("Nincs adat a befizetésekről.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to fetch befizetes data. Status code: {befizetesResponse.StatusCode}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to fetch users data. Status code: {usersResponse.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        // Define a class to represent the structure of the user object
        class User
        {
            public string Nev { get; set; }
            // Add more properties if necessary to represent other fields in the user object
        }

        // Define a class to represent the structure of the befizetes object
        class Befizetes
        {
            public string User { get; set; }
            public decimal Osszeg { get; set; }
            // Add more properties if necessary to represent other fields in the befizetes object
        }
    }
}
