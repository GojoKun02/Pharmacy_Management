using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy_Management
{
    public partial class New_user : Form
    {
        private string connectionString = "Data Source=(localdb)\\Mylocaldb;Initial Catalog=Pharmacy_Management;" +
            "Integrated Security=True;";
        public New_user()
        {
            InitializeComponent();
            txtpassword.PasswordChar = '*';
            txtpasskey.PasswordChar = '*';
            



        }

        private void New_user_Load(object sender, EventArgs e)
        {

        }
      

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private bool IsAdminAuthorized(string adminUsername, string adminPassword)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = "SELECT Password FROM Users " +
                        "WHERE Username = 'masteradmin' AND Role = 'Admin'";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", adminUsername);
                        string storedPassword = cmd.ExecuteScalar() as string;
                        
                        return storedPassword != null && storedPassword == adminPassword;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        private bool AddNewUser(string username, string password, string role)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = "INSERT INTO Users (Username, [Password], Role) VALUES (@Username, @Password, @Role)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@Password", password); 
                        cmd.Parameters.AddWithValue("@Role", role);

                        return cmd.ExecuteNonQuery() > 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return false;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string adminPassword = txtpasskey.Text.Trim();

            string newUsername = txtusername.Text.Trim();

            string newPassword = txtpassword.Text.Trim();

            string role = comborole.SelectedItem?.ToString();


            if (role != "Admin" && role != "Employee")
            {
                MessageBox.Show("Invalid role. Choose 'Admin' or 'Employee'.");
                return;
            }

            if (!IsAdminAuthorized("masteradmin", adminPassword))
            {
                MessageBox.Show("Invalid Admin Password. Authorization failed!");
                return;
            }

            if (AddNewUser(newUsername, newPassword, role))
            {
                MessageBox.Show("New user added successfully!");


            }
            else
            {
                MessageBox.Show("Failed to add new user. User may already exist.");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to clear both fields?",
                                                        "Confirm Delete",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Warning);
            comborole.Items.Clear();
            txtpasskey.Clear();
            txtusername.Clear();
            txtpassword.Clear();

        }
    }
}