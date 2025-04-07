using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Pharmacy_Management
{
    public partial class Meds : Form
    {
        private string connectionString = @"Server=(localdb)\Mylocaldb; Database=Pharmacy_Management; Integrated Security=True;";
        private DataTable dt = new DataTable();
        private SqlDataAdapter adapter;

        public Meds()
        {
            InitializeComponent();

            textBox1.TextChanged += SearchTextChanged;
            textBox2.TextChanged += SearchTextChanged;
            textBox1.KeyPress += TextBox1_KeyPress;
            dataGridView1.CellEndEdit += DataGridView1_CellEndEdit;
        }

        private void Meds_Load(object sender, EventArgs e)
        {
            LoadMedicineData();
        }

        private void LoadMedicineData(string medidPrefix = "", string mednamePrefix = "")
        {
            string query = @"
                SELECT 
                    MedicineID, 
                    MedicineName, 
                    MedicineDosage, 
                    Price, 
                    Medicine_Batch_No, 
                    Medicine_Expiration_Date
                FROM Medicine
                WHERE (@MedidPrefix = '' OR MedicineID LIKE @MedidPrefix + '%')
                AND (@MednamePrefix = '' OR MedicineName LIKE @MednamePrefix + '%');";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MedidPrefix", medidPrefix);
                    cmd.Parameters.AddWithValue("@MednamePrefix", mednamePrefix);

                    try
                    {
                        conn.Open();
                        adapter = new SqlDataAdapter(cmd);

                        SqlCommandBuilder builder = new SqlCommandBuilder(adapter);
                        adapter.UpdateCommand = builder.GetUpdateCommand();
                        adapter.InsertCommand = builder.GetInsertCommand();
                        adapter.DeleteCommand = builder.GetDeleteCommand();

                        dt.Clear();
                        adapter.Fill(dt);
                        dt.AcceptChanges();
                        dataGridView1.DataSource = dt;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading data: " + ex.Message);
                    }
                }
            }
        }

        private void SearchTextChanged(object sender, EventArgs e)
        {
            LoadMedicineData(textBox1.Text.Trim(), textBox2.Text.Trim());
        }

        private void DataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataRow row = ((DataRowView)dataGridView1.Rows[e.RowIndex].DataBoundItem).Row;
                if (row.RowState == DataRowState.Unchanged)
                {
                    row.SetModified();
                }
            }
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (adapter == null || dt == null)
            {
                MessageBox.Show("No data loaded!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Ensure DataGridView commits any changes before updating the database
                dataGridView1.EndEdit();
                BindingContext[dt].EndCurrentEdit();

                // Get the changes in the DataTable
                DataTable changes = dt.GetChanges();
                if (changes == null)
                {
                    MessageBox.Show("No changes detected!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Prepare the SqlDataAdapter with proper commands
                    adapter.UpdateCommand = new SqlCommand(
                        @"UPDATE Medicine
                SET MedicineName = @MedicineName,
                    MedicineDosage = @MedicineDosage,
                    Price = @Price,
                    Medicine_Batch_No = @Medicine_Batch_No,
                    Medicine_Expiration_Date = @Medicine_Expiration_Date
                WHERE MedicineID = @MedicineID", conn);

                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@MedicineID", SqlDbType.Int));
                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@MedicineName", SqlDbType.NVarChar, 100));
                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@MedicineDosage", SqlDbType.NVarChar, 50));
                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Price", SqlDbType.Decimal));
                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Medicine_Batch_No", SqlDbType.NVarChar, 50));
                    adapter.UpdateCommand.Parameters.Add(new SqlParameter("@Medicine_Expiration_Date", SqlDbType.DateTime));

                    // Update the records in the database
                    int updatedRows = adapter.Update(changes);

                    // After update, accept changes
                    dt.AcceptChanges();
                }

                // Reload the updated data from the database
                LoadMedicineData(textBox1.Text.Trim(), textBox2.Text.Trim());

                // MessageBox is removed here
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Database error: {sqlEx.Message}", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating database: {ex.Message}", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
