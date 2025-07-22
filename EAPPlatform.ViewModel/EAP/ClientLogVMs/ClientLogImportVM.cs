using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using EAPPlatform.Model.EAP;


namespace EAPPlatform.ViewModel.EAP.ClientLogVMs
{
    public partial class ClientLogTemplateVM : BaseTemplateVM
    {

	    protected override void InitVM()
        {
        }

    }

    public class ClientLogImportVM : BaseImportVM<ClientLogTemplateVM, ClientLog>
    {

    }

}
