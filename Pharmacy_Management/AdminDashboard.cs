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
    public partial class AdminDashboard : Form
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void OpenPanel2(Form childForm)
        {
            panel1.Controls.Clear();                
            childForm.TopLevel = false;           
            childForm.FormBorderStyle = FormBorderStyle.None; 
            childForm.Dock = DockStyle.Fill;         
            panel1.Controls.Add(childForm);        
            childForm.Show();
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {Meds meds = new Meds();

            OpenPanel2(meds);

        }


        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            Login_page login_Page = new Login_page();
            OpenPanel2(login_Page);
        }

        private void button4_Click(object sender, EventArgs e)
        {

            New_user new_User = new New_user();
          
            OpenPanel2(new_User);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Customers customers = new Customers();
            OpenPanel2(customers);

        }

        private void button3_Click(object sender, EventArgs e)

        {
            Billing billing = new Billing();
            OpenPanel2(billing);
        }

        private void Inventry_Click(object sender, EventArgs e)
        {
            Inventary inventory = new Inventary();

            OpenPanel2(inventory);
        }

        private void Inventry_Click_1(object sender, EventArgs e)
        {
            Inventary inventary = new Inventary();
            OpenPanel2(inventary);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Sales sales = new Sales();
            OpenPanel2(sales);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Customers customer = new Customers();   
            OpenPanel2(customer);
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Billing billing1 = new Billing();   
            OpenPanel2(billing1);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Meds meds = new Meds();
            OpenPanel2(meds);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            New_user new_User = new New_user();
            OpenPanel2(new_User);
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            Login_page login_page = new Login_page();
          this.Hide();
            login_page.Show();
           
        }
    }
}
