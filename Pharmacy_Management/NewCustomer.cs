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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Pharmacy_Management
{
    public partial class demo : Form
    {
        private string connectionString = @"Server=(localdb)\Mylocaldb; Database=Pharmacy_Management; Integrated Security=True;";
        public demo()
        {
            InitializeComponent();
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox1.KeyPress += TextBox4_KeyPress;
        }
      
        private void label6_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
        }
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters (e.g., backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Block non-numeric characters
            }
        }
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Save_Changes_btn_Click(object sender, EventArgs e)
        {
            // Get text from textboxes
            string value1 = textBox1.Text.Trim();
            string value2 = textBox2.Text.Trim();
            string value3 = textBox3.Text.Trim();
            string value4 = textBox6.Text.Trim();
            string value5 = textBox4.Text.Trim();

            // Ensure all fields are filled
            if (string.IsNullOrWhiteSpace(value1) || string.IsNullOrWhiteSpace(value2) ||
                string.IsNullOrWhiteSpace(value3) || string.IsNullOrWhiteSpace(value4))
            {
                MessageBox.Show("Please fill all fields before adding to the table.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Insert Customer data
                    string customerQuery = "INSERT INTO Customer (CustomerName, Email, PhoneNo) VALUES (@CustomerName, @Email, @PhoneNo)";
                    using (SqlCommand cmd = new SqlCommand(customerQuery, conn))
                    {
                        cmd.Parameters.Add("@CustomerName", SqlDbType.NVarChar).Value = textBox2.Text;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = textBox3.Text;
                        cmd.Parameters.Add("@PhoneNo", SqlDbType.NVarChar).Value = textBox1.Text;
                        cmd.ExecuteNonQuery();
                    }

                    // Insert Medicine data
                    string medicineQuery = "INSERT INTO Medicine (MedicineName) VALUES (@MedicineName)";
                    using (SqlCommand cmd = new SqlCommand(medicineQuery, conn))
                    {
                        cmd.Parameters.Add("@MedicineName", SqlDbType.NVarChar).Value = textBox6.Text;
                        cmd.Parameters.Add("@MedicineID", SqlDbType.NVarChar).Value = textBox4.Text;
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Data added successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

          


            // Add row to DataGridView
            dataGridView1.Rows.Add(value1, value2, value3, value4, value5);

            // Clear textboxes after adding data
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox6.Clear();
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void TextBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters (e.g., backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Block non-numeric characters
            }
        }
    }
}
