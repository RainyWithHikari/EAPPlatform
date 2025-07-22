
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentVMs
{
    public partial class EquipmentTemplateVM : BaseTemplateVM
    {
        
        [Display(Name = "_Model._Equipment._EQID")]
        public ExcelPropety EQID_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.EQID);
        [Display(Name = "_Model._Equipment._EquipmentType")]
        public ExcelPropety EquipmentType_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.EquipmentTypeId);
        [Display(Name = "_Model._Equipment._Name")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.Name);
        [Display(Name = "_Model._Equipment._CreateTime")]
        public ExcelPropety CreateTime_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.CreateTime, true);
        [Display(Name = "_Model._Equipment._UpdateTime")]
        public ExcelPropety UpdateTime_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.UpdateTime, true);
        [Display(Name = "_Model._Equipment._CreateBy")]
        public ExcelPropety CreateBy_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.CreateBy);
        [Display(Name = "_Model._Equipment._UpdateBy")]
        public ExcelPropety UpdateBy_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.UpdateBy);
        [Display(Name = "_Model._Equipment._IsValid")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<Equipment>(x => x.IsValid);

	    protected override void InitVM()
        {
            
            EquipmentType_Excel.DataType = ColumnDataType.ComboBox;
            EquipmentType_Excel.ListItems = DC.Set<EquipmentType>().GetSelectListItems(Wtm, y => y.Name.ToString());

        }

    }

    public class EquipmentImportVM : BaseImportVM<EquipmentTemplateVM, Equipment>
    {
        //import

    }

}