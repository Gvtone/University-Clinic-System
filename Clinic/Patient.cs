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
    public partial class Patient : UserControl
    {
        public Patient()
        {
            InitializeComponent();

            pnlPatient.Controls.Clear();
            SubPatient patient = new SubPatient();
            pnlPatient.Controls.Add(patient);
            patient.Show();
        }
    }
}
