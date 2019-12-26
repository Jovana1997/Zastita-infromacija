using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Klijent
{
    public partial class Form1 : Form
    {
        ServiceReference1.Service1Client obj = new ServiceReference1.Service1Client();
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            ServiceReference1.Korisnik k = new ServiceReference1.Korisnik();
            k.Username = tbUsername.Text;
            k.Password = tbPassword.Text;
            obj.Login(k);
            MessageBox.Show("Ok");
        }
    }
}
