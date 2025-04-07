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
    }
}