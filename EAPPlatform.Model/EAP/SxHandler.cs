using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace EAPPlatform.Model.EAP
{
    [Table("SXHANDLER")]
    public class SxHandler
    {
        [Column("EQID")]
        public string EQID { get; set; }

        [Column("NAME")]
        public string NAME { get; set; }
    }
}
