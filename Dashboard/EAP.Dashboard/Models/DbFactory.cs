using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace EAP.Dashboard.Models
{
    public class DbFactory
    {
        public static SqlSugarClient GetSqlSugarClient()
        {
            //var db = new SqlSugarClient(new ConnectionConfig()
            //{
            //    DbType = DbType.Sqlite, //配置数据库类型
            //    ConnectionString = @"DataSource=" + "D:\\DB\\HZ_RMS.db",  //数据库连接字符串
            //    IsAutoCloseConnection = true,//设置为true无需使用using或者Close操作，自动关闭连接，不需要手动关闭数据链接
            //    InitKeyType = InitKeyType.Attribute//默认SystemTable, 字段信息读取, 如：该属性是不是主键，是不是标识列等等信息
            //}); //默认SystemTable
            //var test = ConfigurationManager.ConnectionStrings["Oracle_DB"].ToString();
            var site = ConfigurationManager.AppSettings["site"].ToString();
            var connName = site + "_Oracle_DB";
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[connName].ToString(), //必填
                DbType = SqlSugar.DbType.Oracle, //必填
                IsAutoCloseConnection = true, //默认false
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false
                }
            }); ; //默认SystemTable
            return db;
        }

        public static SqlSugarClient GetSqlSugarClient_OTMS()
        {
            var site = ConfigurationManager.AppSettings["site"].ToString();
            var connName = site + "_OTMS_DB";
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[connName].ToString(), //必填
                DbType = SqlSugar.DbType.Oracle, //必填
                IsAutoCloseConnection = true, //默认false
                InitKeyType = InitKeyType.Attribute,
                MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false
                }
            }); ; //默认SystemTable
            return db;
        }

        public static SqlSugarClient GetWMSClient()
        {
            var site = ConfigurationManager.AppSettings["Site"].ToString();
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConfigurationManager.ConnectionStrings[$"{site}_WMS_DB"].ToString(), //必填
                DbType = SqlSugar.DbType.Oracle, //必填
                IsAutoCloseConnection = true, //默认false
                InitKeyType = InitKeyType.Attribute,

            }); //默认SystemTable
            return db;
        }
    }
}