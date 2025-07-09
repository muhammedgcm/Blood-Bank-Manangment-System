using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace gorselProgramlamaFinal
{
    public partial class frmRandevu : Form
    {
        public frmRandevu()
        {
            InitializeComponent();
        }

        string connectionString = "server=localhost;database=bloodbank;uid=root;pwd=19661972;";

        private void frmRandevu_Load(object sender, EventArgs e)
        {
            cmbDonor.SelectedIndexChanged -= cmbDonor_SelectedIndexChanged;
            DonorleriYukle();
            PersonelleriYukle();
            cmbDonor.SelectedIndexChanged += cmbDonor_SelectedIndexChanged;
            VerileriYukle();
        }

        private void VerileriYukle()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT r.randevu_id, CONCAT(d.donor_ad, ' ', d.donor_soyad) AS donor_adsoyad, " +
                                   "r.kan_grubu, r.tarih, r.saat, CONCAT(p.personel_adsoyad) AS personel_adsoyad, r.notlar " +
                                   "FROM blood_randevu r " +
                                   "INNER JOIN blood_bagis d ON r.donor_id = d.donor_id " +
                                   "INNER JOIN blood_personel p ON r.personel_id = p.personel_id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgv.DataSource = dt;
                    StilUygula();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void DonorleriYukle()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT donor_id, CONCAT(donor_ad, ' ', donor_soyad) AS ad_soyad FROM blood_bagis";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader dr = cmd.ExecuteReader();

                    Dictionary<int, string> donorlar = new Dictionary<int, string>
                    {
                        { 0, "-- Donör Seçiniz --" }
                    };

                    while (dr.Read())
                    {
                        int id = dr.GetInt32("donor_id");
                        string adSoyad = dr.GetString("ad_soyad");
                        donorlar.Add(id, adSoyad);
                    }

                    cmbDonor.DataSource = new BindingSource(donorlar, null);
                    cmbDonor.DisplayMember = "Value";
                    cmbDonor.ValueMember = "Key";
                    cmbDonor.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Donörler yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void PersonelleriYukle()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT personel_id, personel_adsoyad FROM blood_personel";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader dr = cmd.ExecuteReader();

                    Dictionary<int, string> personeller = new Dictionary<int, string>
                    {
                        { 0, "-- Personel Seçiniz --" }
                    };

                    while (dr.Read())
                    {
                        int id = dr.GetInt32("personel_id");
                        string adSoyad = dr.GetString("personel_adsoyad");
                        personeller.Add(id, adSoyad);
                    }

                    cmbPersonel.DataSource = new BindingSource(personeller, null);
                    cmbPersonel.DisplayMember = "Value";
                    cmbPersonel.ValueMember = "Key";
                    cmbPersonel.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Personeller yüklenirken hata oluştu: " + ex.Message);
            }
        }

        private void btnGonder_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedDonorId = (int)cmbDonor.SelectedValue;
                string kanGrubu = cmbKanGrubu.Text;
                DateTime tarih = dtTarih.Value;
                string saat = mtbSaat.Text;
                int selectedPersonelId = (int)cmbPersonel.SelectedValue;
                string notlar = txtNot.Text;

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO blood_randevu (donor_id, kan_grubu, tarih, saat, personel_id, notlar) " +
                                   "VALUES (@donorId, @kanGrubu, @tarih, @saat, @personelId, @notlar)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@donorId", selectedDonorId);
                    cmd.Parameters.AddWithValue("@kanGrubu", kanGrubu);
                    cmd.Parameters.AddWithValue("@tarih", tarih);
                    cmd.Parameters.AddWithValue("@saat", saat);
                    cmd.Parameters.AddWithValue("@personelId", selectedPersonelId);
                    cmd.Parameters.AddWithValue("@notlar", notlar);

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Randevu başarıyla kaydedildi.");

                        // Veriler temizlenmeden önce mail gönder
                        SendEmailAsync(((KeyValuePair<int, string>)cmbDonor.SelectedItem).Value,
                              kanGrubu,
                              tarih.ToString("dd/MM/yyyy"),
                               saat,
                               ((KeyValuePair<int, string>)cmbPersonel.SelectedItem).Value,
                               notlar
                            );

                        cmbDonor.SelectedIndex = 0;
                        cmbKanGrubu.SelectedIndex = 0;
                        dtTarih.Value = DateTime.Now;
                        mtbSaat.Clear();
                        cmbPersonel.SelectedIndex = 0;
                        txtNot.Clear();

                        VerileriYukle();
                    }
                    else
                    {
                        MessageBox.Show("Randevu kaydedilirken bir hata oluştu.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void SendEmailAsync(string donor, string kanGrubu, string tarih, string saat, string personel, string notlar)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("muhammedgocmen3838@gmail.com", "fxagtfnpnffazrga"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("muhammedgocmen3838@gmail.com"),
                    Subject = "Yeni Randevu",
                    Body = "Yeni bir randevu kaydedildi.\n\nDonör: " + donor +
                           "\nKan Grubu: " + kanGrubu +
                           "\nTarih: " + tarih +
                           "\nSaat: " + saat +
                           "\nPersonel: " + personel +
                           "\nNotlar: " + notlar,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add("muhammedgocmen3838@gmail.com");

                // asenkron yerine doğrudan gönderim (hata yakalama kolay)
                smtpClient.Send(mailMessage);
                MessageBox.Show("E-posta başarıyla gönderildi.");

                smtpClient.Dispose();
                mailMessage.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("E-posta gönderimi sırasında hata oluştu: " + ex.Message);
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

        private void cmbDonor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDonor.SelectedValue is int selectedDonorId)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT donor_kangrubu FROM blood_bagis WHERE donor_id = @donorId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@donorId", selectedDonorId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        cmbKanGrubu.Text = result.ToString();
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
           frmHome home = new frmHome();
            home.Show();
            this.Hide();
        }
    }
}
