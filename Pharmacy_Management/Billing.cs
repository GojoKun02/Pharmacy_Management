using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using static Pharmacy_Management.Login_page;

namespace Pharmacy_Management
{
    public partial class Billing : Form
    {
        private string connectionString = @"Server=(localdb)\Mylocaldb; Database=Pharmacy_Management; Integrated Security=True;";
        private decimal totalPrice = 0;

        public Billing()
        {
            InitializeComponent();
            InitializeDataGridView();
            textBox1.KeyPress += TextBox1_KeyPress;
            textBox3.KeyPress += TextBox3_KeyPress;
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add("PhoneNo", "Phone Number");
            dataGridView1.Columns.Add("MedicineID", "Medicine ID");
            dataGridView1.Columns.Add("MedicineName", "Medicine Name");
            dataGridView1.Columns.Add("Quantity", "Quantity");
            dataGridView1.Columns.Add("PricePerUnit", "Price Per Unit");
            dataGridView1.Columns.Add("TotalPrice", "Total Price");

            UpdateTotalPriceDisplay();
        }

        private void AddMedicine(string phoneNo, string medicineName, int quantity)
        {
            string getMedicineQuery = @"SELECT MedicineID, MedicineName, Price FROM Medicine WHERE MedicineName = @MedicineName;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(getMedicineQuery, conn))
            {
                cmd.Parameters.AddWithValue("@MedicineName", medicineName);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int medicineID = Convert.ToInt32(reader["MedicineID"]);
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        decimal totalMedicinePrice = price * quantity;
                        totalPrice += totalMedicinePrice;

                        reader.Close();

                        dataGridView1.Rows.Add(phoneNo, medicineID, medicineName, quantity, price, totalMedicinePrice);

                        UpdateTotalPriceDisplay();

                        MessageBox.Show($"Added: {medicineName} (Qty: {quantity}) - Total: {totalPrice:C}");
                    }
                    else
                    {
                        MessageBox.Show("Medicine not found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private int GetSaleID()
        {
            int saleID = 1; // Default first ID
            string getSaleIDQuery = "SELECT ISNULL(MAX(SaleID), 0) + 1 FROM Sale;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(getSaleIDQuery, conn))
            {
                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    saleID = (result != DBNull.Value && result != null) ? Convert.ToInt32(result) : 1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching SaleID: " + ex.Message);
                }
            }

            return saleID;
        }

        private void SaveBillToDatabase()
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("No items to save.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string phoneNo = dataGridView1.Rows[0].Cells["PhoneNo"].Value.ToString();
                    string userRole = Session.Role ?? "Unknown";

                    // **Step 1: Get the Next SaleID**
                    int saleID;
                    string getSaleIDQuery = "SELECT ISNULL(MAX(SaleID), 0) + 1 FROM Sale;";
                    using (SqlCommand saleIdCmd = new SqlCommand(getSaleIDQuery, conn))
                    {
                        saleID = Convert.ToInt32(saleIdCmd.ExecuteScalar());  // Fetch new SaleID
                    }

                    // **Step 2: Insert Each Medicine with the Same SaleID**
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int medicineID = Convert.ToInt32(row.Cells["MedicineID"].Value);
                        int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                        decimal totalAmount = Convert.ToDecimal(row.Cells["TotalPrice"].Value);

                        string insertSaleQuery = @"INSERT INTO Sale (SaleID, PhoneNo, MedicineID, Quantity, TotalAmount, SaleDate, UserID, Role) 
                                           VALUES (@SaleID, @PhoneNo, @MedicineID, @Quantity, @TotalAmount, GETDATE(), @UserID, @Role);";

                        using (SqlCommand cmd = new SqlCommand(insertSaleQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@SaleID", saleID);  // Using the same SaleID
                            cmd.Parameters.AddWithValue("@PhoneNo", phoneNo);
                            cmd.Parameters.AddWithValue("@MedicineID", medicineID);
                            cmd.Parameters.AddWithValue("@Quantity", quantity);
                            cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                            cmd.Parameters.AddWithValue("@UserID", Session.UserID);
                            cmd.Parameters.AddWithValue("@Role", userRole);
                            cmd.ExecuteNonQuery();
                        }

                        // **Step 3: Update Inventory**
                        string updateInventoryQuery = @"UPDATE Inventory 
                                               SET Quantity = Quantity - @Quantity 
                                               WHERE MedicineID = @MedicineID;";

                        using (SqlCommand updateCmd = new SqlCommand(updateInventoryQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@Quantity", quantity);
                            updateCmd.Parameters.AddWithValue("@MedicineID", medicineID);
                            updateCmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show($"Bill saved successfully with Sale ID: {saleID}");

                    // **Clear Data After Saving**
                    dataGridView1.Rows.Clear();
                    totalPrice = 0;
                    UpdateTotalPriceDisplay();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while saving bill: " + ex.Message);
                }
            }
        }



        private void UpdateTotalPriceDisplay()
        {
            labelTotalPrice.Text = $"Total: {totalPrice:C}";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string phoneNo = textBox1.Text.Trim();
            string medicineName = textBox2.Text.Trim();
            int quantity;

            if (string.IsNullOrEmpty(phoneNo) || string.IsNullOrEmpty(medicineName) || !int.TryParse(textBox3.Text, out quantity))
            {
                MessageBox.Show("Please enter valid Phone Number, Medicine Name, and Quantity.");
                return;
            }

            AddMedicine(phoneNo, medicineName, quantity);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            totalPrice = 0;
            UpdateTotalPriceDisplay();
            MessageBox.Show("Cart cleared.");
        }

        private void Billing_Load(object sender, EventArgs e)
        {
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void Total_Click(object sender, EventArgs e)
        {
        }

        private void Print_bill_Click(object sender, EventArgs e)
        {
            SaveBillToDatabase();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

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
            TextBox textBox = sender as TextBox;
            if (textBox != null && char.IsDigit(e.KeyChar) && textBox.Text.Length >= 10)
            {
                e.Handled = true; // Prevent further input if length exceeds 10
            }
        }
        private void TextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits and control characters (e.g., backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Block non-numeric characters
            }
            TextBox textBox = sender as TextBox;
            if (textBox != null && char.IsDigit(e.KeyChar) && textBox.Text.Length >= 10)
            {
                e.Handled = true; // Prevent further input if length exceeds 10
            }
        }
    }
}
