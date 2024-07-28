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
    public partial class Inventory : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        public static EventHandler Add_Done;

        private static string itemID = "";

        public Inventory()
        {
            InitializeComponent();
            Com.Connection = CN;

            btnModify.Hide();
            btnDelete.Hide();
            dtpPurchase.Value = DateTime.Today;
            frmMain.Load_ModifyInventory += new EventHandler(Modify_Content);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtBrand.Text) || string.IsNullOrEmpty(txtQuantity.Text))
            {
                MessageBox.Show("Please input item details!");
                return;
            }


            if (!IsDigit(txtQuantity.Text) || string.IsNullOrEmpty(txtQuantity.Text))
            {
                MessageBox.Show("Invalid input in Quantity field!");
                txtQuantity.Focus();
                return;
            }

            CN.Open();

            string name = txtName.Text;
            string brand = txtBrand.Text;
            string quantitly = txtQuantity.Text;
            string purchase = dtpPurchase.Value.ToString("yyyy/MM/dd");

            Com.CommandText = string.Format("INSERT INTO supply(Name, Brand, Quantity, Purchase) VALUES('{0}', '{1}', '{2}', '{3}')", name, brand, quantitly, purchase);
            Com.ExecuteNonQuery();
            CN.Close();

            if (MessageBox.Show("Item successfully added!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
            {
                Add_Done?.Invoke(sender, e);
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

        // Set up form for modifying
        private void Modify_Content(object sender, EventArgs e)
        {
            btnAdd.Hide();
            btnModify.Show();
            btnDelete.Show();
            btnDelete.Show();

            string name = DatagridMain.itemName;
            string brand = DatagridMain.itemBrand;
            CN.Open();
            Com.CommandText = string.Format("SELECT ID FROM supply WHERE `Name` = '{0}' AND Brand = '{1}'", name, brand);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                itemID = reader["ID"].ToString();
            }
            reader.Close();
            CN.Close();

            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM supply WHERE ID = '{0}'", itemID);
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                txtName.Text = reader["Name"].ToString();
                txtBrand.Text = reader["Brand"].ToString();
                txtQuantity.Text = reader["Quantity"].ToString();
                dtpPurchase.Value = (DateTime)reader["Purchase"];
            }
            reader.Close();
            CN.Close();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text) || string.IsNullOrEmpty(txtBrand.Text) || string.IsNullOrEmpty(txtQuantity.Text))
            {
                MessageBox.Show("Please input item details!");
                return;
            }


            if (!IsDigit(txtQuantity.Text) || string.IsNullOrEmpty(txtQuantity.Text))
            {
                MessageBox.Show("Invalid input in Quantity field!");
                txtQuantity.Focus();
                return;
            }

            CN.Open();

            string name = txtName.Text;
            string brand = txtBrand.Text;
            string quantitly = txtQuantity.Text;
            string purchase = dtpPurchase.Value.ToString("yyyy/MM/dd");

            Com.CommandText = string.Format("UPDATE supply SET Name = '{0}', Brand = '{1}', Quantity = '{2}', Purchase = '{3}' WHERE ID = '{4}'", name, brand, quantitly, purchase, itemID);
            Com.ExecuteNonQuery();
            CN.Close();

            if (MessageBox.Show("Item successfully modified!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
            {
                Add_Done?.Invoke(sender, e);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this item?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                CN.Open();
                Com.CommandText = string.Format("DELETE FROM supply WHERE ID = '{0}'", itemID);
                Com.ExecuteNonQuery();
                CN.Close();

                if (MessageBox.Show("Item successfully deleted!", "Success", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Add_Done?.Invoke(sender, e);
                }
            }
        }
    }
}
