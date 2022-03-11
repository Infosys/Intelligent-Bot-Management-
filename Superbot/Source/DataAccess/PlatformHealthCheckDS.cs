/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class PlatformHealthCheckDS
    {
        public void GetViewData()
        {
            try
            {
                string connString = ConfigurationManager.AppSettings["DBConnectionString"];
                using (SqlConnection connection = new SqlConnection(connString))
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT * from your_view WHERE your_where_clause";

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // process result
                                reader.GetInt32(0); // get first column from view, assume it's a 32-bit int
                                reader.GetString(1); // get second column from view, assume it's a string
                                                     // etc.
                            }
                        }
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
