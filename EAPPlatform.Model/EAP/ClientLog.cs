using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("CLIENTLOG")]
    [Display(Name = "_Model._ClientLog")]

    public class ClientLog: TopBasePoco
    {
        [Display(Name = "_Model._ClientLog._DateTime")]
        [Column("DATETIME")]
        public DateTime DateTime { get; set; }

        [StringLength(100)]
        [Display(Name = "_Model._ClientLog._EQID")]
        [Column("EQID")]
        public string EQID { get; set; }

        [StringLength(100)]
        [Display(Name = "_Model._ClientLog._LogType")]
        [Column("LOGTYPE")]
        public string LogType { get; set; }

        [StringLength(100)]
        [Display(Name = "_Model._ClientLog._LogLevel")]
        [Column("LOGLEVEL")]
        public string LogLevel { get; set; }

        [StringLength(2000)]
        [Display(Name = "_Model._ClientLog._LogContent")]
        [Column("LOGCONTENT")]
        public string LogContent { get; set; }

    }
}
