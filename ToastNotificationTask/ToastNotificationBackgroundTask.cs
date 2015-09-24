using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace ToastNotificationTask
{
    public sealed class ToastNotificationBackgroundTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            //Inside here developer can retrieve and consume the pre-defined 
            //arguments and user inputs;
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            var arguments = details.Argument;
            var input = details.UserInput["Video"];

            // ...
        }
    }
}

