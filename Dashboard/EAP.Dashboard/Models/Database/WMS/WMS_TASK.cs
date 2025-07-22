using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAP.Models.Database.WMS
{
    [SugarTable("WMS_TASK")]
    public class WMS_TASK
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string TASK_ID { get; set; } //AGV搬运任务唯一码

        public string MVT { get; set; } //SAP数据,代表不同的出入库类型

        public string TASK_TYPE_CODE { get; set; } //出库任务/入库任务

        public string TASK_STATUS { get; set; } //完成/异常完成

        public string WORK_ORDER { get; set; } //工单号，出库任务含此信息，其他任务无此信息

        public string CTU_ID { get; set; } // CTU 编号
        public string CTU_START_POSITION { get; set; } // CTU搬运任务起点
        public string CTU_END_POSITION { get; set; }// CTU搬运任务终点

        public string TRAY_CODE { get; set; } //静电箱外箱码，A代表带隔板，B代表不带隔板

        public DateTime TASK_CREATE_TIME { get; set; } //搬运任务生成时间，代表人工在WMS系统触发任务时间

        public DateTime CTU_START_TIME { get; set; } //CTU开始执行任务时间

        public DateTime CTU_END_TIME { get; set; } //CTU完成该任务的时间

        public string BATCH_ID { get; set; } //批次号，SAP生成

        public string PART_NUMBER { get; set; } // 产品料号，可用于区分产品类型，7寸，非7寸，真空包等

        public string HU_ID { get; set; } //WMS唯一码，一个码代表一盘料

        public string PLANT_CODE { get; set; } //厂别，料盘绑定信息
        public string STORAGE_CODE { get; set; } //库别，料盘绑定信息

        public string LOT_CODE { get; set; } //Lot号，料盘绑定信息

        public string DC_CODE { get; set; } //DC号，料盘绑定信息

        public string REEL_ID { get; set; } //REEL ID 料盘号
        public int REEL_ORDER { get; set; } //带储物格料箱的储物格顺序，1-21

        public int PACK_NUMBER { get; set; } //单盘数量（颗数）

        public bool IS_MANTISSA { get; set; } //尾数盘标志位

    }
}
