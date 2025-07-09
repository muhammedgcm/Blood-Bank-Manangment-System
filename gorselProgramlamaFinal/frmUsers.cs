using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace gorselProgramlamaFinal
{
    public partial class frmUsers : Form
    {
        public frmUsers()
        {
            InitializeComponent();
        }

       
        string connectionString = "Server=localhost;Database=bloodbank;Uid=root;Pwd=19661972;";

        private void button1_Click(object sender, EventArgs e)
        {
            frmHome home = new frmHome();
            home.Show();
            this.Hide();
        }

        
        private void btnEkle_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "INSERT INTO blood_personel (personel_adsoyad, personel_email, personel_kullaniciadi, personel_parola, personel_telefon, personel_adres) " +
                                   "VALUES (@adsoyad, @email, @kullaniciadi, @parola, @telefon, @adres)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@adsoyad", txtName.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@kullaniciadi", txtKullanici.Text);
                    cmd.Parameters.AddWithValue("@parola", txtParola.Text);
                    cmd.Parameters.AddWithValue("@telefon", txtTel.Text);
                    cmd.Parameters.AddWithValue("@adres", txtAdres.Text);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Personel başarıyla eklendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Temizle();
                    }
                    else
                    {
                        MessageBox.Show("Ekleme başarısız!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        
        private void Temizle()
        {
            txtName.Text = "";
            txtEmail.Text = "";
            txtKullanici.Text = "";
            txtParola.Text = "";
            txtTel.Text = "";
            txtAdres.Text = "";
            txtID.Text = "";
        }
        private void PersonelListele()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM blood_personel";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgv.DataSource = dt;
                    StilUygula();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "UPDATE blood_personel SET " +
                                   "personel_adsoyad = @adsoyad, " +
                                   "personel_email = @email, " +
                                   "personel_kullaniciadi = @kullaniciadi, " +
                                   "personel_parola = @parola, " +
                                   "personel_telefon = @telefon, " +
                                   "personel_adres = @adres " +
                                   "WHERE personel_id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@adsoyad", txtName.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@kullaniciadi", txtKullanici.Text);
                    cmd.Parameters.AddWithValue("@parola", txtParola.Text);
                    cmd.Parameters.AddWithValue("@telefon", txtTel.Text);
                    cmd.Parameters.AddWithValue("@adres", txtAdres.Text);
                    cmd.Parameters.AddWithValue("@id", txtID.Text); 

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Personel bilgileri güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Temizle();
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme başarısız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili personeli silmek istediğinizden emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    int selectedID = Convert.ToInt32(dgv.SelectedRows[0].Cells["personel_id"].Value);

                    using (MySqlConnection conn = new MySqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                            string query = "DELETE FROM blood_personel WHERE personel_id = @id";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", selectedID);

                            int silinen = cmd.ExecuteNonQuery();

                            if (silinen > 0)
                            {
                                MessageBox.Show("Personel başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                PersonelListele(); // Listeyi güncelle
                                Temizle();         // Textbox'ları temizle (varsa)
                            }
                            else
                            {
                                MessageBox.Show("Silme işlemi başarısız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            conn.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir satır seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void frmUsers_Load(object sender, EventArgs e)
        {
            PersonelListele();
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Başlık satırına tıklamayı engeller
            {
                DataGridViewRow row = dgv.Rows[e.RowIndex];

                txtID.Text = row.Cells["personel_id"].Value.ToString();
                txtName.Text = row.Cells["personel_adsoyad"].Value.ToString();
                txtEmail.Text = row.Cells["personel_email"].Value.ToString();
                txtKullanici.Text = row.Cells["personel_kullaniciadi"].Value.ToString();
                txtParola.Text = row.Cells["personel_parola"].Value.ToString();
                txtTel.Text = row.Cells["personel_telefon"].Value.ToString();
                txtAdres.Text = row.Cells["personel_adres"].Value.ToString();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM blood_personel WHERE personel_adsoyad LIKE @arama";
                    MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@arama", "%" + txtAra.Text + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgv.DataSource = dt;

                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Arama sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        
    }
}

