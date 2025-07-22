using SqlSugar;
using System.Configuration;
namespace EAPPlatform.Util
{
    public class DbFactory
    {
        public static SqlSugarClient GetSqlSugarClient()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "User Id=USI_OTMS;Password=USI_OTMS123;Pooling = True;Max Pool Size = 10;Min Pool Size = 1;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.5.1.196)(PORT=1521)))(CONNECT_DATA=(SID=SMDDB1)))",
                //ConnectionString = "User Id=USI_OTMS;Password=USI_OTMS123;Pooling = True;Max Pool Size = 10;Min Pool Size = 1;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.150.137)(PORT=1521)))(CONNECT_DATA=(SID=SMDDB1)))", //必填
                DbType = DbType.Oracle, //必填
                IsAutoCloseConnection = true, //默认false
                InitKeyType = InitKeyType.Attribute,

            }); //默认SystemTable
            return db;
        }
    }
}
