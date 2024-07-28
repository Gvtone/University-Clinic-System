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

namespace Clinic
{
    public partial class DatagridMain : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        public static string name, date;
        public static string medicineName, medicineBrand;
        public static string itemName, itemBrand;
        public static string toolName;
        public static int numOfRow;

        public DatagridMain()
        {
            InitializeComponent();
            Com.Connection = CN;

            frmMain.Patient_Clicked += new EventHandler(Patient_Set);
            frmMain.Medicine_Clicked += new EventHandler(Medicine_Set);
            frmMain.Tools_Clicked += new EventHandler(Tools_Set);
            frmMain.Inventory_Clicked += new EventHandler(Inventory_Set);
            TopBar.Print_Clicked += new EventHandler(Set_NumberofSelectedRow);
            TopBar.Get_Row += new EventHandler(Get_SelectedRow);
            TopBar.Modified_Clicked += new EventHandler(Set_NumberofSelectedRow);
        }

        // Datagrid for Patients
        private void Patient_Set(object sender, EventArgs e)
        {
            dtgMain.DataSource = null;
            dtgMain.Rows.Clear();
            dtgMain.Columns.Clear();
            dtgMain.Refresh();

            // Table display text
            lblTitle.Text = "Patient";

            // Setting column names
            dtgMain.Rows.Clear();
            dtgMain.ColumnCount = 11;
            dtgMain.Columns[0].Name = "Name";
            dtgMain.Columns[1].Name = "Address";
            dtgMain.Columns[2].Name = "Birthday";
            dtgMain.Columns[3].Name = "Position";
            dtgMain.Columns[4].Name = "Course";
            dtgMain.Columns[5].Name = "Age";
            dtgMain.Columns[6].Name = "Sex";
            dtgMain.Columns[7].Name = "Civil Status";
            dtgMain.Columns[8].Name = "Contact Number";
            dtgMain.Columns[9].Name = "Year & Section";
            dtgMain.Columns[10].Name = "Date Added";

            // Cell settings
            dtgMain.RowTemplate.Height = 30;
            dtgMain.ColumnHeadersHeight = 60;
            dtgMain.Columns[0].Width = 200;
            dtgMain.Columns[1].Width = 110;
            dtgMain.Columns[2].Width = 110;
            dtgMain.Columns[5].Width = 60;
            dtgMain.Columns[6].Width = 70;
            dtgMain.Columns[7].Width = 80;
            dtgMain.Columns[8].Width = 120;
            dtgMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtgMain.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Retrieving data from database
            CN.Open();
            Com.CommandText = "SELECT * FROM patient ORDER BY Name";
            reader = Com.ExecuteReader();
            while (reader.Read())
            {
                // Removes time from date time
                DateTime birthday = (DateTime)reader["Birthday"];
                string date = birthday.ToString("yyyy/MM/dd");

                DateTime dateAdded = (DateTime)reader["Date_Added"];
                string addedDate = dateAdded.ToString("yyyy/MM/dd");

                dtgMain.Rows.Add(reader["Name"].ToString(), reader["Address"].ToString(), date,
                    reader["Position"].ToString(), reader["Course"].ToString(), reader["Age"].ToString(), reader["Sex"].ToString(), reader["Civil_Status"].ToString(),
                    reader["Contact_Number"].ToString(), reader["Year_Section"].ToString(), addedDate);
            }

            // Closing reader
            dtgMain.ClearSelection();
            reader.Close();
            CN.Close();
        }

        // Datagrid for Medicine
        private void Medicine_Set(object sender, EventArgs e)
        {
            dtgMain.DataSource = null;
            dtgMain.Rows.Clear();
            dtgMain.Columns.Clear();
            dtgMain.Refresh();

            lblTitle.Text = "Medicine";

            dtgMain.Rows.Clear();
            dtgMain.ColumnCount = 4;
            dtgMain.Columns[0].Name = "Name";
            dtgMain.Columns[1].Name = "Brand";
            dtgMain.Columns[2].Name = "Stock";
            dtgMain.Columns[3].Name = "Expiration";

            dtgMain.RowTemplate.Height = 30;
            dtgMain.ColumnHeadersHeight = 50;
            dtgMain.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtgMain.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            CN.Open();
            Com.CommandText = "SELECT * FROM medicine ORDER BY Name";
            reader = Com.ExecuteReader();
            while (reader.Read())
            {
                DateTime expiration = (DateTime)reader["Expiration"];
                string date = expiration.ToString("yyyy/MM/dd");

                dtgMain.Rows.Add(reader["Name"].ToString(), reader["Brand"].ToString(), reader["Stock"].ToString(),
                    date);
            }

            dtgMain.ClearSelection();
            reader.Close();
            CN.Close();
        }

