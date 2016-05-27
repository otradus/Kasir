using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kaos
{
    class App
    {
        public static string toko = Environment.GetCommandLineArgs()[1].ToString();
        public static bool admin = Convert.ToBoolean(Environment.GetCommandLineArgs()[2].ToString());
        public static string printer = Environment.GetCommandLineArgs()[3].ToString();


        public static string getConnectionString()
        {
            string[] settings = System.IO.File.ReadAllLines(@"C:\test\settingskaos.ini");
            MySqlConnectionStringBuilder connstring = new MySqlConnectionStringBuilder();
            connstring.Server = settings[0];
            connstring.UserID = settings[1];
            connstring.Password = settings[2];
            connstring.Database = Environment.GetCommandLineArgs()[1].ToString();


            return connstring.ToString();
        }

        public static DataTable executeReader(string query)

        {
            DataTable results = new DataTable("Results");
            using (MySqlConnection connection = new MySqlConnection(App.getConnectionString()))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();

                    using (MySqlDataReader reader = command.ExecuteReader())
                        results.Load(reader);
                }
            }
            return results;
        }

        public static void executeNonQuery(string query)
        {
            MySqlConnection conn = new MySqlConnection(getConnectionString());
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
        }


        public static object executeScalar(string query)
        {
            MySqlConnection conn = new MySqlConnection(getConnectionString());
            object result;
            try
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                result = cmd.ExecuteScalar();
                //if (result != null){int r = Convert.ToInt32(result);Console.WriteLine("Number of countries in the World database is: " + r);}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result = null;
            }

            conn.Close();
            return result;
        }

        //        public static MySqlConnection conn = new MySqlConnection(getConnectionString());


        public static void loadTable(DataGridView dtv, string search)
        {
            MySqlConnection conn = new MySqlConnection(getConnectionString());
            MySqlCommand command1 = new MySqlCommand(search, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command1);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            //    foreach (DataRow dr in dt.Rows)
            //  {
            //    dr["Harga"] = ;
            //}

            dtv.DataSource = dt;
            if (dtv.Columns["Harga"] != null) { dtv.Columns["Harga"].DefaultCellStyle.Format = "c"; }
            if (dtv.Columns["HargaBeli"] != null) { dtv.Columns["HargaBeli"].DefaultCellStyle.Format = "c"; }
            if (dtv.Columns["Subtotal"] != null) { dtv.Columns["Subtotal"].DefaultCellStyle.Format = "c"; }
            //           dtv.Columns["Harga"].DefaultCellStyle.Format = "c";
            //         dtv.Columns["HargaBeli"].DefaultCellStyle.Format = "c";
            dtv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        public static void loadComboBox(ComboBox cmbx, string search)
        {
            MySqlConnection conn = new MySqlConnection(getConnectionString());
            MySqlCommand command1 = new MySqlCommand(search, conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command1);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            foreach (DataRow dr in dt.Rows)
            {
                cmbx.Items.Add(dr["Merk"]);
            }

            //            cmbx.DataSource = dt;
        }

        public static string mysqlcurrency(string str)
        {
            return "CONCAT('Rp', FORMAT(" + str + ", 0))";
        }

        public static string strtomoney(string str)
        {
            //return "Rp" + str.Replace(",", ".");
            try
            {
                return String.Format("{0:C0}", Convert.ToInt32(str));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return "";
            }
        }

        public static double moneytodouble(string str)
        {
            try
            {
                str = str.Replace("Rp", "");
                str = str.Replace(".", "");
                return Convert.ToDouble(str);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                return 0;
            }
        }

        public static string doubletomoney(object amount)
        {
            string money = Convert.ToString(amount);
            return strtomoney(money);
        }

        public static void formatDataGridView(DataGridView dgv)
        {
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToOrderColumns = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.RowHeadersVisible = false;

        }

        public static int cInt(object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static double cDouble(object obj)
        {
            return Convert.ToDouble(obj);
        }

        public static string stripMoney(string money)
        {
            money = money.Replace("Rp", "");
            money = money.Replace(".", "");
            return money;
        }

        public static void shellCommand(string cmdtext)
        {
            var proc1 = new System.Diagnostics.ProcessStartInfo();
            proc1.UseShellExecute = true;

            proc1.WorkingDirectory = @"C:\Windows\System32";

            proc1.FileName = @"C:\Windows\System32\cmd.exe";
            //proc1.Verb = "runas";
            proc1.Arguments = "/c " + cmdtext;
            proc1.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            System.Diagnostics.Process.Start(proc1);
        }


        public static void printPenjualan(string faktur, string sales)
        {
            DateTime tgl = DateTime.Now;
            DataTable rs = executeReader("SELECT * FROM penjualan WHERE Faktur = '" + faktur + "'");

            //PRINT INVOICE
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Convert.ToChar(27) + "a1" + Convert.ToChar(27) + "!4" + "Toko B.H. [KAOS]");
            sb.AppendLine("Tasikmalaya");
            sb.AppendLine(Convert.ToChar(27) + "@");
            sb.AppendLine("Faktur: " + faktur + " Sales: " + sales);
            sb.AppendLine("Tanggal: " + tgl.ToShortDateString() + " Jam: " + tgl.ToShortTimeString());
            sb.AppendLine("");
            sb.AppendLine("========================================");

            double total = 0;
            int qty = 0;
            int i = 1;
            foreach (DataRow row in rs.Rows)
            {
                sb.AppendLine(Left((i.ToString() + ". " + row[3].ToString()), 40));
                sb.AppendLine("   " + strtomoney(row[5].ToString()) + Convert.ToChar(9) + "x" + Convert.ToChar(9) + row[4].ToString() + Convert.ToChar(9) + strtomoney(row[6].ToString()));
                total += Convert.ToDouble(row[6]);
                qty += Convert.ToInt32(row[4]);
                i += 1;
            }

            sb.AppendLine("-----------------------------------------");
            sb.AppendLine("   Qty: " + qty.ToString() + Convert.ToChar(9) + Convert.ToChar(9) + " TOTAL: " + strtomoney(total.ToString()));
            sb.AppendLine("");

            sb.AppendLine(Convert.ToChar(29) + "VA0");


            System.IO.File.WriteAllText(@"C:\test\invoicekaos.txt", sb.ToString());

            shellCommand("copy c:\\test\\invoicekaos.txt " + printer);

        }

        public static void printPembelian(string nota, string user)
        {
            DateTime tgl = DateTime.Now;
            DataTable rs = executeReader("SELECT Kode, Nama, Jumlah FROM pembelian WHERE Nota = '" + nota + "'");

            //PRINT INVOICE
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Convert.ToChar(27) + "a1" + Convert.ToChar(27) + "!4" + "PEMBELIAN [KAOS]");
            sb.AppendLine(Convert.ToChar(27) + "@");
            sb.AppendLine("Nota: " + nota + " User: " + user);
            sb.AppendLine("Tanggal: " + tgl.ToShortDateString() + " Jam: " + tgl.ToShortTimeString());
            sb.AppendLine("");
            sb.AppendLine("========================================");

            int qty = 0;
            foreach (DataRow row in rs.Rows)
            {
                sb.AppendLine(row[0].ToString() + Convert.ToChar(9) + row[1].ToString() + Convert.ToChar(9) + row[2].ToString());
                qty += Convert.ToInt32(row[2]);
            }

            sb.AppendLine("-----------------------------------------");
            sb.AppendLine("   " + Convert.ToChar(9) + Convert.ToChar(9) + Convert.ToChar(9) + "Qty: " + qty.ToString());
            sb.AppendLine("");

            sb.AppendLine(Convert.ToChar(29) + "VA0");


            System.IO.File.WriteAllText(@"C:\test\invoicekaospembelian.txt", sb.ToString());

            shellCommand("copy c:\\test\\invoicekaospembelian.txt " + printer);

        }

        public static string Left(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }
    }
}
