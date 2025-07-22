
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;
using EAPPlatform.Model;

namespace EAPPlatform.ViewModel.EAP.EquipmentConfigurationVMs
{
    public partial class EquipmentConfigurationTemplateVM : BaseTemplateVM
    {
        
        [Display(Name = "_Model._EquipmentConfiguration._EQID")]
        public ExcelPropety EQID_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.EQIDId);
        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationItem")]
        public ExcelPropety ConfigurationItem_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.ConfigurationItem);
        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationValue")]
        public ExcelPropety ConfigurationValue_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.ConfigurationValue);
        [Display(Name = "_Model._EquipmentConfiguration._ConfigurationName")]
        public ExcelPropety ConfigurationName_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.ConfigurationName);
        [Display(Name = "_Model._EquipmentConfiguration._DisplayOrder")]
        public ExcelPropety DisplayOrder_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.DisplayOrder);
        [Display(Name = "_Model._EquipmentConfiguration._CreateTime")]
        public ExcelPropety CreateTime_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.CreateTime, true);
        [Display(Name = "_Model._EquipmentConfiguration._UpdateTime")]
        public ExcelPropety UpdateTime_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.UpdateTime, true);
        [Display(Name = "_Model._EquipmentConfiguration._CreateBy")]
        public ExcelPropety CreateBy_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.CreateBy);
        [Display(Name = "_Model._EquipmentConfiguration._UpdateBy")]
        public ExcelPropety UpdateBy_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.UpdateBy);
        [Display(Name = "_Model._EquipmentConfiguration._IsValid")]
        public ExcelPropety IsValid_Excel = ExcelPropety.CreateProperty<EquipmentConfiguration>(x => x.IsValid);

	    protected override void InitVM()
        {
            
            EQID_Excel.DataType = ColumnDataType.ComboBox;
            EQID_Excel.ListItems = DC.Set<Equipment>().GetSelectListItems(Wtm, y => y.EQID.ToString());

        }

    }

    public class EquipmentConfigurationImportVM : BaseImportVM<EquipmentConfigurationTemplateVM, EquipmentConfiguration>
    {
        //import

    }

}