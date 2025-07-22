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
    public partial class ClientLogBatchVM : BaseBatchVM<ClientLog, ClientLog_BatchEdit>
    {
        public ClientLogBatchVM()
        {
            ListVM = new ClientLogListVM();
            LinkedVM = new ClientLog_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class ClientLog_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
