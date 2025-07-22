using Quartz;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Support.Quartz;

namespace EAPPlatform.Job
{
    [QuartzRepeat(86400, 0, true)]
    public class RegularDayJob:WtmJob
    {
        public override Task Execute(IJobExecutionContext context)
        {
            //Wtm.DoLog("test");
            return Task.CompletedTask;
        }
    }
}
