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
    public partial class frmKanKayıt : Form
    {
        public frmKanKayıt()
        {
            InitializeComponent();
            PersonelYukle();
            LoadKayitlar();
            StilUygula();
        }

        private void LoadKayitlar()
        {
            string connStr = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"
                    SELECT 
                        k.kayit_id, 
                        b.donor_id,
                        CONCAT(b.donor_ad, ' ', b.donor_soyad) AS DonorAdi, 
                        k.kan_grubu, 
                        k.miktar_ml, 
                        k.bagis_tarihi, 
                        p.personel_adsoyad AS Personel, 
                        k.personel_id,
                        k.notlar
                    FROM blood_kayit k
                    JOIN blood_bagis b ON k.donor_id = b.donor_id
                    JOIN blood_personel p ON k.personel_id = p.personel_id";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgv.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıtlar çekilemedi: " + ex.Message);
                }
            }
        }

        private void PersonelYukle()
        {
            string connStr = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT personel_id, personel_adsoyad FROM blood_personel";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    Dictionary<int, string> personelListesi = new Dictionary<int, string>();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32("personel_id");
                        string adsoyad = reader.GetString("personel_adsoyad");
                        personelListesi.Add(id, adsoyad);
                    }

                    cmbPersonel.DataSource = new BindingSource(personelListesi, null);
                    cmbPersonel.DisplayMember = "Value";
                    cmbPersonel.ValueMember = "Key";
                    cmbPersonel.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Personel listesi yüklenemedi: " + ex.Message);
                }
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            string connStr = "server=localhost;user=root;password=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO blood_kayit 
                        (donor_id, personel_id, miktar_ml, kan_grubu, bagis_tarihi, notlar)
                        VALUES (@donor_id, @personel_id, @miktar, @grup, @tarih, @not)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@donor_id", int.Parse(txtDonorID.Text));
                    cmd.Parameters.AddWithValue("@personel_id", ((KeyValuePair<int, string>)cmbPersonel.SelectedItem).Key);
                    cmd.Parameters.AddWithValue("@miktar", int.Parse(txtMiktar.Text));
                    cmd.Parameters.AddWithValue("@grup", cmbKanGrubu.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@tarih", dtTarih.Value.Date);
                    cmd.Parameters.AddWithValue("@not", txtNotlar.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Kan kaydı eklendi.");
                    LoadKayitlar();
                    Temizle();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Kayıt eklenemedi: " + ex.Message);
                }
            }
        }

        private void Temizle()
        {
            txtDonorID.Clear();
            txtMiktar.Clear();
            cmbKanGrubu.SelectedIndex = -1;
            cmbPersonel.SelectedIndex = -1;
            txtNotlar.Clear();
            dtTarih.Value = DateTime.Now;
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgv.Rows[e.RowIndex];
                txtDonorID.Text = row.Cells["donor_id"].Value.ToString();
                txtMiktar.Text = row.Cells["miktar_ml"].Value.ToString();
                cmbKanGrubu.SelectedItem = row.Cells["kan_grubu"].Value.ToString();
                dtTarih.Value = Convert.ToDateTime(row.Cells["bagis_tarihi"].Value);
                txtNotlar.Text = row.Cells["notlar"].Value.ToString();
                cmbPersonel.SelectedValue = row.Cells["personel_id"].Value;
            }
        }

        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                int kayitId = Convert.ToInt32(dgv.SelectedRows[0].Cells["kayit_id"].Value);

                string connStr = "server=localhost;user=root;password=19661972;database=bloodbank;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string query = @"UPDATE blood_kayit SET 
                            donor_id=@donor_id,
                            personel_id=@personel_id,
                            miktar_ml=@miktar,
                            kan_grubu=@grup,
                            bagis_tarihi=@tarih,
                            notlar=@not
                            WHERE kayit_id=@id";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@donor_id", int.Parse(txtDonorID.Text));
                        cmd.Parameters.AddWithValue("@personel_id", ((KeyValuePair<int, string>)cmbPersonel.SelectedItem).Key);
                        cmd.Parameters.AddWithValue("@miktar", int.Parse(txtMiktar.Text));
                        cmd.Parameters.AddWithValue("@grup", cmbKanGrubu.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@tarih", dtTarih.Value.Date);
                        cmd.Parameters.AddWithValue("@not", txtNotlar.Text);
                        cmd.Parameters.AddWithValue("@id", kayitId);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Kayıt güncellendi.");
                        LoadKayitlar();
                        Temizle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Güncelleme hatası: " + ex.Message);
                    }
                }
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                int kayitId = Convert.ToInt32(dgv.SelectedRows[0].Cells["kayit_id"].Value);

                string connStr = "server=localhost;user=root;password=19661972;database=bloodbank;";
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM blood_kayit WHERE kayit_id=@id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", kayitId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Kayıt silindi.");
                        LoadKayitlar();
                        Temizle();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Silme hatası: " + ex.Message);
                    }
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

        private void frmKanKayıt_Load(object sender, EventArgs e)
        {

        }
        private void txtAra_TextChanged(object sender, EventArgs e)
        {
            string arama = txtAra.Text;
            string connStr = "server=localhost;user=root;password=19661972;database=bloodbank;";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT 
                    k.kayit_id, 
                    b.donor_id,
                    CONCAT(b.donor_ad, ' ', b.donor_soyad) AS DonorAdi, 
                    k.kan_grubu, 
                    k.miktar_ml, 
                    k.bagis_tarihi, 
                    p.personel_adsoyad AS Personel, 
                    k.notlar
                FROM blood_kayit k
                JOIN blood_bagis b ON k.donor_id = b.donor_id
                JOIN blood_personel p ON k.personel_id = p.personel_id
                WHERE CONCAT(b.donor_ad, ' ', b.donor_soyad) LIKE @arama";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@arama", "%" + arama + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgv.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Arama hatası: " + ex.Message);
                }
            }
        }

        private void btnTemizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmHome home = new frmHome();
            home.Show();
            this.Hide();
        }
    }
}