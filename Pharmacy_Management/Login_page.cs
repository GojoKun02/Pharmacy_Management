using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Pharmacy_Management
{
    public partial class Login_page : Form
    {
        public Login_page()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

        public static class Session
        {
            public static int UserID { get; set; }
            public static string Username { get; set; }
            public static string Role { get; set; }
        }

        private bool IsValidPassword(string password)
        {
            // Regular expression for password validation
            string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }

       

        private void button1_Click_1(object sender, EventArgs e)
        {

                string username = textBox1.Text.Trim();
                string password = textBox2.Text.Trim();

                // Check if fields are empty
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Username and Password cannot be empty.");
                    return;
                }

                // Validate password
                if (!IsValidPassword(password))
                {
                    MessageBox.Show("Password must contain at least:\n" +
                                    "- One uppercase letter\n" +
                                    "- One lowercase letter\n" +
                                    "- One number\n" +
                                    "- One special character\n" +
                                    "- At least 8 characters long");
                    return;
                }

                string connectionString = "Data Source=(localdb)\\Mylocaldb;Initial Catalog=Pharmacy_Management;Integrated Security=True";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        string query = "SELECT UserID, Role FROM Users WHERE Username = @Username AND Password = @Password";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Username", username);
                            cmd.Parameters.AddWithValue("@Password", password);

                            SqlDataReader reader = cmd.ExecuteReader();

                            if (reader.Read())
                            {
                                Session.UserID = reader.GetInt32(0);
                                Session.Username = username;
                                Session.Role = reader.GetString(1);

                                MessageBox.Show($"Welcome {Session.Username}!");

                                if (Session.Role == "Admin")
                                {
                                    AdminDashboard adminDashboard = new AdminDashboard();
                                    adminDashboard.Show();
                                }
                                else if (Session.Role == "Employee")
                                {
                                    EmployeeDashboard employeeDashboard = new EmployeeDashboard();
                                    employeeDashboard.Show();
                                }

                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Invalid Username or Password.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }

        private void button3_Click(object sender, EventArgs e)
        {


            DialogResult result = MessageBox.Show("Are you sure you want to clear both fields?",
                                                        "Confirm Delete",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Warning);
            textBox1.Clear();
            textBox2.Clear();
        }
    }
    }

