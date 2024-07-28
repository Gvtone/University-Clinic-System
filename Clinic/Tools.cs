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
    public partial class Tools : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        public static EventHandler Add_Done;

        private static string toolID = "";

        public Tools()
        {
            InitializeComponent();
            Com.Connection = CN;

            btnModify.Hide();
            btnDelete.Hide();
            dtpCalibration.Value = DateTime.Today;
            frmMain.Load_ModifyTools += new EventHandler(Modify_Content);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please input tool details!");
                return;
            }

            CN.Open();

            string name = txtName.Text;
            string calibration = dtpCalibration.Value.ToString("yyyy/MM/dd");
            string comment = txtComment.Text;

            if (string.IsNullOrEmpty(txtComment.Text))
            {
                Com.CommandText = string.Format("INSERT INTO tool(Name, Calibration) VALUES('{0}', '{1}')", name, calibration);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Tool successfully added!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
            else
            {
                Com.CommandText = string.Format("INSERT INTO tool(Name, Calibration, Comment) VALUES('{0}', '{1}', '{2}')", name, calibration, comment);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Tool successfully added!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
        }

        // Set up form for modifying
        private void Modify_Content(object sender, EventArgs e)
        {
            btnAdd.Hide();
            btnModify.Show();
            btnDelete.Show();
            btnDelete.Show();

            string name = DatagridMain.toolName;
            CN.Open();
            Com.CommandText = string.Format("SELECT ID FROM tool WHERE `Name` = '{0}'", name);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                toolID = reader["ID"].ToString();
            }
            reader.Close();
            CN.Close();

            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM tool WHERE ID = '{0}'", toolID);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                txtName.Text = reader["Name"].ToString();
                dtpCalibration.Value = (DateTime)reader["Calibration"];
                txtComment.Text = reader["Comment"].ToString();
            }
            reader.Close();
            CN.Close();
        }

        // Modify Button
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Please input tool details!");
                return;
            }

            CN.Open();

            string name = txtName.Text;
            string calibration = dtpCalibration.Value.ToString("yyyy/MM/dd");
            string comment = txtComment.Text;

            if (string.IsNullOrEmpty(txtComment.Text))
            {
                Com.CommandText = string.Format("UPDATE tool SET Name = '{0}', Calibration = '{1}', WHERE ID = '{2}'", name, calibration, toolID);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Tool successfully modified!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
            else
            {
                Com.CommandText = string.Format("UPDATE tool SET Name = '{0}', Calibration = '{1}', Comment = '{2}' WHERE ID = '{3}'", name, calibration, comment, toolID);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Tool successfully modified!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
        }

        // Delete Button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this tool?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CN.Open();
                Com.CommandText = string.Format("DELETE FROM tool WHERE ID = '{0}'", toolID);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Tool successfully deleted!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
        }
    }
}
