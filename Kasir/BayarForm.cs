using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kasir
{
    public partial class BayarForm : Form
    {
        public BayarForm()
        {
            InitializeComponent();
        }

        public void BayarAdd(double sum)
        {
            double bayar = App.moneytodouble(textBox2.Text);
            textBox2.Text = App.doubletomoney(bayar + sum);
        }

        private void BayarForm_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            BayarAdd(100000);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BayarAdd(50000);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            BayarAdd(20000);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            BayarAdd(10000);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            BayarAdd(5000);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            BayarAdd(2000);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            BayarAdd(1000);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            BayarAdd(500);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            BayarAdd(100);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (groupBox2.Enabled == false)
            {
                groupBox2.Visible = true;
                groupBox2.Enabled = true;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                groupBox1.Enabled = false;
            }
            else
            {
                groupBox2.Visible = false;
                groupBox2.Enabled = false;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                groupBox1.Enabled = true;
            }
        }
    }
}
