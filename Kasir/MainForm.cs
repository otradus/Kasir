using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Kasir
{
    delegate void Function();   // a simple delegate for marshalling calls from event handlers to the GUI thread

    public partial class MainForm : Form
    {
        public static bool verified = false;
        public static double total = 0;
        public static DataTable dt = new DataTable();

        public MainForm()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }


        private DPFP.Template Template;

        private void MainForm_Load(object sender, EventArgs e)
        {
            CaptureForm Verifier = new CaptureForm();
            //Verifier.Verify(Template);
            label1.Text = DateTime.Now.ToShortDateString();
            timer1.Start();
            App.formatDataGridView(dataGridView1);
            App2.DoubleBuffered(dataGridView1, true);

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            AddPembayaran(listBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToLongTimeString();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            BayarForm bayar = new BayarForm();
            bayar.ShowDialog();
        }

        private void MainForm_MouseClick(object sender, MouseEventArgs e)
        {
            RefreshTables();
        }

        public void RefreshTables()
        {
            listBox2.Items.Clear();

            DataTable items = App.executeReader("SELECT Faktur FROM kaos.penjualancompact WHERE Tanggal = '28/05/2016'");
            foreach (DataRow row in items.Rows)
            {
                listBox2.Items.Add(row[0]);
            }

        }

        public static object[] bayarlist;
        public void AddPembayaran(ListBox lstbx)
        {
            if (lstbx.SelectedIndex != -1)
            {
                //App.loadTableFromList(dataGridView1, dt, ("SELECT Faktur, Kode, Nama, Jumlah, Harga, Subtotal FROM kaos.penjualan WHERE Faktur = '" + lstbx.SelectedItem.ToString() + "'"));             
                App.Testing("SELECT Faktur, Kode, Nama, Jumlah, Harga, Subtotal FROM kaos.penjualan WHERE Faktur = '" + lstbx.SelectedItem.ToString() + "'", dataGridView1);
                //foreach (DataRow row in dt.Rows)
                //{
                //    dataGridView1.Rows.Add("Kaos", row[0], row[1], row[2], row[3], row[4], row[5]);
                //}
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }
    }
}