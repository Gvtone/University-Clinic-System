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

namespace Clinic
{
    public partial class PatientPreview : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        public PatientPreview()
        {
            InitializeComponent();
            Com.Connection = CN;

            txtDate.Height = 28;
            txtName.Height = 28;
            txtAddress.Height = 28;
            txtBirthday.Height = 28;
            txtPosition.Height = 28;
            txtCourse.Height = 28;
            txtAge.Height = 28;
            txtSex.Height = 28;
            txtCivilStatus.Height = 28;
            txtContact.Height = 28;
            txtYearSection.Height = 28;
            txtHospitalizationHistory.Height = 28;
            txtBP.Height = 28;
            txtOthersMedical.Height = 28;
            txtOthersDental.Height = 28;
        }

        private void PatientPreview_Load(object sender, EventArgs e)
        {
            string name = DatagridMain.name;
            string date = DatagridMain.date;

            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM patient WHERE NAME = '{0}' AND Date_Added = '{1}'", name, date);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                txtName.Text = reader["Name"].ToString();
                txtAddress.Text = reader["Address"].ToString();
                txtBirthday.Text = reader["Birthday"].ToString();
                txtPosition.Text = reader["Position"].ToString();
                txtCourse.Text = reader["Course"].ToString();
                txtAge.Text = reader["Age"].ToString();
                txtSex.Text = reader["Sex"].ToString();
                txtCivilStatus.Text = reader["Civil_Status"].ToString();
                txtContact.Text = reader["Contact_Number"].ToString();
                txtYearSection.Text = reader["Year_Section"].ToString();

                DateTime set = (DateTime)reader["Date_Added"];
                string formDate = set.ToString("yyyy/MM/dd");
                txtDate.Text = formDate;
            }
            
            reader.Close();
            CN.Close();


        }
    }
}
