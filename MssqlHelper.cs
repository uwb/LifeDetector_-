using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace RADARMRM
{
    public class MssqlHelper
    {
        private string m_sConnStr;
        protected SqlConnection m_pSqlConn;

        public MssqlHelper(string sConnStr)
        {
            m_sConnStr = sConnStr;
            m_pSqlConn = new SqlConnection(m_sConnStr);
        }

        /// <summary>
        /// 执行SQL语句，返回数据到DataSet中
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sSQL)
        {
            DataSet dataSet = null;
            try
            {
                m_pSqlConn.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter(sSQL, m_pSqlConn);
                dataSet = new DataSet();
                sqlDA.Fill(dataSet);
            }
            catch
            {
                ;
            }
            finally
            {
                m_pSqlConn.Close();
            }
            return dataSet;
        }

        
        /// <summary>
        /// 执行SQL语句，返回 DataReader
        /// </summary>
        /// <param name="sSQL"></param>
        /// <returns></returns>
        public SqlDataReader GetDataReader(string sSQL)
        {
            m_pSqlConn.Open();
            SqlCommand command = new SqlCommand(sSQL, m_pSqlConn);
            SqlDataReader dataReader = command.ExecuteReader();

            return dataReader;
        }

        public bool ExecuteSql(string sSQL)
        {
            bool bRt = false;
            SqlTransaction myTrans = null;
            try
            {
                m_pSqlConn.Open();
                myTrans = m_pSqlConn.BeginTransaction();
                SqlCommand Cmd = new SqlCommand(sSQL, m_pSqlConn, myTrans);
                Cmd.ExecuteNonQuery();
                myTrans.Commit();
                bRt = true;
            }
            catch (SqlException ex)
            {
                if (myTrans != null)
                    myTrans.Rollback();
            }
            finally
            {
                m_pSqlConn.Close();
            }

            return bRt;
        }

        public void Replace()
        {
            this.m_pSqlConn.Close();
        }

        public void InsertRaderMessage(string overall, string radar, string vibration, string licensePlate)
        {
            try
            {
                SqlCommand command = new SqlCommand();
                command.Connection = this.m_pSqlConn;
                command.Parameters.Add("@time", SqlDbType.SmallDateTime);
                command.Parameters.Add("@overall", SqlDbType.NChar);
                command.Parameters.Add("@radar", SqlDbType.NChar);
                command.Parameters.Add("@vibration", SqlDbType.NChar);
                command.Parameters.Add("@licensePlate", SqlDbType.NChar);
                command.CommandText = "INSERT INTO result (time, ALARM_overall, ALARM_radar, ALARM_vibration, licensePlate) Values (@time, @overall, @radar, @vibration, @licensePlate)";

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
            catch (Exception e)
            {
                
            }
        }
    }
}
