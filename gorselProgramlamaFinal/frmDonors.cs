using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gorselProgramlamaFinal
{
    public partial class frmDonors : Form
    {
        public frmDonors()
        {
            InitializeComponent();
            LoadData();
        }

        // Verileri DataGridView'e yüklemek
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

        // DataGridView stilini ayarlamak
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

        // Ana ekran butonuna tıklanması
        private void button5_Click(object sender, EventArgs e)
        {
            frmHome home = new frmHome();
            home.Show();
            this.Hide();
        }

        // Yeni bağışçı eklemek için buton
        private void btnEkle_Click(object sender, EventArgs e)
        {
            string donorAd = textBox2.Text;
            string donorSoyad = textBox3.Text;
            string donorEmail = textBox4.Text;
            string donorCinsiyet = comboBox1.SelectedItem.ToString();
            string donorKanGrubu = comboBox2.SelectedItem.ToString();
            string donorTelefon = textBox5.Text;
            string donorAdres = textBox6.Text;

            // Bağışçı verilerini veritabanına eklemek için SQL sorgusu
            string connectionString = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO blood_bagis (donor_ad, donor_soyad, donor_email, donor_cinsiyet, donor_kangrubu, donor_telefon, donor_adres) " +
                                   "VALUES (@donorAd, @donorSoyad, @donorEmail, @donorCinsiyet, @donorKanGrubu, @donorTelefon, @donorAdres)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@donorAd", donorAd);
                    cmd.Parameters.AddWithValue("@donorSoyad", donorSoyad);
                    cmd.Parameters.AddWithValue("@donorEmail", donorEmail);
                    cmd.Parameters.AddWithValue("@donorCinsiyet", donorCinsiyet);
                    cmd.Parameters.AddWithValue("@donorKanGrubu", donorKanGrubu);
                    cmd.Parameters.AddWithValue("@donorTelefon", donorTelefon);
                    cmd.Parameters.AddWithValue("@donorAdres", donorAdres);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Bağışçı başarıyla eklendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Listeyi güncelle
                    }
                    else
                    {
                        MessageBox.Show("Bağışçı eklenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            // TextBox ve ComboBox'lardan alınan veriler
            string donorId = textBox1.Text; // Seçili bağışçının ID'si
            string donorAd = textBox2.Text;
            string donorSoyad = textBox3.Text;
            string donorEmail = textBox4.Text;
            string donorCinsiyet = comboBox1.SelectedItem.ToString();
            string donorKanGrubu = comboBox2.SelectedItem.ToString();
            string donorTelefon = textBox5.Text;
            string donorAdres = textBox6.Text;

            // Veritabanı bağlantısı
            string connectionString = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Güncelleme sorgusu
                    string query = "UPDATE blood_bagis SET donor_ad = @donorAd, donor_soyad = @donorSoyad, donor_email = @donorEmail, " +
                                   "donor_cinsiyet = @donorCinsiyet, donor_kangrubu = @donorKanGrubu, donor_telefon = @donorTelefon, " +
                                   "donor_adres = @donorAdres WHERE donor_id = @donorId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@donorId", donorId);
                    cmd.Parameters.AddWithValue("@donorAd", donorAd);
                    cmd.Parameters.AddWithValue("@donorSoyad", donorSoyad);
                    cmd.Parameters.AddWithValue("@donorEmail", donorEmail);
                    cmd.Parameters.AddWithValue("@donorCinsiyet", donorCinsiyet);
                    cmd.Parameters.AddWithValue("@donorKanGrubu", donorKanGrubu);
                    cmd.Parameters.AddWithValue("@donorTelefon", donorTelefon);
                    cmd.Parameters.AddWithValue("@donorAdres", donorAdres);

                    // Sorguyu çalıştır ve etkilenen satır sayısını kontrol et
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Bağışçı bilgileri başarıyla güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Listeyi güncelle
                    }
                    else
                    {
                        MessageBox.Show("Bağışçı bilgileri güncellenemedi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Seçilen satırdaki hücrelerin verilerini textboxlara aktar
            if (e.RowIndex >= 0) // Satır seçildiğinden emin olalım
            {
                // Bağışçı bilgilerini al ve textboxlara aktar
                textBox1.Text = dgv.Rows[e.RowIndex].Cells["donor_id"].Value.ToString();
                textBox2.Text = dgv.Rows[e.RowIndex].Cells["donor_ad"].Value.ToString();
                textBox3.Text = dgv.Rows[e.RowIndex].Cells["donor_soyad"].Value.ToString();
                textBox4.Text = dgv.Rows[e.RowIndex].Cells["donor_email"].Value.ToString();

                // ComboBox'lara değerleri aktarma
                comboBox1.SelectedItem = dgv.Rows[e.RowIndex].Cells["donor_cinsiyet"].Value.ToString();
                comboBox2.SelectedItem = dgv.Rows[e.RowIndex].Cells["donor_kangrubu"].Value.ToString();

                textBox5.Text = dgv.Rows[e.RowIndex].Cells["donor_telefon"].Value.ToString();
                textBox6.Text = dgv.Rows[e.RowIndex].Cells["donor_adres"].Value.ToString();
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            // Silmek için seçilen bağışçının ID'sini al
            string donorId = textBox1.Text;

            if (string.IsNullOrEmpty(donorId))
            {
                MessageBox.Show("Lütfen silmek istediğiniz bağışçıyı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Silme işlemi için veritabanı bağlantısı
            string connectionString = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Bağışçıyı silme sorgusu
                    string query = "DELETE FROM blood_bagis WHERE donor_id = @donorId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@donorId", donorId);

                    // Sorguyu çalıştır ve etkilenen satır sayısını kontrol et
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Bağışçı başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData(); // Listeyi güncelle
                    }
                    else
                    {
                        MessageBox.Show("Silme işlemi başarısız oldu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            // TextBox'ları temizle
            textBox1.Clear(); // donor_id
            textBox2.Clear(); // donor_ad
            textBox3.Clear(); // donor_soyad
            textBox4.Clear(); // donor_email
            textBox5.Clear(); // donor_telefon
            textBox6.Clear(); // donor_adres

            // ComboBox'ları temizle
            comboBox1.SelectedIndex = -1; // donor_cinsiyet
            comboBox2.SelectedIndex = -1; // donor_kangrubu

            // Formu sıfırlamak, boş hale getirmek için başka işlemler ekleyebilirsiniz
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            // Kullanıcının arama metnini al
            string searchText = textBox7.Text.Trim();

            // Arama metni boşsa tüm verileri yükle
            if (string.IsNullOrEmpty(searchText))
            {
                LoadData();
            }
            else
            {
                // Arama yapmak için veritabanı bağlantısı
                string connectionString = "server=localhost;user=root;password=19661972;database=bloodbank;";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        // Arama sorgusu (LIKE ile metni içeren satırları getirir)
                        string query = "SELECT * FROM blood_bagis WHERE donor_ad LIKE @searchText OR donor_soyad LIKE @searchText OR donor_email LIKE @searchText";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        dgv.DataSource = dt; // DataGridView'i filtrelenmiş verilerle doldur

                        StilUygula(); // DataGridView stilini uygula
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Veritabanı hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

}
        

    

