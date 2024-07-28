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
    public partial class TopBar : UserControl
    {
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        // Event Handlers
        public static EventHandler Add_Clicked;
        public static EventHandler Overview_Clicked;
        public static EventHandler Print_Clicked;
        public static EventHandler Get_Row;
        public static EventHandler Proceed_Print;
        public static EventHandler Modified_Clicked;
        public static EventHandler Load_Panel;
        public static EventHandler Get_TableRow;
        public static EventHandler Open_Patient;

        public TopBar()
        {
            InitializeComponent();
            Com.Connection = CN;

            btnOverview.Focus();
            btnModify.Enabled = true;
            btnPrint.Enabled = true;
            frmMain.Change_Button += new EventHandler(ChangeButton);
        }

        // Overview button
        private void btnOverview_Click(object sender, EventArgs e)
        {
            Overview_Clicked?.Invoke(sender, e);

            btnModify.Enabled = true;
            btnPrint.Enabled = true;
        }

        // Add button
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Add_Clicked?.Invoke(sender, e);

            btnModify.Enabled = false;
            btnPrint.Enabled = false;
        }

        // Modify button
        private void btnModify_Click(object sender, EventArgs e)
        {
            Modified_Clicked?.Invoke(sender, e);
            int numOfRow = DatagridMain.numOfRow;

            if (frmMain.currentTab == "Patient")
            {
                if (numOfRow <= 0)
                {
                    MessageBox.Show("Please select a patient to delete!");
                    return;
                }
                else
                {
                    Get_Row?.Invoke(sender, e);

                    if (MessageBox.Show("Are you sure you want to delete this patient's record?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string name = DatagridMain.name;
                        string date = DatagridMain.date;

                        CN.Open();
                        Com.CommandText = string.Format("DELETE FROM patient WHERE NAME = '{0}' AND Date_Added = '{1}'", name, date);
                        Com.ExecuteNonQuery();
                        CN.Close();

                        // Decrement counter in SQL
                        int patientCounter = 0;
                        CN.Open();
                        Com.CommandText = string.Format("SELECT Patient_Count FROM counter");
                        reader = Com.ExecuteReader();
                        if (reader.Read())
                        {
                            patientCounter = Convert.ToInt32(reader["Patient_Count"]);
                        }
                        CN.Close();

                        if (patientCounter != 0)
                        {
                            CN.Open();
                            Com.CommandText = string.Format("UPDATE counter SET Patient_Count = Patient_Count - 1");
                            Com.ExecuteNonQuery();
                            CN.Close();
                        }
                        
                        if (MessageBox.Show("Record deleted successfuly!", "Success") == DialogResult.OK)
                        {
                            Open_Patient?.Invoke(sender, e);
                        }
                        return;
                    }
                }
            }
            else if (frmMain.currentTab == "Medicine")
            {
                if (numOfRow <= 0)
                {
                    MessageBox.Show("Please select a medicine to modify!");
                    return;
                }
            }
            else if (frmMain.currentTab == "Tools")
            {
                if (numOfRow <= 0)
                {
                    MessageBox.Show("Please select a tool to modify!");
                    return;
                }
            }
            else if (frmMain.currentTab == "Inventory")
            {
                if (numOfRow <= 0)
                {
                    MessageBox.Show("Please select an item to modify!");
                    return;
                }
            }

            Get_Row?.Invoke(sender, e);
            Load_Panel?.Invoke(sender, e);
        }

        // Print button
        private void btnPrint_Click(object sender, EventArgs e)
        {
            Print_Clicked?.Invoke(sender, e);
            int numOfRow = DatagridMain.numOfRow;

            if (frmMain.currentTab == "Patient")
            {
                if (numOfRow <= 0)
                {
                    MessageBox.Show("Please select a patient!");
                    return;
                }

                Get_Row?.Invoke(sender, e);
                Proceed_Print?.Invoke(sender, e);
            }
        }

        public void ChangeButton(object sender, EventArgs e)
        {
            if (frmMain.currentTab == "Patient")
            {
                btnModify.Text = "Delete";
                btnModify.onHoverState.BorderColor = Color.LightCoral;
                btnModify.onHoverState.FillColor = Color.LightCoral;

                btnModify.OnPressedState.BorderColor = Color.LightCoral;
                btnModify.OnPressedState.FillColor = Color.LightCoral;
            }
            else
            {
                btnModify.Text = "Modify";
                btnModify.onHoverState.BorderColor = Color.FromArgb(105, 181, 255);
                btnModify.onHoverState.FillColor = Color.FromArgb(105, 181, 255);

                btnModify.OnPressedState.BorderColor = Color.FromArgb(101, 136, 252);
                btnModify.OnPressedState.FillColor = Color.FromArgb(101, 136, 252);
            }
        }
    }
}
