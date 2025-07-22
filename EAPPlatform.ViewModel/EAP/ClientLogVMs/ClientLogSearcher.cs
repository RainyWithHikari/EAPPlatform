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
    public partial class ClientLogSearcher : BaseSearcher
    {
        [Display(Name = "_Model._ClientLog._DateTime")]
        public DateRange DateTime { get; set; }
        [Display(Name = "_Model._ClientLog._EQID")]
        public String EQID { get; set; }

        protected override void InitVM()
        {
        }

    }
}
