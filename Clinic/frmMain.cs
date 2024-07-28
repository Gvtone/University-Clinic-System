using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public partial class frmMain : Form
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        // Takes note of what button is selected
        public static string currentTab;
        private static bool isTopBarLoaded = false;
        DatagridMain dtgMain = new DatagridMain();

        // Event Handlers
        public static EventHandler Patient_Clicked;
        public static EventHandler Medicine_Clicked;
        public static EventHandler Tools_Clicked;
        public static EventHandler Inventory_Clicked;

        public static EventHandler Load_ModifyMedicine;
        public static EventHandler Load_ModifyTools;
        public static EventHandler Load_ModifyInventory;

        public static EventHandler Change_Button;

        public frmMain()
        {
            InitializeComponent();
            Com.Connection = CN;

            TopBar.Overview_Clicked += new EventHandler(Load_Grid);
            TopBar.Add_Clicked += new EventHandler(Load_Form);
            TopBar.Proceed_Print += new EventHandler(print);
            SubPatient.Add_Done += new EventHandler(btnPatient_Click);
            Medicine.Add_Done += new EventHandler(btnMedicine_Click);
            Tools.Add_Done += new EventHandler(btnTools_Click);
            Inventory.Add_Done += new EventHandler(btnInventory_Click);
            TopBar.Load_Panel += new EventHandler(Load_ModifyPanel);
            TopBar.Open_Patient += new EventHandler(btnPatient_Click);
        }

        // Loads first user control
        private void frmMain_Load(object sender, EventArgs e)
        {
            btnHome.PerformClick();
        }

        // Closes the application
        private void btnClose_Click(object sender, EventArgs e)
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

        // Minimize the window
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // <Right Panel>
        // Home button
        private void btnHome_Click(object sender, EventArgs e)
        {
            // Updates main panel
            pnlMain.Controls.Clear();
            Home home = new Home();
            pnlMain.Controls.Add(home);
            home.Show();

            if (isTopBarLoaded == true)
            {
                // Hides table options
                pnlHeader.Controls.Clear();
                isTopBarLoaded = false;
            }

            // Set current tab to this button
            currentTab = "Home";
            Change_Button?.Invoke(sender, e);
        }

        // Patient button
        private void btnPatient_Click(object sender, EventArgs e)
        {
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(dtgMain);
            dtgMain.Show();

            if (isTopBarLoaded == false)
            {
                // Shows table options
                pnlHeader.Controls.Clear();
                TopBar tb = new TopBar();
                pnlHeader.Controls.Add(tb);
                tb.Show();

                isTopBarLoaded = true;
            }
            
            currentTab = "Patient";

            /* Sends signal that button has been clicked
               Prepares the datagrid to show correct data*/
            Patient_Clicked?.Invoke(sender, e);
            Change_Button?.Invoke(sender, e);
        }

        // Medicine button
        private void btnMedicine_Click(object sender, EventArgs e)
        {
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(dtgMain);
            dtgMain.Show();

            if (isTopBarLoaded == false)
            {
                pnlHeader.Controls.Clear();
                TopBar tb = new TopBar();
                pnlHeader.Controls.Add(tb);
                tb.Show();

                isTopBarLoaded = true;
            }

            currentTab = "Medicine";
            Medicine_Clicked?.Invoke(sender, e);
            Change_Button?.Invoke(sender, e);
        }

        // Tools Button
        private void btnTools_Click(object sender, EventArgs e)
        {
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(dtgMain);
            dtgMain.Show();

            if (isTopBarLoaded == false)
            {
                pnlHeader.Controls.Clear();
                TopBar tb = new TopBar();
                pnlHeader.Controls.Add(tb);
                tb.Show();

                isTopBarLoaded = true;
            }

            currentTab = "Tools";
            Tools_Clicked?.Invoke(sender, e);
            Change_Button?.Invoke(sender, e);
        }

        // Inventory button
        private void btnInventory_Click(object sender, EventArgs e)
        {
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(dtgMain);
            dtgMain.Show();

            if (isTopBarLoaded == false)
            {
                pnlHeader.Controls.Clear();
                TopBar tb = new TopBar();
                pnlHeader.Controls.Add(tb);
                tb.Show();

                isTopBarLoaded = true;
            }

            currentTab = "Inventory";
            Inventory_Clicked?.Invoke(sender, e);
            Change_Button?.Invoke(sender, e);
        }
        // </Right Panel>

        // Load datagrid with respect to the current tab
        private void Load_Grid(object sender, EventArgs e)
        {
            pnlMain.Controls.Clear();
            pnlMain.Controls.Add(dtgMain);
            dtgMain.Show();

            if (currentTab == "Patient")
            {
                Patient_Clicked?.Invoke(sender, e);
            }
            else if (currentTab == "Medicine")
            {
                Medicine_Clicked?.Invoke(sender, e);
            }
            else if (currentTab == "Tools")
            {
                Tools_Clicked?.Invoke(sender, e);
            }
            else if (currentTab == "Inventory")
            {
                Inventory_Clicked?.Invoke(sender, e);
            }
        }

        // Load form with respect to the current tab
        private void Load_Form(object sender, EventArgs e)
        {
            if (currentTab == "Patient")
            {
                pnlMain.Controls.Clear();
                Patient patient = new Patient();
                pnlMain.Controls.Add(patient);
                patient.Show();
            }
            else if (currentTab == "Medicine")
            {
                pnlMain.Controls.Clear();
                Medicine medicine = new Medicine();
                pnlMain.Controls.Add(medicine);
                medicine.Show();
            }
            else if (currentTab == "Tools")
            {
                pnlMain.Controls.Clear();
                Tools tools = new Tools();
                pnlMain.Controls.Add(tools);
                tools.Show();
            }
            else if (currentTab == "Inventory")
            {
                pnlMain.Controls.Clear();
                Inventory inventory = new Inventory();
                pnlMain.Controls.Add(inventory);
                inventory.Show();
            }
        }

        private void Load_ModifyPanel(object sender, EventArgs e)
        {
            if (currentTab == "Medicine")
            {
                pnlMain.Controls.Clear();
                Medicine medicine = new Medicine();
                pnlMain.Controls.Add(medicine);
                medicine.Show();
                Load_ModifyMedicine?.Invoke(sender, e);
            }
            else if (currentTab == "Tools")
            {
                pnlMain.Controls.Clear();
                Tools tools = new Tools();
                pnlMain.Controls.Add(tools);
                Load_ModifyTools?.Invoke(sender, e);
                tools.Show();
            }
            else if (currentTab == "Inventory")
            {
                pnlMain.Controls.Clear();
                Inventory inventory = new Inventory();
                pnlMain.Controls.Add(inventory);
                Load_ModifyInventory?.Invoke(sender, e);
                inventory.Show();
            }
        }

        // Printing
        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (currentTab == "Patient")
            {
                string name = DatagridMain.name;
                string date = DatagridMain.date;
                string patientID = "";
                CN.Open();
                Com.CommandText = string.Format("SELECT ID FROM patient WHERE NAME = '{0}' AND Date_Added = '{1}'", name, date);
                reader = Com.ExecuteReader();
                if (reader.Read())
                {
                    patientID = reader["ID"].ToString();
                }
                reader.Close();
                CN.Close();

                // <-------------------------------------------Patient------------------------------------------->
                string patientname = "";
                string address = "";
                string birthday = "";
                string posistion = "";
                string course = "";
                string age = "";
                string sex = "";
                string civilStatus = "";
                string contact = "";
                string yearSection = "";
                string dateAdded = "";

                CN.Open();
                Com.CommandText = string.Format("SELECT * FROM patient WHERE ID = '{0}'", patientID);
                reader = Com.ExecuteReader();
                if (reader.Read())
                {
                    patientname = reader["Name"].ToString();
                    address = reader["Address"].ToString();
                    DateTime set1 = (DateTime)reader["Birthday"];
                    string formDate1 = set1.ToString("MMMM dd, yyyy");
                    birthday = formDate1;
                    posistion = reader["Position"].ToString();
                    course = reader["Course"].ToString();
                    age = reader["Age"].ToString();
                    sex = reader["Sex"].ToString();
                    civilStatus = reader["Civil_Status"].ToString();
                    contact = reader["Contact_Number"].ToString();
                    yearSection = reader["Year_Section"].ToString();

                    DateTime set2 = (DateTime)reader["Date_Added"];
                    string formDate2 = set2.ToString("MMMM dd, yyyy");
                    dateAdded = formDate2;
                }
                reader.Close();
                CN.Close();

                // Personal Background
                e.Graphics.DrawImage(Properties.Resources.Dental_Record, 0, 0, 827, 1170);
                e.Graphics.DrawString(dateAdded, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(581, 267));
                e.Graphics.DrawString(patientname, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(139, 308));
                e.Graphics.DrawString(address, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(154, 322));
                e.Graphics.DrawString(birthday, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(154, 336));
                e.Graphics.DrawString(posistion, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(153, 350));
                e.Graphics.DrawString(course, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(147, 363));
                e.Graphics.DrawString(age, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(580, 309));
                e.Graphics.DrawString(sex, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(579, 322));
                e.Graphics.DrawString(civilStatus, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(620, 336));
                e.Graphics.DrawString(contact, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(610, 350));
                e.Graphics.DrawString(yearSection, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(608, 364));
                // <-------------------------------------------Patient------------------------------------------->

                // <-------------------------------------------Medical------------------------------------------->
                string hospitalization = "";
                string bp = "";

                CN.Open();
                Com.CommandText = string.Format("SELECT * FROM medical WHERE Patient_ID = '{0}'", patientID);
                reader = Com.ExecuteReader();
                if (reader.Read())
                {
                    hospitalization = reader["Hospitalization"].ToString();
                    bp = reader["BP"].ToString();
                }
                reader.Close();
                CN.Close();

                // Medical History
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 121, 406, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 249, 406, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 389, 406, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 486, 406, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 602, 406, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 121, 419, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 249, 419, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 389, 419, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 486, 419, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 602, 419, 20, 10);

                e.Graphics.DrawString(hospitalization, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(229, 472));
                e.Graphics.DrawString(bp, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(570, 472));

                string historyMedical;
                CN.Open();
                Com.CommandText = string.Format("SELECT type.History " +
                    "FROM medical, medical_type, `type` " +
                    "WHERE medical.ID = medical_type.Medical_ID " +
                    "AND medical_type.Type_ID = type.ID " +
                    "AND medical.Patient_ID = {0}", patientID);
                reader = Com.ExecuteReader();
                while (reader.Read())
                {
                    historyMedical = reader["History"].ToString();

                    if (historyMedical == "Allergies")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 121, 406, 20, 10);
                    }
                    else if (historyMedical == "Kidney Disease")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 249, 406, 20, 10);
                    }
                    else if (historyMedical == "Diabetes")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 389, 406, 20, 10);
                    }
                    else if (historyMedical == "Asthma")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 486, 406, 20, 10);
                    }
                    else if (historyMedical == "Primary Complex")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 602, 406, 20, 10);
                    }
                    else if (historyMedical == "Heart Disease")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 121, 419, 20, 10);
                    }
                    else if (historyMedical == "Pneumonia")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 249, 419, 20, 10);
                    }
                    else if (historyMedical == "Anemia")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 389, 419, 20, 10);
                    }
                    else if (historyMedical == "Bronchitis")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 486, 419, 20, 10);
                    }
                    else if (historyMedical == "Tuberculosis")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 602, 419, 20, 10);
                    }
                    else
                    {
                        e.Graphics.DrawString(historyMedical, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(357, 444));
                    }
                }
                reader.Close();
                CN.Close();
                // <-------------------------------------------Medical------------------------------------------->

                // <-------------------------------------------Dental------------------------------------------->
                // Dental History
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 192, 514, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 314, 514, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 439, 514, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 192, 527, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 314, 527, 20, 10);
                e.Graphics.DrawImage(Properties.Resources.Unchecked, 439, 527, 20, 10);

                string historyDental;
                CN.Open();
                Com.CommandText = string.Format("SELECT dental.History " +
                    "FROM patient, patient_dental, dental " +
                    "WHERE patient.ID = patient_dental.Patient_ID " +
                    "AND patient_dental.Dental_ID = dental.ID " +
                    "AND patient.ID = {0}", patientID);
                reader = Com.ExecuteReader();
                while (reader.Read())
                {
                    historyDental = reader["History"].ToString();

                    if (historyDental == "Gingivitis")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 192, 514, 20, 10);
                    }
                    else if (historyDental == "Ankylosis")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 314, 514, 20, 10);
                    }
                    else if (historyDental == "Ankyloglossia")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 439, 514, 20, 10);
                    }
                    else if (historyDental == "Pyorrhea")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 192, 527, 20, 10);
                    }
                    else if (historyDental == "Her Lip")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 314, 527, 20, 10);
                    }
                    else if (historyDental == "Cleft Plate")
                    {
                        e.Graphics.DrawImage(Properties.Resources.Checked, 439, 527, 20, 10);
                    }
                    else
                    {
                        e.Graphics.DrawString(historyDental, new Font("Arial", 8, FontStyle.Bold), Brushes.Black, new Point(603, 513));
                    }
                }
                reader.Close();
                CN.Close();
                // <-------------------------------------------Dental------------------------------------------->

                // <-------------------------------------------Teeth------------------------------------------->
                string location, treatment, condition;
                CN.Open();
                Com.CommandText = string.Format("SELECT Location, `Condition`, Treatment FROM teeth WHERE Patient_ID = {0}", patientID);
                reader = Com.ExecuteReader();
                while (reader.Read())
                {
                    location = reader["Location"].ToString();
                    treatment = reader["Treatment"].ToString();
                    condition = reader["Condition"].ToString();
                    if (location == "55")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 579));
                    }
                    else if (location == "54")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 579));
                    }
                    else if (location == "53")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 579));
                    }
                    else if (location == "52")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 579));
                    }
                    else if (location == "51")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 579));
                    }
                    else if (location == "61")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 579));
                    }
                    else if (location == "62")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 579));
                    }
                    else if (location == "63")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 579));
                    }
                    else if (location == "64")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 579));
                    }
                    else if (location == "65")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 558));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 579));
                    }
                    else if (location == "18")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(209, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(209, 676));
                    }
                    else if (location == "17")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(239, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(239, 676));
                    }
                    else if (location == "16")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(269, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(269, 676));
                    }
                    else if (location == "15")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 676));
                    }
                    else if (location == "14")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 676));
                    }
                    else if (location == "13")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 676));
                    }
                    else if (location == "12")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 676));
                    }
                    else if (location == "11")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 676));
                    }
                    else if (location == "21")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 676));
                    }
                    else if (location == "22")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 676));
                    }
                    else if (location == "23")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 676));
                    }
                    else if (location == "24")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 676));
                    }
                    else if (location == "25")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 676));
                    }
                    else if (location == "26")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(599, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(599, 676));
                    }
                    else if (location == "27")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(629, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(629, 676));
                    }
                    else if (location == "28")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(659, 656));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(659, 676));
                    }
                    else if (location == "48")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(209, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(209, 780));
                    }
                    else if (location == "47")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(239, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(239, 780));
                    }
                    else if (location == "46")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(269, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(269, 780));
                    }
                    else if (location == "45")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 780));
                    }
                    else if (location == "44")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 780));
                    }
                    else if (location == "43")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 780));
                    }
                    else if (location == "42")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 780));
                    }
                    else if (location == "41")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 780));
                    }
                    else if (location == "31")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 780));
                    }
                    else if (location == "32")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 780));
                    }
                    else if (location == "33")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 780));
                    }
                    else if (location == "34")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 780));
                    }
                    else if (location == "35")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 780));
                    }
                    else if (location == "36")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(599, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(599, 780));
                    }
                    else if (location == "37")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(629, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(629, 780));
                    }
                    else if (location == "38")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(659, 801));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(659, 780));
                    }
                    else if (location == "85")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(299, 877));
                    }
                    else if (location == "84")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(329, 877));
                    }
                    else if (location == "83")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(359, 877));
                    }
                    else if (location == "82")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(389, 877));
                    }
                    else if (location == "81")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(419, 877));
                    }
                    else if (location == "71")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(449, 877));
                    }
                    else if (location == "72")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(479, 877));
                    }
                    else if (location == "73")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(509, 877));
                    }
                    else if (location == "74")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(539, 877));
                    }
                    else if (location == "75")
                    {
                        e.Graphics.DrawString(treatment, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 898));
                        e.Graphics.DrawString(condition, new Font("Arial", 7, FontStyle.Bold), Brushes.Black, new Point(569, 877));
                    }
                }
                reader.Close();
                CN.Close();

                // Set of Teeth 1
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 299, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 329, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 359, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 389, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 419, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 449, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 479, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 509, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 539, 616, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 569, 616, 28, 20);

                // Set of Teeth 2
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 209, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 239, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 269, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 299, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 329, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 359, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 389, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 419, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 449, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 479, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 509, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 539, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 569, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 599, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 629, 713, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 659, 713, 28, 20);

                // Set of Teeth 2 Part 2
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 209, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 239, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 269, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 299, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 329, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 359, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 389, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 419, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 449, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 479, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 509, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 539, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 569, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 599, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 629, 734, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 659, 734, 28, 20);

                // Set of Teeth 3
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 299, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 329, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 359, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 389, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 419, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 449, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 479, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 509, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 539, 831, 28, 20);
                e.Graphics.DrawImage(Properties.Resources.toothPrint, 569, 831, 28, 20);

                // Generates image names base on the selected parts in the database
                List<string> teethImage = new List<string>();
                string imageName = "toothPrint", currentTeeth = "", currentlyReading;
                CN.Open();
                Com.CommandText = string.Format("SELECT teeth.Location, part.Specific " +
                    "FROM teeth, teeth_part, part " +
                    "WHERE teeth.ID = teeth_part.Teeth_ID " +
                    "AND teeth_part.Part_ID = part.ID " +
                    "AND teeth.Patient_ID = {0} " +
                    "ORDER BY teeth.Location, part.Specific", patientID);
                reader = Com.ExecuteReader();
                while (reader.Read())
                {
                    currentlyReading = reader["Location"].ToString();
                    if (string.IsNullOrEmpty(currentTeeth))
                    {
                        currentTeeth = currentlyReading;
                    }

                    if (currentlyReading == currentTeeth)
                    {
                        imageName += reader["Specific"].ToString();
                    }
                    else
                    {
                        teethImage.Add(imageName);
                        currentTeeth = currentlyReading;
                        imageName = "toothPrint";
                        imageName += reader["Specific"].ToString();
                    }
                }
                teethImage.Add(imageName);
                reader.Close();
                CN.Close();

                string imageLocation;
                int imageCounter = 0;
                CN.Open();
                Com.CommandText = string.Format("SELECT DISTINCT(teeth.Location) " +
                    "FROM teeth, teeth_part, part " +
                    "WHERE teeth.ID = teeth_part.Teeth_ID " +
                    "AND teeth_part.Part_ID = part.ID " +
                    "AND teeth.Patient_ID = {0} " +
                    "ORDER BY teeth.Location", patientID);
                reader = Com.ExecuteReader();
                while (reader.Read())
                {
                    imageLocation = reader["Location"].ToString();

                    if (imageLocation == "55")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 299, 616, 28, 20);
                    }
                    else if (imageLocation == "54")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 329, 616, 28, 20);
                    }
                    else if (imageLocation == "53")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 359, 616, 28, 20);
                    }
                    else if (imageLocation == "52")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 389, 616, 28, 20);
                    }
                    else if (imageLocation == "51")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 419, 616, 28, 20);
                    }
                    else if (imageLocation == "61")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 449, 616, 28, 20);
                    }
                    else if (imageLocation == "62")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 479, 616, 28, 20);
                    }
                    else if (imageLocation == "63")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 509, 616, 28, 20);
                    }
                    else if (imageLocation == "64")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 539, 616, 28, 20);
                    }
                    else if (imageLocation == "65")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 569, 616, 28, 20);
                    }
                    else if (imageLocation == "18")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 209, 713, 28, 20);
                    }
                    else if (imageLocation == "17")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 239, 713, 28, 20);
                    }
                    else if (imageLocation == "16")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 269, 713, 28, 20);
                    }
                    else if (imageLocation == "15")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 299, 713, 28, 20);
                    }
                    else if (imageLocation == "14")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 329, 713, 28, 20);
                    }
                    else if (imageLocation == "13")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 359, 713, 28, 20);
                    }
                    else if (imageLocation == "12")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 389, 713, 28, 20);
                    }
                    else if (imageLocation == "11")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 419, 713, 28, 20);
                    }
                    else if (imageLocation == "21")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 449, 713, 28, 20);
                    }
                    else if (imageLocation == "22")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 479, 713, 28, 20);
                    }
                    else if (imageLocation == "23")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 509, 713, 28, 20);
                    }
                    else if (imageLocation == "24")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 539, 713, 28, 20);
                    }
                    else if (imageLocation == "25")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 569, 713, 28, 20);
                    }
                    else if (imageLocation == "26")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 599, 713, 28, 20);
                    }
                    else if (imageLocation == "27")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 629, 713, 28, 20);
                    }
                    else if (imageLocation == "28")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 659, 713, 28, 20);
                    }
                    else if (imageLocation == "48")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 209, 734, 28, 20);
                    }
                    else if (imageLocation == "47")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 239, 734, 28, 20);
                    }
                    else if (imageLocation == "46")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 269, 734, 28, 20);
                    }
                    else if (imageLocation == "45")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 299, 734, 28, 20);
                    }
                    else if (imageLocation == "44")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 329, 734, 28, 20);
                    }
                    else if (imageLocation == "43")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 359, 734, 28, 20);
                    }
                    else if (imageLocation == "42")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 389, 734, 28, 20);
                    }
                    else if (imageLocation == "41")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 419, 734, 28, 20);
                    }
                    else if (imageLocation == "31")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 449, 734, 28, 20);
                    }
                    else if (imageLocation == "32")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 479, 734, 28, 20);
                    }
                    else if (imageLocation == "33")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 509, 734, 28, 20);
                    }
                    else if (imageLocation == "34")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 539, 734, 28, 20);
                    }
                    else if (imageLocation == "35")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 569, 734, 28, 20);
                    }
                    else if (imageLocation == "36")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 599, 734, 28, 20);
                    }
                    else if (imageLocation == "37")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 629, 734, 28, 20);
                    }
                    else if (imageLocation == "38")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 659, 734, 28, 20);
                    }
                    else if (imageLocation == "85")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 299, 831, 28, 20);
                    }
                    else if (imageLocation == "84")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 329, 831, 28, 20);
                    }
                    else if (imageLocation == "83")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 359, 831, 28, 20);
                    }
                    else if (imageLocation == "82")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 389, 831, 28, 20);
                    }
                    else if (imageLocation == "81")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 419, 831, 28, 20);
                    }
                    else if (imageLocation == "71")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 449, 831, 28, 20);
                    }
                    else if (imageLocation == "72")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 479, 831, 28, 20);
                    }
                    else if (imageLocation == "73")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 509, 831, 28, 20);
                    }
                    else if (imageLocation == "74")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 539, 831, 28, 20);
                    }
                    else if (imageLocation == "75")
                    {
                        string pic = teethImage[imageCounter];
                        Bitmap bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject(pic);
                        e.Graphics.DrawImage(bmp, 569, 831, 28, 20);
                    }

                    imageCounter++;
                }
                reader.Close();
                CN.Close();
                // <-------------------------------------------Teeth------------------------------------------->
            }
        }

        private void print(object sender, EventArgs e)
        {
            PrintPreviewDialog dialog = new PrintPreviewDialog();
            dialog.PrintPreviewControl.Zoom = 1;
            dialog.Document = printDocument1;
            printDocument1.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1170);
            Margins margins = new Margins(100, 100, 100, 100);
            printDocument1.DefaultPageSettings.Margins = margins;

            dialog.ShowDialog();
        }
    }
}
