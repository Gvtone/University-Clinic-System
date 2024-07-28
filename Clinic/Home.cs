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
    public partial class Home : UserControl
    {
        // SQL connection
        MySqlConnection CN = new MySqlConnection("server=localhost;uid=root; password=''; database=clinic");
        MySqlCommand Com = new MySqlCommand();
        MySqlDataReader reader;

        public Home()
        {
            InitializeComponent();
            Com.Connection = CN;

            // Starts the timer for Time and Date
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("t");
            lblDate.Text = DateTime.Now.ToString("D");
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Invoke(new MethodInvoker(delegate ()
            {
                lblTime.Text = DateTime.Now.ToString("t");
                lblDate.Text = DateTime.Now.ToString("D");
            }));
        }

        private void Home_Load(object sender, EventArgs e)
        {
            // Sets label to show last patient's information
            string name = "", birthday = "", position = "", course = "", age = "", sex = "", dateAdded = "";
            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM patient ORDER BY ID DESC LIMIT 1");
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                name = reader["Name"].ToString();

                DateTime set1 = (DateTime)reader["Birthday"];
                string formDate1 = set1.ToString("MMMM dd, yyyy");
                birthday = formDate1;

                position = reader["Position"].ToString();
                course = reader["Course"].ToString();
                age = reader["Age"].ToString();
                sex = reader["Sex"].ToString();

                DateTime set2 = (DateTime)reader["Date_Added"];
                string formDate2 = set2.ToString("MMMM dd, yyyy");
                dateAdded = formDate2;
            }
            reader.Close();
            CN.Close();

            lblRecent.Text = string.Format("Name: {0}\r\n" +
                "Birthday: {1}\r\n" +
                "Position: {2}\r\n" +
                "Course: {3}\r\n" +
                "Age: {4}\r\n" +
                "Sex: {5}\r\n" +
                "Date Added: {6}",
                name, birthday, position, course, age, sex, dateAdded);

            lblNotif.Text = "";
            string notification = "";
            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM medicine");
            reader = Com.ExecuteReader();
            while (reader.Read())
            {
                int stock = Convert.ToInt32(reader["Stock"]);

                if (stock < 20)
                {
                    notification += string.Format("* There are only {0} stocks of {1} left\r\n", stock, reader["Name"]);
                }

                DateTime dateNow = DateTime.Now;
                DateTime expirationDate = (DateTime)reader["Expiration"];
                TimeSpan diff1 = expirationDate.Subtract(dateNow);
                int compare = DateTime.Compare(dateNow, expirationDate);

                if (diff1.TotalDays < 30 && stock < 2 && compare < 0)
                {
                    notification += string.Format("* {0} stocks of {1} are nearning their expiration\r\n", stock, reader["Name"]);
                }
                else if (diff1.TotalDays < 30 && stock > 1 && compare < 0)
                {
                    notification += string.Format("* {0} stock of {1} is nearning its expiration\r\n", stock, reader["Name"]);
                }
                else if (stock < 2 && compare > 0)
                {
                    notification += string.Format("* {0} stocks of {1} have expired\r\n", stock, reader["Name"]);
                }
                else if (stock > 1 && compare > 0)
                {
                    notification += string.Format("* {0} stock of {1} have expired\r\n", stock, reader["Name"]);
                }
            }
            lblNotif.Text = notification;
            reader.Close();
            CN.Close();

            DateTime dateToday = DateTime.Today;
            string dateTodayFormated = dateToday.ToString("yyyy/MM/dd");
            DateTime SQLDate = DateTime.Today;
            string SQLDateFormated = "";
            string patient_count = "";
            CN.Open();
            Com.CommandText = string.Format("SELECT * FROM counter");
            reader = Com.ExecuteReader();
            if (reader.Read())
            {
                patient_count = reader["Patient_Count"].ToString();
                SQLDate = (DateTime)reader["Date"];
                SQLDateFormated = SQLDate.ToString("yyyy/MM/dd");
            }
            reader.Close();
            CN.Close();

            if (DateTime.Compare(dateToday, SQLDate) > 0)
            {
                CN.Open();
                Com.CommandText = string.Format("UPDATE `clinic`.`counter` SET `Date` = '{0}', `Patient_Count` = '0' WHERE `Date` = '{1}' " +
                    "AND `Patient_Count` = '{2}'; ", dateTodayFormated, SQLDateFormated, patient_count);
                Com.ExecuteNonQuery();

                Com.CommandText = string.Format("SELECT Patient_Count FROM counter");
                reader = Com.ExecuteReader();
                if (reader.Read())
                {
                    lblPatientNum.Text = reader["Patient_Count"].ToString();
                }
                reader.Close();
                CN.Close();
            }
            else
            {
                CN.Open();
                Com.CommandText = string.Format("SELECT Patient_Count FROM counter");
                reader = Com.ExecuteReader();
                if (reader.Read())
                {
                    lblPatientNum.Text = reader["Patient_Count"].ToString();
                }
                reader.Close();
                CN.Close();
            }
        }
    }
}
