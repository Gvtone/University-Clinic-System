using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Clinic
{
    public partial class frmStart : Form
    {
        public frmStart()
        {
            InitializeComponent();

            
            try
            {
                Process.Start("D:\\xampp\\xampp-control.exe");
                //Process.Start("C:\\xampp\\xampp-control.exe");
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 2)
                {
                    MessageBox.Show("The specified file was not found.", "xampp-control.exe not found!");
                }
                else
                {
                    MessageBox.Show("An error occurred while starting the process: " + ex.Message);
                }
            }
        }

        // Enter main application
        private void btnStart_Click(object sender, EventArgs e)
        {
            if (ShowException())
            {
                frmMain main = new frmMain();
                main.Show();
                Hide();
            }
        }

        // Exit application
        private void lblExit_Click(object sender, EventArgs e)
        {
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "xampp-control")
                {
                    // This is the process you want to close
                    if (p.CloseMainWindow() == false)
                    {
                        p.Kill();
                    }
                }
            }

            Application.Exit();
        }

        private void lblExit_MouseHover(object sender, EventArgs e)
        {
            lblExit.ForeColor = Color.FromArgb(105, 181, 255);
        }

        private void lblExit_MouseLeave(object sender, EventArgs e)
        {
            lblExit.ForeColor = Color.FromArgb(34, 50, 66);
        }

        // Checks if mysql server is running
        public bool ShowException()
        {
            string mySelectQuery = "SELECT column1 FROM patient";
            MySqlConnection myConnection = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
            MySqlCommand myCommand = new MySqlCommand(mySelectQuery, myConnection);

            try // If successful return true
            {
                myCommand.Connection.Open();
                return true;
            }
            catch (MySqlException e) // If failed, it will catch the exception and throw the error to the user
            {
                MessageBox.Show(e.Message, "Database not open!");
                return false;
            }
        }
    }
}
