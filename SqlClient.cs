using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace RADARMRM
{
    class SqlClient
    {
        SqlConnection connection;
        SqlCommand command = new SqlCommand();
        Form1 ui;

        public SqlClient(Form1 ui, string sqlConnectionString)
        {
            this.ui = ui;
            connection = new SqlConnection(sqlConnectionString);
            try
            {
                connection.Open();
                command.Connection = connection;
                command.Parameters.Add("@time", SqlDbType.SmallDateTime);
                command.Parameters.Add("@overall", SqlDbType.NChar);
                command.Parameters.Add("@radar", SqlDbType.NChar);
                command.Parameters.Add("@vibration", SqlDbType.NChar);
                command.Parameters.Add("@licensePlate", SqlDbType.NChar);
                command.CommandText = "INSERT INTO result (time, ALARM_overall, ALARM_radar, ALARM_vibration, licensePlate) Values (@time, @overall, @radar, @vibration, @licensePlate)";
            }
            catch(Exception e)
            {
                ui.UpdateStatus(e.Message);
            }
        }

        public void Insert(string overall, string radar, string vibration, string licensePlate)
        {
            try
            {
                command.Parameters[0].Value = DateTime.Now;
                command.Parameters[1].Value = overall;
                if (radar == null)
                    command.Parameters[2].Value = DBNull.Value;
                else
                    command.Parameters[2].Value = radar;
                if (vibration == null)
                    command.Parameters[3].Value = DBNull.Value;
                else
                    command.Parameters[3].Value = vibration;
                command.Parameters[4].Value = licensePlate;
                command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                ui.UpdateStatus(e.Message);
            }
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
