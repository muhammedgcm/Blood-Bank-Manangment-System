using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.Drawing;

namespace gorselProgramlamaFinal
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }

        private void frmHome_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // 🔄 Tüm verileri yükler
        private void LoadData()
        {
            string connectionString = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM blood_bagis";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgv.DataSource = dt;
                    StilUygula();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri çekme hatası: " + ex.Message);
                }
            }
        }
        private void StilUygula()
        {
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.RowTemplate.Height = 30;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkRed;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        }


       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FilterData(textBox1.Text);
        }

       
        private void FilterData(string arama)
        {
            string connectionString = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT * FROM blood_bagis 
                                     WHERE donor_ad LIKE @arama 
                                     OR donor_soyad LIKE @arama 
                                     OR donor_email LIKE @arama 
                                     OR donor_cinsiyet LIKE @arama 
                                     OR donor_kangrubu LIKE @arama 
                                     OR donor_telefon LIKE @arama 
                                     OR donor_adres LIKE @arama";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@arama", "%" + arama + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgv.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Arama sırasında hata: " + ex.Message);
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            frmUsers users = new frmUsers();
            users.Show();
            this.Hide();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            frmDonors donors = new frmDonors();
            donors.Show();
            this.Hide();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Oturum Kapatılıyor.", "Sistem Bilgisi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            frmLogin login = new frmLogin();
            login.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            frmStock stock = new frmStock();
            stock.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmKanKayıt kayit = new frmKanKayıt();
            kayit.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmRandevu randevu = new frmRandevu();
            randevu.Show();
            this.Hide();
        }
    }
}
