using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace Pharmacy_Management
{
    public partial class Sales : Form
    {
        private string connectionString = @"Server=(localdb)\Mylocaldb; Database=Pharmacy_Management; Integrated Security=True;";
        private DataTable dt = new DataTable();
        private DataView dv;

        public Sales()
        {
            InitializeComponent();
            // Hook up the event for the Search button click
            Search_btn.Click += Search_btn_Click;

            // Set default sorting to "Recent to Old"
            comboBox1.SelectedIndex = 0;
        }

    

        private void LoadSalesData(string startDate, string sortOrder)
        {
            // Correct SQL query
            string query = @"SELECT
    SaleID, 
    PhoneNo, 
    STRING_AGG(CAST(MedicineID AS VARCHAR(MAX)), ', ') AS MedicineIDs, 
    SUM(TotalAmount) AS TotalAmount,
            MIN(SaleDate) AS SaleDate,
            UserID,
    Role
FROM Sale
WHERE(@StartDate IS NULL OR SaleDate >= @StartDate)
GROUP BY SaleID, PhoneNo, UserID, Role
ORDER BY SaleDate DESC;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                // Set the start date parameter (if any)
                cmd.Parameters.AddWithValue("@StartDate", string.IsNullOrEmpty(startDate) ? (object)DBNull.Value : startDate);

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    dt.Clear();
                    adapter.Fill(dt);
                    dt.AcceptChanges();

                    // Bind the data to DataGridView
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading sales: " + ex.Message);
                }
            }
        }
    


    private void ApplyFilters()
        {
            string selectedDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string sortOrder = comboBox1.SelectedItem.ToString() == "Old to Recent" ? "ASC" : "DESC";

            // Apply filter on DataView
            string filter = $"CONVERT(SaleDate, 'System.String') LIKE '{selectedDate}%'";
            dv.RowFilter = filter;
            dv.Sort = $"SaleDate {sortOrder}";

            // Call method to populate medicines for each sale
            AddMedicinesToSales();
        }

        private void AddMedicinesToSales()
        {
            foreach (DataRow row in dv.ToTable().Rows)
            {
                string saleID = row["SaleID"].ToString();
                string medicineQuery = @"
                    SELECT m.MedicineName
                    FROM Sale s
                    JOIN Medicine m ON s.MedicineID = m.MedicineID
                    WHERE s.SaleID = @SaleID;
                ";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(medicineQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@SaleID", saleID);
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        string medicines = "";
                        while (reader.Read())
                        {
                            medicines += reader.GetString(0) + ", ";
                        }
                        if (medicines.Length > 0) medicines = medicines.Substring(0, medicines.Length - 2); // Remove trailing comma
                        row["Medicines"] = medicines; // Add medicines to the DataTable
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading medicines: " + ex.Message);
                    }
                }
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e) => ApplyFilters();

        private void Delete_btn_Click(object sender, EventArgs e)
        {
           
        }

        private void Search_btn_Click(object sender, EventArgs e)
        {
            // Get the selected date from DateTimePicker
            string selectedDate = dateTimePicker1.Value.ToString("yyyy-MM-dd");

            // Get the selected sort order from ComboBox
            string sortOrder = comboBox1.SelectedItem.ToString() == "Old to Recent" ? "ASC" : "DESC";

            // Call LoadSalesData with the selected filters
            LoadSalesData(selectedDate, sortOrder);
        }

        private void Delete_btn_Click_1(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string saleID = dataGridView1.SelectedRows[0].Cells["SaleID"].Value.ToString();

                DialogResult result = MessageBox.Show("Are you sure you want to delete this sale?",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "DELETE FROM Sale WHERE SaleID = @SaleID";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@SaleID", saleID);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Remove from DataTable instead of reloading from DB
                    DataRow[] rows = dt.Select($"SaleID = {saleID}");
                    foreach (var row in rows) dt.Rows.Remove(row);
                    dt.AcceptChanges();

                    MessageBox.Show("Sale deleted successfully!");
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }
        }

        private void Update_btn_Click(object sender, EventArgs e)
        {

        }
    }
    }

