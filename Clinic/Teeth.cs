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
    public partial class Teeth : Form
    {
        // Event Handlers
        public static EventHandler Add_Clicked;

        public static List<string> teeth = new List<string>();
        public static string picName = "Teeth";

        public Teeth()
        {
            InitializeComponent();
            teeth.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chkLoc1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoc1.Checked)
            {
                chkLoc1.BackgroundImage = Properties.Resources.teethSelected_1_;
            }
            else if (!chkLoc1.Checked)
            {
                chkLoc1.BackgroundImage = Properties.Resources.teeth_1_;
            }
        }

        private void chkLoc2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoc2.Checked)
            {
                chkLoc2.BackgroundImage = Properties.Resources.teethSelected_2_;
            }
            else if (!chkLoc2.Checked)
            {
                chkLoc2.BackgroundImage = Properties.Resources.teeth_2_;
            }
        }

        private void chkLoc3_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoc3.Checked)
            {
                chkLoc3.BackgroundImage = Properties.Resources.teethSelected_3_;
            }
            else if (!chkLoc3.Checked)
            {
                chkLoc3.BackgroundImage = Properties.Resources.teeth_3_;
            }
        }

        private void chkLoc4_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoc4.Checked)
            {
                chkLoc4.BackgroundImage = Properties.Resources.teethSelected_4_;
            }
            else if (!chkLoc4.Checked)
            {
                chkLoc4.BackgroundImage = Properties.Resources.teeth_4_;
            }
        }

        private void chkLoc5_CheckedChanged(object sender, EventArgs e)
        {
            if (chkLoc5.Checked)
            {
                chkLoc5.BackgroundImage = Properties.Resources.teethSelected_5_;
            }
            else if (!chkLoc5.Checked)
            {
                chkLoc5.BackgroundImage = Properties.Resources.teeth_5_;
            }
        }

        private void btnDone_Click(object sender, EventArgs e)
        {
            foreach (string item in activeBoxList(this))
            {
                if (item == "chkLoc1")
                {
                    teeth.Add("1");
                }
                else if (item == "chkLoc2")
                {
                    teeth.Add("2");
                }
                else if (item == "chkLoc3")
                {
                    teeth.Add("3");
                }
                else if (item == "chkLoc4")
                {
                    teeth.Add("4");
                }
                else if (item == "chkLoc5")
                {
                    teeth.Add("5");
                }
            }

            teeth.Sort();
            changePic();

            Add_Clicked?.Invoke(sender, e);
            Close();
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
                    list[list.GetUpperBound(0)] = ((CheckBox)c).Name;
                }
            }

            return list;
        }

        // Updates the picture according to the selected part
        private void changePic()
        {
            string part = "";
            int counter = 0;
            foreach (string item in teeth)
            {
                part += item;
                counter++;
            }

            picName = "Teeth";
            if (!string.IsNullOrEmpty(part))
            {
                string fullToothName = string.Format($"Tooth{part}");
                picName = fullToothName;
            }
        }
    }
}
