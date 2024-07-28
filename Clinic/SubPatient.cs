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
    public partial class SubPatient : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        // Event Handlers
        public static EventHandler Add_Done;

        private static string[,] teeth = new string[52, 5];
        public static string teethNumber;

        public SubPatient()
        {
            InitializeComponent();
            Com.Connection = CN;

            txtOthersMedical.Height = 28;
            txtHopitalizationHistory.Height = 28;
            txtBP.Height = 28;
            txtOthersDental.Height = 28;
            dtpDate.Value = DateTime.Today;
            dtpBirthday.Value = DateTime.Today;

            Teeth.Add_Clicked += new EventHandler(Load_Teeth);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!textboxHasContent(grpPersonalBackground))
            {
                MessageBox.Show("Please make sure that boxes with (*) has contents!");
                return;
            }
            if (!IsDigit(txtAge.Text))
            {
                MessageBox.Show("Invalid input in Age!");
                txtAge.Focus();
                return;
            }
            string contactLength = txtContact.Text;
            if (!IsDigit(txtContact.Text) || contactLength.Length != 11)
            {
                MessageBox.Show("Invalid input in Contact Number!");
                txtContact.Focus();
                return;
            }

            // ========================= [PERSONAL BACKGROUND] =========================
            string Name = txtName.Text;
            string Address = txtAddress.Text;
            string Birthday = dtpBirthday.Value.ToString("yyyy/MM/dd");
            string Position = txtPosition.Text;
            string Course = txtCourse.Text;
            string Age = txtAge.Text;
            string Sex = cmbSex.Text;
            string Civil_Status = cmbCivilStatus.Text;
            string Contact_Number = txtContact.Text;
            string Year_Section = (cmbYear.Text + "-" + cmbSection.Text);
            string Date_Added = dtpDate.Value.ToString("yyyy/MM/dd");

            // Insert patient personal data
            CN.Open();
            Com.CommandText = string.Format("INSERT INTO patient(Name, Address, Birthday, Position, Course, Age, Sex, Civil_Status, Contact_Number, Year_Section, Date_Added) " +
                "VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')", Name, Address, Birthday, Position, Course, Age, Sex, Civil_Status, Contact_Number, Year_Section, Date_Added);
            Com.ExecuteNonQuery();
            CN.Close();

            // Set Patient_ID to be readily available
            CN.Open();
            Com.CommandText = string.Format("SELECT ID FROM patient WHERE Name = '{0}' AND Date_Added = '{1}'", Name, Date_Added);
            reader = Com.ExecuteReader();
            reader.Read();
            string Patient_ID = reader["ID"].ToString();
            reader.Close();
            CN.Close();

            // ========================= [MEDICAL HISTORY] =========================
            string Hospitalization = txtHopitalizationHistory.Text;
            string BP = txtBP.Text;

            CN.Open();
            Com.CommandText = string.Format("INSERT INTO medical(Patient_ID, Hospitalization, BP) VALUES('{0}', '{1}', '{2}')", Patient_ID, Hospitalization, BP);
            Com.ExecuteNonQuery();
            CN.Close();

            // Medical Checkboxes
            if (hasContent(grpMedicalHistory))
            {
                string[] activeList = activeBoxList(grpMedicalHistory);

                foreach (string boxName in activeList)
                {
                    // Gets the ID that corresponds to the name of the checked textbox
                    string Type_ID = "";
                    CN.Open();
                    Com.CommandText = string.Format("SELECT ID FROM type WHERE History = '{0}'", boxName);
                    reader = Com.ExecuteReader();
                    if (reader.Read())
                    {
                        Type_ID = reader["ID"].ToString();
                    }
                    reader.Close();
                    CN.Close();

                    // Gets the Medical ID that corresponds to the Patient ID
                    CN.Open();
                    Com.CommandText = string.Format("SELECT ID FROM medical WHERE Patient_ID = '{0}'", Patient_ID);
                    reader = Com.ExecuteReader();
                    reader.Read();
                    string Medical_ID = reader["ID"].ToString();
                    reader.Close();
                    CN.Close();

                    // Inserts data to Medical_Type to connect Mecial and Type
                    CN.Open();
                    Com.CommandText = string.Format("INSERT INTO medical_type(Medical_ID, Type_ID) VALUES('{0}', '{1}')", Medical_ID, Type_ID);
                    Com.ExecuteNonQuery();
                    CN.Close();
                }
            }
            
            // Medical Others
            if (!string.IsNullOrEmpty(txtOthersMedical.Text))
            {
                string otherType = txtOthersMedical.Text;

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO type(History) VALUES('{0}')", otherType);
                Com.ExecuteNonQuery();
                CN.Close();

                // Gets the ID that corresponds to the text inside the "other" textbox
                CN.Open();
                Com.CommandText = string.Format("SELECT ID FROM type WHERE History = '{0}'", otherType);
                reader = Com.ExecuteReader();
                reader.Read();
                string Type_ID = reader["ID"].ToString();
                reader.Close();
                CN.Close();

                // Gets the Medical ID that corresponds to the Patient ID
                CN.Open();
                Com.CommandText = string.Format("SELECT ID FROM medical WHERE Patient_ID = '{0}'", Patient_ID);
                reader = Com.ExecuteReader();
                reader.Read();
                string Medical_ID = reader["ID"].ToString();
                reader.Close();
                CN.Close();

                // Inserts data to Medical_Type to connect Mecial and Type
                CN.Open();
                Com.CommandText = string.Format("INSERT INTO medical_type(Medical_ID, Type_ID) VALUES('{0}', '{1}')", Medical_ID, Type_ID);
                Com.ExecuteNonQuery();
                CN.Close();
            }

            // ========================= [DENTAL HISTORY] =========================
            if (hasContent(grpDentalHistory))
            {
                string[] activeList = activeBoxList(grpDentalHistory);

                foreach (string boxName in activeList)
                {
                    // Gets the ID that corresponds to the name of the checked textbox
                    CN.Open();
                    Com.CommandText = string.Format("SELECT ID FROM dental WHERE History = '{0}'", boxName);
                    reader = Com.ExecuteReader();
                    reader.Read();
                    string Dental_ID = reader["ID"].ToString();
                    reader.Close();
                    CN.Close();

                    // Inserts data to Patient_Dental to connect Patient and Dental
                    CN.Open();
                    Com.CommandText = string.Format("INSERT INTO patient_dental(Patient_ID, Dental_ID) VALUES('{0}', '{1}')", Patient_ID, Dental_ID);
                    Com.ExecuteNonQuery();
                    CN.Close();
                }
            }

            // Dental Others
            if (!string.IsNullOrEmpty(txtOthersDental.Text))
            {
                string otherType = txtOthersDental.Text;

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO dental(History) VALUES('{0}')", otherType);
                Com.ExecuteNonQuery();
                CN.Close();

                // Gets the ID that corresponds to the text inside the "other" textbox
                CN.Open();
                Com.CommandText = string.Format("SELECT ID FROM dental WHERE History = '{0}'", otherType);
                reader = Com.ExecuteReader();
                reader.Read();
                string Dental_ID = reader["ID"].ToString();
                reader.Close();
                CN.Close();

                // Inserts data to Patient_Dental to connect Patient and Dental
                CN.Open();
                Com.CommandText = string.Format("INSERT INTO patient_dental(Patient_ID, Dental_ID) VALUES('{0}', '{1}')", Patient_ID, Dental_ID);
                Com.ExecuteNonQuery();
                CN.Close();
            }

            // ========================= [Teeth] =========================
            // <55>
            if (!string.IsNullOrEmpty(chkT55.Text) && !string.IsNullOrEmpty(chkC55.Text))
            {
                string t = chkT55.Text;
                string c = chkC55.Text;
                string location = "55";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT55.Text))
            {
                string t = chkT55.Text;
                string location = "55";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC55.Text))
            {
                string c = chkC55.Text;
                string location = "55";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "55";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </55>

            // <54>
            if (!string.IsNullOrEmpty(chkT54.Text) && !string.IsNullOrEmpty(chkC54.Text))
            {
                string t = chkT54.Text;
                string c = chkC54.Text;
                string location = "54";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT54.Text))
            {
                string t = chkT54.Text;
                string location = "54";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC54.Text))
            {
                string c = chkC54.Text;
                string location = "54";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "54";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </54>

            // <53>
            if (!string.IsNullOrEmpty(chkT53.Text) && !string.IsNullOrEmpty(chkC53.Text))
            {
                string t = chkT53.Text;
                string c = chkC53.Text;
                string location = "53";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT53.Text))
            {
                string t = chkT53.Text;
                string location = "53";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC53.Text))
            {
                string c = chkC53.Text;
                string location = "53";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "53";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </53>

            // <52>
            if (!string.IsNullOrEmpty(chkT52.Text) && !string.IsNullOrEmpty(chkC52.Text))
            {
                string t = chkT52.Text;
                string c = chkC52.Text;
                string location = "52";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT52.Text))
            {
                string t = chkT52.Text;
                string location = "52";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC52.Text))
            {
                string c = chkC52.Text;
                string location = "52";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "52";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </52>

            // <51>
            if (!string.IsNullOrEmpty(chkT51.Text) && !string.IsNullOrEmpty(chkC51.Text))
            {
                string t = chkT51.Text;
                string c = chkC51.Text;
                string location = "51";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT51.Text))
            {
                string t = chkT51.Text;
                string location = "51";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC51.Text))
            {
                string c = chkC51.Text;
                string location = "51";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "51";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </51>

            // <61>
            if (!string.IsNullOrEmpty(chkT61.Text) && !string.IsNullOrEmpty(chkC61.Text))
            {
                string t = chkT61.Text;
                string c = chkC61.Text;
                string location = "61";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT61.Text))
            {
                string t = chkT61.Text;
                string location = "61";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC61.Text))
            {
                string c = chkC61.Text;
                string location = "61";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "61";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </61>

            // <62>
            if (!string.IsNullOrEmpty(chkT62.Text) && !string.IsNullOrEmpty(chkC62.Text))
            {
                string t = chkT62.Text;
                string c = chkC62.Text;
                string location = "62";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT62.Text))
            {
                string t = chkT62.Text;
                string location = "62";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC62.Text))
            {
                string c = chkC62.Text;
                string location = "62";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "62";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </62>

            // <63>
            if (!string.IsNullOrEmpty(chkT63.Text) && !string.IsNullOrEmpty(chkC63.Text))
            {
                string t = chkT63.Text;
                string c = chkC63.Text;
                string location = "63";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT63.Text))
            {
                string t = chkT63.Text;
                string location = "63";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC63.Text))
            {
                string c = chkC63.Text;
                string location = "63";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "63";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </63>

            // <64>
            if (!string.IsNullOrEmpty(chkT64.Text) && !string.IsNullOrEmpty(chkC64.Text))
            {
                string t = chkT64.Text;
                string c = chkC64.Text;
                string location = "64";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT64.Text))
            {
                string t = chkT64.Text;
                string location = "64";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC64.Text))
            {
                string c = chkC64.Text;
                string location = "64";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "64";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </64>

            // <65>
            if (!string.IsNullOrEmpty(chkT65.Text) && !string.IsNullOrEmpty(chkC65.Text))
            {
                string t = chkT65.Text;
                string c = chkC65.Text;
                string location = "65";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT65.Text))
            {
                string t = chkT65.Text;
                string location = "65";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC65.Text))
            {
                string c = chkC65.Text;
                string location = "65";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "65";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </65>

            // <18>
            if (!string.IsNullOrEmpty(chkT18.Text) && !string.IsNullOrEmpty(chkC18.Text))
            {
                string t = chkT18.Text;
                string c = chkC18.Text;
                string location = "18";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT18.Text))
            {
                string t = chkT18.Text;
                string location = "18";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC18.Text))
            {
                string c = chkC18.Text;
                string location = "18";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "18";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </18>

            // <17>
            if (!string.IsNullOrEmpty(chkT17.Text) && !string.IsNullOrEmpty(chkC17.Text))
            {
                string t = chkT17.Text;
                string c = chkC17.Text;
                string location = "17";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT17.Text))
            {
                string t = chkT17.Text;
                string location = "17";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC17.Text))
            {
                string c = chkC17.Text;
                string location = "17";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "17";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </17>

            // <16>
            if (!string.IsNullOrEmpty(chkT16.Text) && !string.IsNullOrEmpty(chkC16.Text))
            {
                string t = chkT16.Text;
                string c = chkC16.Text;
                string location = "16";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT16.Text))
            {
                string t = chkT16.Text;
                string location = "16";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC16.Text))
            {
                string c = chkC16.Text;
                string location = "16";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "16";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </16>

            // <15>
            if (!string.IsNullOrEmpty(chkT15.Text) && !string.IsNullOrEmpty(chkC15.Text))
            {
                string t = chkT15.Text;
                string c = chkC15.Text;
                string location = "15";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT15.Text))
            {
                string t = chkT15.Text;
                string location = "15";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC15.Text))
            {
                string c = chkC15.Text;
                string location = "15";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "15";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </15>

            // <14>
            if (!string.IsNullOrEmpty(chkT14.Text) && !string.IsNullOrEmpty(chkC14.Text))
            {
                string t = chkT14.Text;
                string c = chkC14.Text;
                string location = "14";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT14.Text))
            {
                string t = chkT14.Text;
                string location = "14";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC14.Text))
            {
                string c = chkC14.Text;
                string location = "14";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "14";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </14>

            // <13>
            if (!string.IsNullOrEmpty(chkT13.Text) && !string.IsNullOrEmpty(chkC13.Text))
            {
                string t = chkT13.Text;
                string c = chkC13.Text;
                string location = "13";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT13.Text))
            {
                string t = chkT13.Text;
                string location = "13";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC13.Text))
            {
                string c = chkC13.Text;
                string location = "13";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "13";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </13>

            // <12>
            if (!string.IsNullOrEmpty(chkT12.Text) && !string.IsNullOrEmpty(chkC12.Text))
            {
                string t = chkT12.Text;
                string c = chkC12.Text;
                string location = "12";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT12.Text))
            {
                string t = chkT12.Text;
                string location = "12";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC12.Text))
            {
                string c = chkC12.Text;
                string location = "12";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "12";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </12>

            // <11>
            if (!string.IsNullOrEmpty(chkT11.Text) && !string.IsNullOrEmpty(chkC11.Text))
            {
                string t = chkT11.Text;
                string c = chkC11.Text;
                string location = "11";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT11.Text))
            {
                string t = chkT11.Text;
                string location = "11";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC11.Text))
            {
                string c = chkC11.Text;
                string location = "11";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "11";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </11>

            // <21>
            if (!string.IsNullOrEmpty(chkT21.Text) && !string.IsNullOrEmpty(chkC21.Text))
            {
                string t = chkT21.Text;
                string c = chkC21.Text;
                string location = "21";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT21.Text))
            {
                string t = chkT21.Text;
                string location = "21";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC21.Text))
            {
                string c = chkC21.Text;
                string location = "21";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "21";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </21>

            // <22>
            if (!string.IsNullOrEmpty(chkT22.Text) && !string.IsNullOrEmpty(chkC22.Text))
            {
                string t = chkT22.Text;
                string c = chkC22.Text;
                string location = "22";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT22.Text))
            {
                string t = chkT22.Text;
                string location = "22";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC22.Text))
            {
                string c = chkC22.Text;
                string location = "22";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "22";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </22>

            // <23>
            if (!string.IsNullOrEmpty(chkT23.Text) && !string.IsNullOrEmpty(chkC23.Text))
            {
                string t = chkT23.Text;
                string c = chkC23.Text;
                string location = "23";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT23.Text))
            {
                string t = chkT23.Text;
                string location = "23";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC23.Text))
            {
                string c = chkC23.Text;
                string location = "23";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "23";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </23>

            // <24>
            if (!string.IsNullOrEmpty(chkT24.Text) && !string.IsNullOrEmpty(chkC24.Text))
            {
                string t = chkT24.Text;
                string c = chkC24.Text;
                string location = "24";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT24.Text))
            {
                string t = chkT24.Text;
                string location = "24";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC24.Text))
            {
                string c = chkC24.Text;
                string location = "24";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "24";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </24>

            // <25>
            if (!string.IsNullOrEmpty(chkT25.Text) && !string.IsNullOrEmpty(chkC25.Text))
            {
                string t = chkT25.Text;
                string c = chkC25.Text;
                string location = "25";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT25.Text))
            {
                string t = chkT25.Text;
                string location = "25";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC25.Text))
            {
                string c = chkC25.Text;
                string location = "25";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "25";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </25>

            // <26>
            if (!string.IsNullOrEmpty(chkT26.Text) && !string.IsNullOrEmpty(chkC26.Text))
            {
                string t = chkT26.Text;
                string c = chkC26.Text;
                string location = "26";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT26.Text))
            {
                string t = chkT26.Text;
                string location = "26";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC26.Text))
            {
                string c = chkC26.Text;
                string location = "26";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "26";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </26>

            // <27>
            if (!string.IsNullOrEmpty(chkT27.Text) && !string.IsNullOrEmpty(chkC27.Text))
            {
                string t = chkT27.Text;
                string c = chkC27.Text;
                string location = "27";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT27.Text))
            {
                string t = chkT27.Text;
                string location = "27";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC27.Text))
            {
                string c = chkC27.Text;
                string location = "27";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "27";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </27>

            // <28>
            if (!string.IsNullOrEmpty(chkT28.Text) && !string.IsNullOrEmpty(chkC28.Text))
            {
                string t = chkT28.Text;
                string c = chkC28.Text;
                string location = "28";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT28.Text))
            {
                string t = chkT28.Text;
                string location = "28";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC28.Text))
            {
                string c = chkC28.Text;
                string location = "28";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "28";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </28>

            // <48>
            if (!string.IsNullOrEmpty(chkT48.Text) && !string.IsNullOrEmpty(chkC48.Text))
            {
                string t = chkT48.Text;
                string c = chkC48.Text;
                string location = "48";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT48.Text))
            {
                string t = chkT48.Text;
                string location = "48";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC48.Text))
            {
                string c = chkC48.Text;
                string location = "48";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "48";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </48>

            // <47>
            if (!string.IsNullOrEmpty(chkT47.Text) && !string.IsNullOrEmpty(chkC47.Text))
            {
                string t = chkT47.Text;
                string c = chkC47.Text;
                string location = "47";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT47.Text))
            {
                string t = chkT47.Text;
                string location = "47";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC47.Text))
            {
                string c = chkC47.Text;
                string location = "47";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "47";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </47>

            // <46>
            if (!string.IsNullOrEmpty(chkT46.Text) && !string.IsNullOrEmpty(chkC46.Text))
            {
                string t = chkT46.Text;
                string c = chkC46.Text;
                string location = "46";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT46.Text))
            {
                string t = chkT46.Text;
                string location = "46";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC46.Text))
            {
                string c = chkC46.Text;
                string location = "46";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "46";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </46>

            // <45>
            if (!string.IsNullOrEmpty(chkT45.Text) && !string.IsNullOrEmpty(chkC45.Text))
            {
                string t = chkT45.Text;
                string c = chkC45.Text;
                string location = "45";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT45.Text))
            {
                string t = chkT45.Text;
                string location = "45";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC45.Text))
            {
                string c = chkC45.Text;
                string location = "45";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "45";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </45>

            // <44>
            if (!string.IsNullOrEmpty(chkT44.Text) && !string.IsNullOrEmpty(chkC44.Text))
            {
                string t = chkT44.Text;
                string c = chkC44.Text;
                string location = "44";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT44.Text))
            {
                string t = chkT44.Text;
                string location = "44";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC44.Text))
            {
                string c = chkC44.Text;
                string location = "44";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "44";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </44>

            // <43>
            if (!string.IsNullOrEmpty(chkT43.Text) && !string.IsNullOrEmpty(chkC43.Text))
            {
                string t = chkT43.Text;
                string c = chkC43.Text;
                string location = "43";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT43.Text))
            {
                string t = chkT43.Text;
                string location = "43";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC43.Text))
            {
                string c = chkC43.Text;
                string location = "43";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "43";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </43>

            // <42>
            if (!string.IsNullOrEmpty(chkT42.Text) && !string.IsNullOrEmpty(chkC42.Text))
            {
                string t = chkT42.Text;
                string c = chkC42.Text;
                string location = "42";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT42.Text))
            {
                string t = chkT42.Text;
                string location = "42";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC42.Text))
            {
                string c = chkC42.Text;
                string location = "42";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "42";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </42>

            // <41>
            if (!string.IsNullOrEmpty(chkT41.Text) && !string.IsNullOrEmpty(chkC41.Text))
            {
                string t = chkT41.Text;
                string c = chkC41.Text;
                string location = "41";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT41.Text))
            {
                string t = chkT41.Text;
                string location = "41";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC41.Text))
            {
                string c = chkC41.Text;
                string location = "41";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "41";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </41>

            // <31>
            if (!string.IsNullOrEmpty(chkT31.Text) && !string.IsNullOrEmpty(chkC31.Text))
            {
                string t = chkT31.Text;
                string c = chkC31.Text;
                string location = "31";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT31.Text))
            {
                string t = chkT31.Text;
                string location = "31";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC31.Text))
            {
                string c = chkC31.Text;
                string location = "31";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "31";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </31>

            // <32>
            if (!string.IsNullOrEmpty(chkT32.Text) && !string.IsNullOrEmpty(chkC32.Text))
            {
                string t = chkT32.Text;
                string c = chkC32.Text;
                string location = "32";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT32.Text))
            {
                string t = chkT32.Text;
                string location = "32";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC32.Text))
            {
                string c = chkC32.Text;
                string location = "32";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "32";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </32>

            // <33>
            if (!string.IsNullOrEmpty(chkT33.Text) && !string.IsNullOrEmpty(chkC33.Text))
            {
                string t = chkT33.Text;
                string c = chkC33.Text;
                string location = "33";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT33.Text))
            {
                string t = chkT33.Text;
                string location = "33";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC33.Text))
            {
                string c = chkC33.Text;
                string location = "33";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "33";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </33>

            // <34>
            if (!string.IsNullOrEmpty(chkT34.Text) && !string.IsNullOrEmpty(chkC34.Text))
            {
                string t = chkT34.Text;
                string c = chkC34.Text;
                string location = "34";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT34.Text))
            {
                string t = chkT34.Text;
                string location = "34";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC34.Text))
            {
                string c = chkC34.Text;
                string location = "34";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "34";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </34>

            // <35>
            if (!string.IsNullOrEmpty(chkT35.Text) && !string.IsNullOrEmpty(chkC35.Text))
            {
                string t = chkT35.Text;
                string c = chkC35.Text;
                string location = "35";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT35.Text))
            {
                string t = chkT35.Text;
                string location = "35";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC35.Text))
            {
                string c = chkC35.Text;
                string location = "35";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "35";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </35>

            // <36>
            if (!string.IsNullOrEmpty(chkT36.Text) && !string.IsNullOrEmpty(chkC36.Text))
            {
                string t = chkT36.Text;
                string c = chkC36.Text;
                string location = "36";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT36.Text))
            {
                string t = chkT36.Text;
                string location = "36";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC36.Text))
            {
                string c = chkC36.Text;
                string location = "36";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "36";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </36>

            // <37>
            if (!string.IsNullOrEmpty(chkT37.Text) && !string.IsNullOrEmpty(chkC37.Text))
            {
                string t = chkT37.Text;
                string c = chkC37.Text;
                string location = "37";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT37.Text))
            {
                string t = chkT37.Text;
                string location = "37";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC37.Text))
            {
                string c = chkC37.Text;
                string location = "37";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "37";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </37>

            // <38>
            if (!string.IsNullOrEmpty(chkT38.Text) && !string.IsNullOrEmpty(chkC38.Text))
            {
                string t = chkT38.Text;
                string c = chkC38.Text;
                string location = "38";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT38.Text))
            {
                string t = chkT38.Text;
                string location = "38";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC38.Text))
            {
                string c = chkC38.Text;
                string location = "38";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "38";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </38>

            // <85>
            if (!string.IsNullOrEmpty(chkT85.Text) && !string.IsNullOrEmpty(chkC85.Text))
            {
                string t = chkT85.Text;
                string c = chkC85.Text;
                string location = "85";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT85.Text))
            {
                string t = chkT85.Text;
                string location = "85";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC85.Text))
            {
                string c = chkC85.Text;
                string location = "85";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "85";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </85>

            // <84>
            if (!string.IsNullOrEmpty(chkT84.Text) && !string.IsNullOrEmpty(chkC84.Text))
            {
                string t = chkT84.Text;
                string c = chkC84.Text;
                string location = "84";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT84.Text))
            {
                string t = chkT84.Text;
                string location = "84";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC84.Text))
            {
                string c = chkC84.Text;
                string location = "84";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "84";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </84>

            // <83>
            if (!string.IsNullOrEmpty(chkT83.Text) && !string.IsNullOrEmpty(chkC83.Text))
            {
                string t = chkT83.Text;
                string c = chkC83.Text;
                string location = "83";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT83.Text))
            {
                string t = chkT83.Text;
                string location = "83";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC83.Text))
            {
                string c = chkC83.Text;
                string location = "83";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "83";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </83>

            // <82>
            if (!string.IsNullOrEmpty(chkT82.Text) && !string.IsNullOrEmpty(chkC82.Text))
            {
                string t = chkT82.Text;
                string c = chkC82.Text;
                string location = "82";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT82.Text))
            {
                string t = chkT82.Text;
                string location = "82";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC82.Text))
            {
                string c = chkC82.Text;
                string location = "82";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "82";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </82>

            // <81>
            if (!string.IsNullOrEmpty(chkT81.Text) && !string.IsNullOrEmpty(chkC81.Text))
            {
                string t = chkT81.Text;
                string c = chkC81.Text;
                string location = "81";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT81.Text))
            {
                string t = chkT81.Text;
                string location = "81";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC81.Text))
            {
                string c = chkC85.Text;
                string location = "81";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "81";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </81>

            // <71>
            if (!string.IsNullOrEmpty(chkT71.Text) && !string.IsNullOrEmpty(chkC71.Text))
            {
                string t = chkT71.Text;
                string c = chkC71.Text;
                string location = "71";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT71.Text))
            {
                string t = chkT71.Text;
                string location = "71";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC71.Text))
            {
                string c = chkC71.Text;
                string location = "71";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "71";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </71>

            // <72>
            if (!string.IsNullOrEmpty(chkT72.Text) && !string.IsNullOrEmpty(chkC72.Text))
            {
                string t = chkT72.Text;
                string c = chkC72.Text;
                string location = "72";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT72.Text))
            {
                string t = chkT72.Text;
                string location = "72";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC72.Text))
            {
                string c = chkC72.Text;
                string location = "72";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "72";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </72>

            // <73>
            if (!string.IsNullOrEmpty(chkT73.Text) && !string.IsNullOrEmpty(chkC73.Text))
            {
                string t = chkT73.Text;
                string c = chkC73.Text;
                string location = "73";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT73.Text))
            {
                string t = chkT73.Text;
                string location = "73";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC73.Text))
            {
                string c = chkC73.Text;
                string location = "73";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "73";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </73>

            // <74>
            if (!string.IsNullOrEmpty(chkT74.Text) && !string.IsNullOrEmpty(chkC74.Text))
            {
                string t = chkT74.Text;
                string c = chkC74.Text;
                string location = "74";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT74.Text))
            {
                string t = chkT74.Text;
                string location = "74";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC74.Text))
            {
                string c = chkC74.Text;
                string location = "74";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "74";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </74>

            // <75>
            if (!string.IsNullOrEmpty(chkT75.Text) && !string.IsNullOrEmpty(chkC75.Text))
            {
                string t = chkT75.Text;
                string c = chkC75.Text;
                string location = "75";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`, Treatment) VALUES('{0}', '{1}', '{2}', '{3}')", Patient_ID, location, c, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkT75.Text))
            {
                string t = chkT75.Text;
                string location = "75";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, Treatment) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, t);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else if (!string.IsNullOrEmpty(chkC75.Text))
            {
                string c = chkC75.Text;
                string location = "75";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location, `Condition`) VALUES('{0}', '{1}', '{2}')", Patient_ID, location, c);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            else
            {
                string location = "75";

                CN.Open();
                Com.CommandText = string.Format("INSERT INTO teeth(Patient_ID, Location) VALUES('{0}', '{1}')", Patient_ID, location);
                Com.ExecuteNonQuery();
                CN.Close();
            }
            // </75>

            
            // Stores what part of the teeth is affected
            string[] parts = { "55", "54", "53", "52", "51", "61", "62", "63", "64", "65", "18",
                "17", "16", "15", "14", "13", "12", "11", "21", "22", "23", "24", "25", "26",
                "27", "28", "48", "47", "46", "45", "44", "43", "42", "41", "31", "32", "33",
                "34", "35", "36", "37", "38", "85", "84", "83", "82", "81", "71", "72", "73",
                "74", "75" };

            for (int i = 0; i < 52; i++)
            {
                // Gets the Medical ID that corresponds to the Patient ID
                CN.Open();
                Com.CommandText = string.Format("SELECT ID FROM teeth WHERE Patient_ID = '{0}' AND Location = '{1}'", Patient_ID, parts[i]);
                reader = Com.ExecuteReader();
                string Teeth_ID = "";
                if (reader.Read())
                {
                    Teeth_ID = reader["ID"].ToString();
                }
                reader.Close();
                CN.Close();

                for (int j = 0; j < 5; j++)
                {
                    if (!string.IsNullOrEmpty(teeth[i, j]))
                    {
                        CN.Open();
                        Com.CommandText = string.Format("INSERT INTO teeth_part(Teeth_ID, Part_ID) VALUES('{0}', '{1}')", Teeth_ID, teeth[i, j]);
                        Com.ExecuteNonQuery();
                        CN.Close();
                    }
                }
            }

            // Increment counter in SQL
            CN.Open();
            Com.CommandText = string.Format("UPDATE counter SET Patient_Count = Patient_Count + 1");
            Com.ExecuteNonQuery();
            CN.Close();

            if (MessageBox.Show("Successfully Added Patient!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
            {
                Add_Done?.Invoke(sender, e);
            }
        }

        // ========================= Custom Functions =========================
        // Checks if there are any active checkbox
        private bool hasContent(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.GetType() == typeof(CheckBox) && ((CheckBox)c).Checked)
                {
                    return true;
                }
            }

            return false;
        }

        // Gives the list of checkboxes that are checked
        private string[] activeBoxList(Control parent)
        {
            string[] list = Array.Empty<string>();

            foreach (Control c in parent.Controls)
            {
                if (c.GetType() == typeof(CheckBox) && ((CheckBox)c).Checked)
                {
                    Array.Resize(ref list, list.Length + 1);
                    list[list.GetUpperBound(0)] = ((CheckBox)c).Text;
                }
            }

            return list;
        }

        // Loads selected parts into array
        private void Load_Teeth(object sender, EventArgs e)
        {
            string[] parts = { "55", "54", "53", "52", "51", "61", "62", "63", "64", "65", "18",
                "17", "16", "15", "14", "13", "12", "11", "21", "22", "23", "24", "25", "26",
                "27", "28", "48", "47", "46", "45", "44", "43", "42", "41", "31", "32", "33",
                "34", "35", "36", "37", "38", "85", "84", "83", "82", "81", "71", "72", "73",
                "74", "75" };

            foreach (string part in parts)
            {
                var index = Array.FindIndex(parts, row => row.Contains(part));
                if (teethNumber == part)
                {
                    List<string> localTeeth = Teeth.teeth;
                    for (int i = 0; i < localTeeth.Count(); i++)
                    {
                        teeth[index, i] = localTeeth[i];
                    }
                    return;
                }
            }
        }
        // Checks if any textbox are empty
        private bool textboxHasContent(Control parent)
        {
            int textBox = 0;
            int dropBox = 0;
            foreach (Control c in parent.Controls)
            {
                if (c.GetType() == typeof(Bunifu.UI.WinForms.BunifuTextBox) && !string.IsNullOrEmpty(((Bunifu.UI.WinForms.BunifuTextBox)c).Text))
                {
                    textBox++;
                }
                if (c.GetType() == typeof(Bunifu.UI.WinForms.BunifuDropdown) && !string.IsNullOrEmpty(((Bunifu.UI.WinForms.BunifuDropdown)c).Text))
                {
                    dropBox++;
                }
            }

            if (textBox == 6 && dropBox == 4)
            {
                return true;
            }
            return false;
        }

        // Checks if text is a digit
        private static bool IsDigit(string input)
        {
            foreach (char c in input)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }


        // -----------------[Teeth Settings]--------------------
        private void btnTeeth55_Click(object sender, EventArgs e)
        {
            // Prompts user to choose a part of the tooth
            teethNumber = "55";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            // Changes the button picture base on the selected part
            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth55.BackgroundImage = bmp;
        }

        private void btnTeeth54_Click(object sender, EventArgs e)
        {
            teethNumber = "54";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth54.BackgroundImage = bmp;
        }

        private void btnTeeth53_Click(object sender, EventArgs e)
        {
            teethNumber = "53";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth53.BackgroundImage = bmp;
        }

        private void btnTeeth52_Click(object sender, EventArgs e)
        {
            teethNumber = "52";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth52.BackgroundImage = bmp;
        }

        private void btnTeeth51_Click(object sender, EventArgs e)
        {
            teethNumber = "51";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth51.BackgroundImage = bmp;
        }

        private void btnTeeth61_Click(object sender, EventArgs e)
        {
            teethNumber = "61";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth61.BackgroundImage = bmp;
        }

        private void btnTeeth62_Click(object sender, EventArgs e)
        {
            teethNumber = "62";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth62.BackgroundImage = bmp;
        }

        private void btnTeeth63_Click(object sender, EventArgs e)
        {
            teethNumber = "63";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth63.BackgroundImage = bmp;
        }

        private void btnTeeth64_Click(object sender, EventArgs e)
        {
            teethNumber = "64";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth64.BackgroundImage = bmp;
        }

        private void btnTeeth65_Click(object sender, EventArgs e)
        {
            teethNumber = "65";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth65.BackgroundImage = bmp;
        }

        private void btnTeeth18_Click(object sender, EventArgs e)
        {
            teethNumber = "18";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth18.BackgroundImage = bmp;
        }

        private void btnTeeth17_Click(object sender, EventArgs e)
        {
            teethNumber = "17";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth17.BackgroundImage = bmp;
        }

        private void btnTeeth16_Click(object sender, EventArgs e)
        {
            teethNumber = "16";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth16.BackgroundImage = bmp;
        }

        private void btnTeeth15_Click(object sender, EventArgs e)
        {
            teethNumber = "15";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth15.BackgroundImage = bmp;
        }

        private void btnTeeth14_Click(object sender, EventArgs e)
        {
            teethNumber = "14";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth14.BackgroundImage = bmp;
        }

        private void btnTeeth13_Click(object sender, EventArgs e)
        {
            teethNumber = "13";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth13.BackgroundImage = bmp;
        }

        private void btnTeeth12_Click(object sender, EventArgs e)
        {
            teethNumber = "12";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth12.BackgroundImage = bmp;
        }

        private void btnTeeth11_Click(object sender, EventArgs e)
        {
            teethNumber = "11";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth11.BackgroundImage = bmp;
        }

        private void btnTeeth48_Click(object sender, EventArgs e)
        {
            teethNumber = "48";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth48.BackgroundImage = bmp;
        }

        private void btnTeeth47_Click(object sender, EventArgs e)
        {
            teethNumber = "47";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth47.BackgroundImage = bmp;
        }

        private void btnTeeth46_Click(object sender, EventArgs e)
        {
            teethNumber = "46";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth46.BackgroundImage = bmp;
        }

        private void btnTeeth45_Click(object sender, EventArgs e)
        {
            teethNumber = "45";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth45.BackgroundImage = bmp;
        }

        private void btnTeeth44_Click(object sender, EventArgs e)
        {
            teethNumber = "44";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth44.BackgroundImage = bmp;
        }

        private void btnTeeth43_Click(object sender, EventArgs e)
        {
            teethNumber = "43";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth43.BackgroundImage = bmp;
        }

        private void btnTeeth42_Click(object sender, EventArgs e)
        {
            teethNumber = "42";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth42.BackgroundImage = bmp;
        }

        private void btnTeeth41_Click(object sender, EventArgs e)
        {
            teethNumber = "41";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth41.BackgroundImage = bmp;
        }

        private void btnTeeth21_Click(object sender, EventArgs e)
        {
            teethNumber = "21";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth21.BackgroundImage = bmp;
        }

        private void btnTeeth22_Click(object sender, EventArgs e)
        {
            teethNumber = "22";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth22.BackgroundImage = bmp;
        }

        private void btnTeeth23_Click(object sender, EventArgs e)
        {
            teethNumber = "23";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth23.BackgroundImage = bmp;
        }

        private void btnTeeth24_Click(object sender, EventArgs e)
        {
            teethNumber = "24";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth24.BackgroundImage = bmp;
        }

        private void btnTeeth25_Click(object sender, EventArgs e)
        {
            teethNumber = "25";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth25.BackgroundImage = bmp;
        }

        private void btnTeeth26_Click(object sender, EventArgs e)
        {
            teethNumber = "26";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth26.BackgroundImage = bmp;
        }

        private void btnTeeth27_Click(object sender, EventArgs e)
        {
            teethNumber = "27";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth27.BackgroundImage = bmp;
        }

        private void btnTeeth28_Click(object sender, EventArgs e)
        {
            teethNumber = "28";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth28.BackgroundImage = bmp;
        }

        private void btnTeeth31_Click(object sender, EventArgs e)
        {
            teethNumber = "31";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth31.BackgroundImage = bmp;
        }

        private void btnTeeth32_Click(object sender, EventArgs e)
        {
            teethNumber = "32";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth32.BackgroundImage = bmp;
        }

        private void btnTeeth33_Click(object sender, EventArgs e)
        {
            teethNumber = "33";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth33.BackgroundImage = bmp;
        }

        private void btnTeeth34_Click(object sender, EventArgs e)
        {
            teethNumber = "34";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth34.BackgroundImage = bmp;
        }

        private void btnTeeth35_Click(object sender, EventArgs e)
        {
            teethNumber = "35";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth35.BackgroundImage = bmp;
        }

        private void btnTeeth36_Click(object sender, EventArgs e)
        {
            teethNumber = "36";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth36.BackgroundImage = bmp;
        }

        private void btnTeeth37_Click(object sender, EventArgs e)
        {
            teethNumber = "37";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth37.BackgroundImage = bmp;
        }

        private void btnTeeth38_Click(object sender, EventArgs e)
        {
            teethNumber = "38";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth38.BackgroundImage = bmp;
        }

        private void btnTeeth85_Click(object sender, EventArgs e)
        {
            teethNumber = "85";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth85.BackgroundImage = bmp;
        }

        private void btnTeeth84_Click(object sender, EventArgs e)
        {
            teethNumber = "84";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth84.BackgroundImage = bmp;
        }

        private void btnTeeth83_Click(object sender, EventArgs e)
        {
            teethNumber = "83";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth83.BackgroundImage = bmp;
        }

        private void btnTeeth82_Click(object sender, EventArgs e)
        {
            teethNumber = "82";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth82.BackgroundImage = bmp;
        }

        private void btnTeeth81_Click(object sender, EventArgs e)
        {
            teethNumber = "81";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth81.BackgroundImage = bmp;
        }

        private void btnTeeth71_Click(object sender, EventArgs e)
        {
            teethNumber = "71";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth71.BackgroundImage = bmp;
        }

        private void btnTeeth72_Click(object sender, EventArgs e)
        {
            teethNumber = "72";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth72.BackgroundImage = bmp;
        }

        private void btnTeeth73_Click(object sender, EventArgs e)
        {
            teethNumber = "73";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth73.BackgroundImage = bmp;
        }

        private void btnTeeth74_Click(object sender, EventArgs e)
        {
            teethNumber = "74";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth74.BackgroundImage = bmp;
        }

        private void btnTeeth75_Click(object sender, EventArgs e)
        {
            teethNumber = "75";
            Teeth teeth = new Teeth();
            teeth.lblToothNumber.Text = "Tooth " + teethNumber;
            teeth.ShowDialog();

            string pic = Teeth.picName;
            Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
            btnTeeth75.BackgroundImage = bmp;
        }
    }
}
