using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections.Generic;

namespace gorselProgramlamaFinal
{
    public partial class frmStock : Form
    {
        public frmStock()
        {
            InitializeComponent();
        }

        private void frmStock_Load(object sender, EventArgs e)
        {
            KanGrubuGrafikCiz();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmHome home = new frmHome();
            home.Show();
            this.Hide();
        }

        private void KanGrubuGrafikCiz()
        {
            string connStr = "server=localhost;uid=root;pwd=19661972;database=bloodbank;";
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT donor_kangrubu, COUNT(*) as adet FROM blood_bagis GROUP BY donor_kangrubu";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    chartKanGrubu.Series.Clear();
                    chartKanGrubu.Titles.Clear();
                    chartKanGrubu.ChartAreas.Clear();

                    // Grafik alanı ayarları (3D)
                    ChartArea area = new ChartArea("Alan1");
                    area.BackColor = Color.White;
                    area.Area3DStyle.Enable3D = true; // 3D grafik aktif
                    area.Area3DStyle.Inclination = 15;
                    area.Area3DStyle.Rotation = 10;
                    area.Area3DStyle.LightStyle = LightStyle.Realistic;

                    area.AxisX.MajorGrid.LineColor = Color.LightGray;
                    area.AxisY.MajorGrid.LineColor = Color.LightGray;
                    area.AxisX.Title = "Kan Grubu";
                    area.AxisY.Title = "Adet";
                    area.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                    area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
                    area.AxisX.LabelStyle.Font = new Font("Segoe UI", 9);
                    area.AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
                    area.AxisX.Interval = 1;
                    area.AxisX.LabelStyle.Angle = -45; // Etiketleri çaprazla

                    chartKanGrubu.ChartAreas.Add(area);
                    chartKanGrubu.BackColor = Color.WhiteSmoke;
                    chartKanGrubu.BorderlineDashStyle = ChartDashStyle.Solid;
                    chartKanGrubu.BorderlineColor = Color.Silver;

                    // Başlık
                    Title baslik = new Title
                    {
                        Text = "Kan Grubu Stokları",
                        Font = new Font("Segoe UI", 16, FontStyle.Bold),
                        ForeColor = Color.DarkRed
                    };
                    chartKanGrubu.Titles.Add(baslik);

                    // Seri tanımı
                    Series seri = new Series("Kan Grupları")
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true,
                        Font = new Font("Segoe UI", 10, FontStyle.Bold),
                        Color = Color.DarkRed,
                        LabelForeColor = Color.Black
                    };

                    // Kan grubu sıralaması
                    string[] kanGruplari = new string[] { "0-", "0+", "A+", "A-", "B+", "B-", "AB+", "AB-" };
                    int[] adetler = new int[kanGruplari.Length];
                    bool veriVarMi = false;

                    while (reader.Read())
                    {
                        veriVarMi = true;
                        string kanGrubu = reader["donor_kangrubu"].ToString();
                        int adet = Convert.ToInt32(reader["adet"]);

                        // Kan grubunun dizindeki indeksini bulma
                        int index = Array.IndexOf(kanGruplari, kanGrubu);
                        if (index >= 0)
                        {
                            adetler[index] = adet; // İlgili kan grubuna adet ekleniyor
                        }
                    }

                    // Verileri grafik serisine ekle
                    for (int i = 0; i < kanGruplari.Length; i++)
                    {
                        seri.Points.AddXY(kanGruplari[i], adetler[i]);
                    }

                    if (veriVarMi)
                    {
                        chartKanGrubu.Series.Add(seri);
                    }
                    else
                    {
                        MessageBox.Show("Hiç veri bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message);
                }
            }
        }




    }
}
