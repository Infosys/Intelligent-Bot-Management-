using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Infosys.Lif.LegacyWorkbench
{
    static class Program
    {
        static string userdomain;
        static string username;
        static string usermachinename;
        static DateTime loggedIndateTime;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new LegacyParser());
            userdomain = System.Environment.UserDomainName.ToString();
            username = System.Environment.UserName;
            usermachinename = System.Environment.MachineName;
            loggedIndateTime = DateTime.Now;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            String adPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            X509Certificate x509 = X509Certificate.CreateFromCertFile(adPath + @"\LifCertificate5.cer");

            string x509PK = x509.GetPublicKeyString();

            X509Certificate x5092 = X509Certificate.CreateFromCertFile(@"D:\TFS_prj\LIF_Labs\Instance5\LifCertificate5.cer");
            string x5092PK = x5092.GetPublicKeyString();


            if (InsertUserDetailInLIF(userdomain, username, usermachinename, loggedIndateTime) && x509PK == x5092PK)
            //if (InsertUserDetailInLIF(userdomain, username, usermachinename, loggedIndateTime))  
            {
                Application.Run(new LegacyParser());
            }
            else
            {
                MessageBox.Show("You are not authorised to access this software. \nPlease contact to Admin", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                Application.Exit();
            }
        }
        static bool InsertUserDetailInLIF(string UserDomainName, string UserName, string MachineName, DateTime Createddatetime)
        {
            SqlConnection sqlConnection1 = null;
            try
            {
                sqlConnection1 = new SqlConnection("server=blrkec0350s;uid=sa;pwd=Infy123+;database=LIFDB");

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = "INSERT INTO [LIFUsageTrack]([UserDomainName],[UserName] ,[MachineName], [loggedIndateTime]) VALUES ('" + UserDomainName + "','" + UserName + "','" + MachineName + "','" + Createddatetime + "');";
                cmd.Connection = sqlConnection1;

                sqlConnection1.Open();
                cmd.ExecuteNonQuery();
                sqlConnection1.Close();

            }
            catch (SqlException e)
            {
                if (sqlConnection1 != null || sqlConnection1.State == ConnectionState.Open)
                {
                    sqlConnection1.Close();
                    return false;
                }
            }
            return true;
        }

    }
}