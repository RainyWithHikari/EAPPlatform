
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentTypeVMs
{
    public partial class EquipmentTypeTemplateVM : BaseTemplateVM
    {
        
        [Display(Name = "_Model._EquipmentType._Name")]
        public ExcelPropety Name_Excel = ExcelPropety.CreateProperty<EquipmentType>(x => x.Name);
        [Display(Name = "_Model._EquipmentType._CreateTime")]
        public ExcelPropety CreateTime_Excel = ExcelPropety.CreateProperty<EquipmentType>(x => x.CreateTime, true);
        [Display(Name = "_Model._EquipmentType._UpdateTime")]
        public ExcelPropety UpdateTime_Excel = ExcelPropety.CreateProperty<EquipmentType>(x => x.UpdateTime, true);
        [Display(Name = "_Model._EquipmentType._CreateBy")]
        public ExcelPropety CreateBy_Excel = ExcelPropety.CreateProperty<EquipmentType>(x => x.CreateBy);
        [Display(Name = "_Model._EquipmentType._UpdateBy")]
        public ExcelPropety UpdateBy_Excel = ExcelPropety.CreateProperty<EquipmentType>(x => x.UpdateBy);
        [Display(Name = "_Model._EquipmentType._IsValid")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<EquipmentType>(x => x.IsValid);

	    protected override void InitVM()
        {
            
        }

    }

    public class EquipmentTypeImportVM : BaseImportVM<EquipmentTypeTemplateVM, EquipmentType>
    {
        //import

    }

}