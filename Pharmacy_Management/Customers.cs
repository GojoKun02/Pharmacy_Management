using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pharmacy_Management
{
    public partial class Customers : Form
    {
        private string connectionString = @"Server=(localdb)\Mylocaldb; Database=Pharmacy_Management; Integrated Security=True;";
        private DataTable dt = new DataTable();

        public Customers()
        {
            InitializeComponent();
            textBox1.TextChanged += SearchTextChanged;
            textBox2.TextChanged += SearchTextChanged;
            textBox1.KeyPress += TextBox1_KeyPress;  // Adding the KeyPress event handler
        }

        private void LoadData(string phonePrefix, string namePrefix)
        {
            string query = @"
                SELECT
                    c.PhoneNo,
                    c.CustomerName,
                    c.Email,
                    ISNULL(STRING_AGG(m.MedicineName, ', '), 'No Medicine Purchased') AS Medicines
                FROM dbo.Customer c
                LEFT JOIN dbo.Sale s ON c.PhoneNo = s.PhoneNo
                OUTER APPLY (
                    SELECT value AS MedicineID
                    FROM STRING_SPLIT(COALESCE(CAST(s.MedicineID AS VARCHAR(MAX)), ''), ',')
                ) AS SplitMed
                LEFT JOIN dbo.Medicine m ON TRY_CAST(SplitMed.MedicineID AS INT) = m.MedicineID
                WHERE (@PhonePrefix = '' OR c.PhoneNo LIKE @PhonePrefix + '%')
                  AND (@NamePrefix = '' OR c.CustomerName LIKE @NamePrefix + '%')
                GROUP BY c.PhoneNo, c.CustomerName, c.Email;
            ";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PhonePrefix", phonePrefix ?? "");
                cmd.Parameters.AddWithValue("@NamePrefix", namePrefix ?? "");

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    dt.Clear();
                    adapter.Fill(dt);
                    dt.AcceptChanges();
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void SearchTextChanged(object sender, EventArgs e)
        {
            string phoneSearch = textBox1.Text.Trim();
            string nameSearch = textBox2.Text.Trim();
            LoadData(phoneSearch, nameSearch);
        }

        private void Delete_btn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string phoneNo = dataGridView1.SelectedRows[0].Cells["PhoneNo"].Value?.ToString();

                if (!string.IsNullOrEmpty(phoneNo))
                {
                    DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?",
                                                          "Confirm Delete",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            string query = "DELETE FROM Customer WHERE PhoneNo = @PhoneNo";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@PhoneNo", phoneNo);
                                conn.Open();
                                cmd.ExecuteNonQuery();
                            }
                        }

                        LoadData(textBox1.Text, textBox2.Text);
                        MessageBox.Show("Customer deleted successfully!");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid selection. Please select a valid row.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void Update_btn_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT PhoneNo, CustomerName, Email FROM Customer";

                using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                {
                    SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                    adapter.UpdateCommand = builder.GetUpdateCommand();

                    adapter.Update(dt);
                    dt.AcceptChanges();
                }
            }

            MessageBox.Show("Records updated successfully!");
            LoadData(textBox1.Text, textBox2.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
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

        private void Add_btn_Click(object sender, EventArgs e)
        {
         demo demo = new demo();
            demo.Show();
            
        }
    }
}
