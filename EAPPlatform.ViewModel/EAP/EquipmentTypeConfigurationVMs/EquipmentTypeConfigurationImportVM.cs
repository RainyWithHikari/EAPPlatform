
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentTypeConfigurationVMs
{
    public partial class EquipmentTypeConfigurationTemplateVM : BaseTemplateVM
    {
        
        [Display(Name = "_Model._EquipmentTypeConfiguration._TypeName")]
        public ExcelPropety TypeName_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.TypeNameId);
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationItem")]
        public ExcelPropety ConfigurationItem_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.ConfigurationItem);
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationValue")]
        public ExcelPropety ConfigurationValue_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.ConfigurationValue);
        [Display(Name = "_Model._EquipmentTypeConfiguration._ConfigurationName")]
        public ExcelPropety ConfigurationName_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.ConfigurationName);
        [Display(Name = "_Model._EquipmentTypeConfiguration._DisplayOrder")]
        public ExcelPropety DisplayOrder_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.DisplayOrder);
        [Display(Name = "_Model._EquipmentTypeConfiguration._CreateTime")]
        public ExcelPropety CreateTime_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.CreateTime, true);
        [Display(Name = "_Model._EquipmentTypeConfiguration._UpdateTime")]
        public ExcelPropety UpdateTime_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.UpdateTime, true);
        [Display(Name = "_Model._EquipmentTypeConfiguration._CreateBy")]
        public ExcelPropety CreateBy_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.CreateBy);
        [Display(Name = "_Model._EquipmentTypeConfiguration._UpdateBy")]
        public ExcelPropety UpdateBy_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.UpdateBy);
        [Display(Name = "_Model._EquipmentTypeConfiguration._IsValid")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<EquipmentTypeConfiguration>(x => x.IsValid);

	    protected override void InitVM()
        {
            
            TypeName_Excel.DataType = ColumnDataType.ComboBox;
            TypeName_Excel.ListItems = DC.Set<EquipmentType>().GetSelectListItems(Wtm, y => y.Name.ToString());

        }

    }

    public class EquipmentTypeConfigurationImportVM : BaseImportVM<EquipmentTypeConfigurationTemplateVM, EquipmentTypeConfiguration>
    {
        //import

    }

}