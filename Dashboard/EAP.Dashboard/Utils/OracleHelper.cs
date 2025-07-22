using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace EAP.Dashboard.Utils
{
    public class OracleHelper
    {
        #region FillData
        public static DataTable GetDTFromCommand(string strSql)
        {
            DataTable dt = new DataTable();
            Oracle.ManagedDataAccess.Client.OracleCommand command = null;
            Oracle.ManagedDataAccess.Client.OracleDataAdapter dataAdapter = null;
           
            string connectionString = GetConnectionString("Connections");
            
            using (Oracle.ManagedDataAccess.Client.OracleConnection connection = new OracleConnection(connectionString))
            {
                try
                {
                    command = connection.CreateCommand();
                    command.CommandText = strSql;
                    command.CommandType = CommandType.Text;

                    connection.Open();

                    dataAdapter = new OracleDataAdapter();
                    dataAdapter.SelectCommand = command;
                    dataAdapter.Fill(dt);

                    connection.Close();

                    dt.TableName = "Table";
                    return dt;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        #endregion
    public static string GetConnectionString(string configItem)
        {
            //var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            //IConfiguration configuration = configBuilder.Build();
            //var connectionString = configuration[configItem+":0:Value"];
            var site = System.Configuration.ConfigurationManager.AppSettings["site"].ToString();
            var connName = site + "_Oracle_DB";
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connName].ToString(); 
            return connectionString;
        }
    
    }
}
