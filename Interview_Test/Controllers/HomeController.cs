using Interview_Test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics;

namespace Interview_Test.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public string _connectionstring = "Server=(localdb)\\MSSQLLocalDB;Database=Inter_Test;Integrated Security=True;";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
        [HttpGet]
        public IActionResult showMasterForm()
        {
            List<UserInfoViewModel> users = new List<UserInfoViewModel>();

            // Create a SQL connection
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Define your SQL query
                    string query = "exec getAllInformation";

                    // Create a SQL command
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the command and obtain a reader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are any rows to read
                            while (reader.Read())
                            {
                                // Create a new instance of the ViewModel for each record
                                var user = new UserInfoViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Facebook = reader.GetString(3),
                                    Instagram = reader.GetString(4),
                                    Twitter = reader.GetString(5)
                                };

                                // Add the user to the list
                                users.Add(user);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred while connecting to the database:");
                    Console.WriteLine(ex.Message);
                }
            }

            // Pass the list of users to the view
            return View(users);
        }

        // create form view
        public IActionResult Master_Form()
        {
            List<UserInfoViewModel> users = new List<UserInfoViewModel>();

            // Create a SQL connection
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                try
                {
                    // Open the connection
                    connection.Open();

                    // Define your SQL query
                    string query = "exec getAllInformation";

                    // Create a SQL command
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Execute the command and obtain a reader
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Check if there are any rows to read
                            while (reader.Read())
                            {
                                // Create a new instance of the ViewModel for each record
                                var user = new UserInfoViewModel
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Email = reader.GetString(2),
                                    Facebook = reader.GetString(3),
                                    Instagram = reader.GetString(4),
                                    Twitter = reader.GetString(5)
                                };

                                // Add the user to the list
                                users.Add(user);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine("An error occurred while connecting to the database:");
                    Console.WriteLine(ex.Message);
                }
            }

            // Pass the list of users to the view
            return View(users);

        }

        [HttpPost]
        public IActionResult createMasterForm([FromBody] UserData userData)
        {
            string jsonSocialMedia = JsonConvert.SerializeObject(userData.SocialMedia); // Convert social media list to JSON

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("insertInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters for the stored procedure
                        command.Parameters.AddWithValue("@p_Name", userData.Name);
                        command.Parameters.AddWithValue("@p_Email", userData.Email);
                        command.Parameters.AddWithValue("@p_Password", userData.Password);
                        command.Parameters.AddWithValue("@ChildrenInfo", jsonSocialMedia); // JSON data for social media

                        command.ExecuteNonQuery(); // Execute the stored procedure
                    }
                }
                catch (SqlException ex)
                {
                    // Handle exception (logging or returning error response)
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return StatusCode(500, "An error occurred while saving the data.");
                }
            }

            return Ok("Data inserted successfully.");

        }


        [HttpGet]
        public IActionResult EditMasterForm(int id)
        {
            // ViewModel to hold the information and social media data
            UserData userData = new UserData
            {
                SocialMedia = new List<SocialMedia>() // Initialize the SocialMedia list
            };

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                connection.Open();

                // Fetch user information
                using (SqlCommand command = new SqlCommand("SELECT * FROM Information WHERE Id = @p_InfoId", connection))
                {
                    command.Parameters.AddWithValue("@p_InfoId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userData.InfoId = id;
                            userData.Name = reader["Name"].ToString();
                            userData.Email = reader["Email"].ToString();
                            userData.Password = reader["Password"].ToString();
                        }
                    }
                }

                // Fetch associated social media records
                using (SqlCommand command = new SqlCommand("SELECT Facebook, Instagram, Twitter FROM Information_Extended WHERE Info_Id = @p_InfoId", connection))
                {
                    command.Parameters.AddWithValue("@p_InfoId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userData.SocialMedia.Add(new SocialMedia
                            {
                                Facebook = reader["Facebook"].ToString(),
                                Instagram = reader["Instagram"].ToString(),
                                Twitter = reader["Twitter"].ToString()
                            });
                        }
                    }
                }
            }

            return View(userData); // Pass the userData to the view
        }

        [HttpPost]
        public IActionResult editMasterForm([FromBody] UserData userData)
        {
            string jsonSocialMedia = JsonConvert.SerializeObject(userData.SocialMedia); // Convert list to JSON

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("updateInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters for the stored procedure
                        command.Parameters.AddWithValue("@p_InfoId", userData.InfoId);
                        command.Parameters.AddWithValue("@p_Name", userData.Name);
                        command.Parameters.AddWithValue("@p_Email", userData.Email);
                        command.Parameters.AddWithValue("@p_Password", userData.Password);
                        command.Parameters.AddWithValue("@ChildrenInfo", jsonSocialMedia);  // JSON data for social media

                        command.ExecuteNonQuery(); // Execute the stored procedure
                    }
                }
                catch (SqlException ex)
                {
                    // Handle exception (logging or returning error response)
                    Console.WriteLine("SQL Error: " + ex.Message);
                    return StatusCode(500, "An error occurred while updating the data.");
                }
            }

            return Ok("Data updated successfully.");
        }

        public IActionResult deleteMasterForm(int Id) 
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

// ViewModel to receive data from the form
public class UserData
{
    public int InfoId { get; set; } // Id of the record to update
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<SocialMedia> SocialMedia { get; set; }

    
}

// Class for social media data (platforms)
public class SocialMedia
{
    public string Facebook { get; set; }
    public string Instagram { get; set; }
    public string Twitter { get; set; }
}