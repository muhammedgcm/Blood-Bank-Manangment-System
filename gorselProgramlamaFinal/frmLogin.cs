using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace gorselProgramlamaFinal
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        // veritabanı bağlantı cümlesi
        string connectionString = "server=localhost;database=bloodbank;uid=root;pwd=19661972;";

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = textBox1.Text;
            string parola = textBox2.Text;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM blood_personel WHERE personel_kullaniciadi = @kadi AND personel_parola = @parola";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@kadi", kullaniciAdi);
                    cmd.Parameters.AddWithValue("@parola", parola);

                    MySqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        MessageBox.Show("Sisteme Hoşgeldiniz!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        frmHome home = new frmHome();
                        home.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("kullanıcı adı veya parola hatalı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("hata: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
