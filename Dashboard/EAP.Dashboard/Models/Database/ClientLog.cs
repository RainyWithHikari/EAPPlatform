using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace EAP.Models.Database
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("CLIENTLOG")]
    public partial class ClientLog
    {
        public ClientLog()
        {
        }
         
        public byte[] ID { get; set; }
        
        [SugarColumn(ColumnName= "DATETIME")]
        public DateTime DateTime { get; set; }

        [SugarColumn(ColumnName = "EQID")]
        public string EQID { get; set; }

        [SugarColumn(ColumnName = "LOGTYPE")]
        public string LogType { get; set; }

        [SugarColumn(ColumnName = "LOGLEVEL")]
        public string LogLevel { get; set; }

        [SugarColumn(ColumnName = "LOGCONTENT")]
        public string LogContent { get; set; }

    }
}