        // Datagrid for Tools
        private void Tools_Set(object sender, EventArgs e)
        {
            dtgMain.DataSource = null;
            dtgMain.Rows.Clear();
            dtgMain.Columns.Clear();
            dtgMain.Refresh();

            lblTitle.Text = "Tools";

            dtgMain.Rows.Clear();
            dtgMain.ColumnCount = 3;
            dtgMain.Columns[0].Name = "Name";
            dtgMain.Columns[1].Name = "Calibration";
            dtgMain.Columns[2].Name = "Comment";

            dtgMain.RowTemplate.Height = 30;
            dtgMain.ColumnHeadersHeight = 50;
            dtgMain.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtgMain.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            CN.Open();
            Com.CommandText = "SELECT * FROM tool ORDER BY Name";
            reader = Com.ExecuteReader();
            while (reader.Read())
            {
                DateTime calibration = (DateTime)reader["Calibration"];
                string date = calibration.ToString("yyyy/MM/dd");

                dtgMain.Rows.Add(reader["Name"].ToString(), date, reader["Comment"]);
            }

            dtgMain.ClearSelection();
            reader.Close();
            CN.Close();
        }

        // Datagrid for Inventory
        private void Inventory_Set(object sender, EventArgs e)
        {
            dtgMain.DataSource = null;
            dtgMain.Rows.Clear();
            dtgMain.Columns.Clear();
            dtgMain.Refresh();

            lblTitle.Text = "Inventory";

            dtgMain.Rows.Clear();
            dtgMain.ColumnCount = 4;
            dtgMain.Columns[0].Name = "Name";
            dtgMain.Columns[1].Name = "Brand";
            dtgMain.Columns[2].Name = "Quantity";
            dtgMain.Columns[3].Name = "Purchase Date";

            dtgMain.RowTemplate.Height = 30;
            dtgMain.ColumnHeadersHeight = 60;
            dtgMain.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dtgMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtgMain.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            CN.Open();
            Com.CommandText = "SELECT * FROM supply ORDER BY Name";
            reader = Com.ExecuteReader();
            while (reader.Read())
            {
                DateTime purchase = (DateTime)reader["Purchase"];
                string date = purchase.ToString("yyyy/MM/dd");

                dtgMain.Rows.Add(reader["Name"].ToString(), reader["Brand"].ToString(), reader["Quantity"].ToString(),
                    date);
            }

            dtgMain.ClearSelection();
            reader.Close();
            CN.Close();
        }

        // Get needed details of selected rows
        private void Get_SelectedRow(object sender, EventArgs e)
        {
            if (frmMain.currentTab == "Patient")
            {
                int col0 = dtgMain.Columns["Name"].Index;
                int col10 = dtgMain.Columns["Date Added"].Index;
                name = dtgMain.SelectedCells[col0].Value.ToString();
                date = dtgMain.SelectedCells[col10].Value.ToString();
            }
            else if (frmMain.currentTab == "Medicine")
            {
                int col0 = dtgMain.Columns["Name"].Index;
                int col1 = dtgMain.Columns["Brand"].Index;
                medicineName = dtgMain.SelectedCells[col0].Value.ToString();
                medicineBrand = dtgMain.SelectedCells[col1].Value.ToString();

            }
            else if (frmMain.currentTab == "Tools")
            {
                int col0 = dtgMain.Columns["Name"].Index;
                toolName = dtgMain.SelectedCells[col0].Value.ToString();
            }
            else if (frmMain.currentTab == "Inventory")
            {
                int col0 = dtgMain.Columns["Name"].Index;
                int col1 = dtgMain.Columns["Brand"].Index;
                itemName = dtgMain.SelectedCells[col0].Value.ToString();
                itemBrand = dtgMain.SelectedCells[col1].Value.ToString();
            }
        }

        // Set number of selected rows
        private void Set_NumberofSelectedRow(object sender, EventArgs e)
        {
            numOfRow = dtgMain.SelectedRows.Count;
        }
    }
}
