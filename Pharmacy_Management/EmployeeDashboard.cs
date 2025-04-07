using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pharmacy_Management
{
    public partial class EmployeeDashboard : Form
    {
        public EmployeeDashboard()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.Sizable; // Ensure the form is resizable
        }

        // Method to open a child form inside panel3
        private void OpenPanel(Form childForm)
        {
            // Clear existing controls from panel3
            panel3.Controls.Clear();

            // Set child form properties for embedding
            childForm.TopLevel = false;                // Embed form into a parent control
            childForm.FormBorderStyle = FormBorderStyle.None; // No border for child form
            childForm.Dock = DockStyle.Fill;           // Fill the panel completely

            // Add the child form to panel3 and display it
            panel3.Controls.Add(childForm);
            childForm.Show();
        }

        // Logout and go back to Login page
        private void button5_Click(object sender, EventArgs e)
        {
            Login_page login_Page = new Login_page();
            login_Page.Show();
            this.Close(); // Close current dashboard
        }

        // Load Meds form in panel3
        private void button1_Click_1(object sender, EventArgs e)
        {
           
        }

        // Load Customers form in panel3
        private void button2_Click(object sender, EventArgs e)
        {
            Customers customers = new Customers();
            OpenPanel(customers);
        }

        // Load Inventory form in panel3
        private void Inventry_Click(object sender, EventArgs e)
        {
            Inventary inventory = new Inventary();
            OpenPanel(inventory);
        }

        // Load Billing form in panel3
        private void button3_Click(object sender, EventArgs e)
        {
            Billing billing = new Billing();
            OpenPanel(billing);
        }

        private void EmployeeDashboard_Load(object sender, EventArgs e)
        {
            // Ensure form and panel are resizable and responsive
            this.FormBorderStyle = FormBorderStyle.Sizable;
            panel3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        // Ignore placeholder methods unless needed
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel1_Paint_1(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            Meds meds = new Meds();
            OpenPanel(meds);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Customers customers = new Customers();
            OpenPanel(customers);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Billing billing1 = new Billing();
            OpenPanel(billing1);
        }

        private void Inventry_Click_1(object sender, EventArgs e)
        {
            Inventary inventary1 = new Inventary();
            OpenPanel(inventary1);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Hide();

            Login_page login_Page = new Login_page();
            login_Page.Show();
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Sales sales = new Sales();  
            OpenPanel(sales);
        }
    }
}