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
    public partial class Inventary : Form
    {
        private string connectionString = @"Server=(localdb)\Mylocaldb; Database=Pharmacy_Management; Integrated Security=True;";

        public Inventary()
        {
            InitializeComponent();
            InitializeInventoryGridView();
            LoadInventoryData(); 
        }

        private void InitializeInventoryGridView()
        {
           
        }

        private DataTable dt = new DataTable();
        private SqlDataAdapter adapter;


        private void LoadInventoryData(string inventoryId = "", string medicineId = "")
        {
            string query = @"SELECT InventoryID, MedicineID, Quantity, BatchNo, InvoiceNumber, TrackingNumber
                             FROM Inventory
                             WHERE (@InventoryID = '' OR InventoryID LIKE @InventoryID + '%')
                             AND (@MedicineID = '' OR MedicineID LIKE @MedicineID + '%');";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@InventoryID", inventoryId);
                cmd.Parameters.AddWithValue("@MedicineID", medicineId);

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string inventoryId = textBox3.Text.Trim(); 
            string medicineId = textBox4.Text.Trim(); 

            LoadInventoryData(inventoryId, medicineId);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            LoadInventoryData(textBox3.Text.Trim(), textBox4.Text.Trim());
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            LoadInventoryData(textBox3.Text.Trim(), textBox4.Text.Trim());
        }

        private void Inventory_Load(object sender, EventArgs e)
        {
            LoadInventoryData(); 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e) //delete function
        {
            // Check if data is loaded
            if (adapter == null || dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("No data loaded or adapter not initialized!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if user has selected any row
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirm deletion
            DialogResult result = MessageBox.Show("Are you sure you want to delete the selected record(s)?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Mark rows for deletion in DataTable
                    foreach (DataGridViewRow selectedRow in dataGridView1.SelectedRows)
                    {
                        if (!selectedRow.IsNewRow)
                        {
                            DataRow row = ((DataRowView)selectedRow.DataBoundItem).Row;
                            row.Delete();
                        }
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        // Set the DeleteCommand for the adapter
                        adapter.DeleteCommand = new SqlCommand(
                            @"DELETE FROM Medicine WHERE MedicineID = @MedicineID", conn);
                        adapter.DeleteCommand.Parameters.Add("@MedicineID", SqlDbType.Int, 0, "MedicineID");

                        // Apply deletions to the database
                        adapter.Update(dt);
                        dt.AcceptChanges();
                    }

                    // Reload the updated data
                    LoadInventoryData(textBox1.Text.Trim(), textBox2.Text.Trim());

                    MessageBox.Show("Record(s) deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (SqlException sqlEx)
                {
                    MessageBox.Show($"Database error: {sqlEx.Message}", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting record(s): {ex.Message}", "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}

    
