using gorselProgramlamaFinal;
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
    public partial class frmSplash : Form
    {
        public frmSplash()
        {
            InitializeComponent();
        }
        int move = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Write the code to show Loading Animation
            timer1.Interval = 20;

            // Progress bar width increases by 5px
            panelMovable.Width += 5;
            move += 5;

            // If the loading is complete then display login form and close this form
            if (move == 640)
            {
                // Stop the Timer and Close this Form
                timer1.Stop();
                this.Hide();

                // Display the Login Form
                frmLogin login = new frmLogin();
                login.Show();
            }
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {
            // Set the progress bar color to a balanced red (Color.Red)
            panelMovable.BackColor = Color.Red;  // Orta kırmızı tonu

            // Load the Timer
            timer1.Start();
        }
    }
}
