using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public partial class Print : Form
    {
        public Print()
        {
            InitializeComponent();

            pnlPrintArea.Controls.Clear();
            PatientPreview patient = new PatientPreview();
            pnlPrintArea.Controls.Add(patient);
            patient.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void print(Panel pnl)
        {
            PrinterSettings ps = new PrinterSettings();
            pnlPrintArea = pnl;
            getPrintArea(pnl);
            printPreviewDialog1.Document = printDocument1;
            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            printPreviewDialog1.ShowDialog();
        }

        private Bitmap mryImg;

        private void getPrintArea(Panel pnl)
        {
            mryImg = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(mryImg, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Rectangle pageArea = e.PageBounds;
            e.Graphics.DrawImage(mryImg, pageArea.Width / 2 - (pnlPrintArea.Width / 2), pnlPrintArea.Location.Y);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            print(pnlPrintArea);
        }
    }
}
