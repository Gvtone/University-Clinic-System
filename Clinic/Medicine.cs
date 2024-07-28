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
    public partial class Medicine : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        public static EventHandler Add_Done;

        private static string medicineID = "";

        public Medicine()
        {
            InitializeComponent();
            Com.Connection = CN;

            btnModify.Hide();
            btnDelete.Hide();
            dtpExpiration.Value = DateTime.Today;
            frmMain.Load_ModifyMedicine += new EventHandler(Modify_Content);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtBrand.Text) || string.IsNullOrEmpty(txtStock.Text))
            {
                MessageBox.Show("Please input medicine details!");
                return;
            }

            if (!IsDigit(txtStock.Text) || string.IsNullOrEmpty(txtStock.Text))
            {
                MessageBox.Show("Invalid input in stock field!");
                txtStock.Focus();
                return;
            }

            CN.Open();

            string name = txtName.Text;
            string brand = txtBrand.Text;
            string stock = txtStock.Text;
            string expiration = dtpExpiration.Value.ToString("yyyy/MM/dd");

            Com.CommandText = string.Format("INSERT INTO medicine(Name, Brand, Stock, Expiration) VALUES('{0}', '{1}', '{2}', '{3}')", name, brand, stock, expiration);
            Com.ExecuteNonQuery();
            CN.Close();

            if (MessageBox.Show("Medicine successfully added!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
            {
                Add_Done?.Invoke(sender, e);
            }
        }

        // Set up form for modifying
        private void Modify_Content(object sender, EventArgs e)
        {
            btnAdd.Hide();
            btnModify.Show();
            btnDelete.Show();
            btnDelete.Show();

            string name = DatagridMain.medicineName;
            string brand = DatagridMain.medicineBrand;
            CN.Open();
            Com.CommandText = string.Format("SELECT ID FROM medicine WHERE `Name` = '{0}' AND Brand = '{1}'", name, brand);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                medicineID = reader["ID"].ToString();
            }
            reader.Close();
            CN.Close();

            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM medicine WHERE ID = '{0}'", medicineID);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                txtName.Text = reader["Name"].ToString();
                txtBrand.Text = reader["Brand"].ToString();
                txtStock.Text = reader["Stock"].ToString();
                dtpExpiration.Value = (DateTime)reader["Expiration"];
            }
            reader.Close();
            CN.Close();
        }

        // Modify Button
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtBrand.Text) || string.IsNullOrEmpty(txtStock.Text))
            {
                MessageBox.Show("Please input medicine details!");
                return;
            }

            if (!IsDigit(txtStock.Text) || string.IsNullOrEmpty(txtStock.Text))
            {
                MessageBox.Show("Invalid input in stock field!");
                txtStock.Focus();
                return;
            }

            CN.Open();

            string name = txtName.Text;
            string brand = txtBrand.Text;
            string stock = txtStock.Text;
            string expiration = dtpExpiration.Value.ToString("yyyy/MM/dd");

            Com.CommandText = string.Format("UPDATE medicine SET Name = '{0}', Brand = '{1}', Stock = '{2}', Expiration = '{3}' WHERE ID = '{4}'", name, brand, stock, expiration, medicineID);
            Com.ExecuteNonQuery();
            CN.Close();

            if (MessageBox.Show("Medicine successfully modified!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
            {
                Add_Done?.Invoke(sender, e);
            }
        }

        // Delete Button
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this medicine?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CN.Open();
                Com.CommandText = string.Format("DELETE FROM medicine WHERE ID = '{0}'", medicineID);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Medicine successfully deleted!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
        }

        private static bool IsDigit(string input)
        {
            foreach (char c in input)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
