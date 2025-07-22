using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.WMS
{
    [SugarTable("WMS_TASK_TYPE")]
    public partial class WMS_TASK_TYPE
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string TASK_TYPE_ID { get; set; } = Guid.NewGuid().ToString("N"); //任务编码

        public string TASK_CODE { get; set; } //对应WMS的任务CODE，可维护

        public string TASK_DESCRIBTION { get; set; } //任务描述

        public bool IS_INVENTORY_OUT { get; set; } //出库任务标志位



    }
}
