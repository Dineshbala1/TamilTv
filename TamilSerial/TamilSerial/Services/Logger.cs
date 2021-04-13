using System;
using Microsoft.AppCenter.Crashes;
using TamilTv.Contracts;

namespace TamilTv.Services
{
    public class Logger : ILogger
    {
        public void Verbose(string messageTemplate)
        {
           // Analytics.TrackEvent(messageTemplate);
        }

        public void Information(string messageTemplate)
        {
            
        }

        public void Warning(string messageTemplate)
        {
            
        }

        public void Error(Exception exception, string messageTemplate)
        {
            Crashes.TrackError(exception, null,
                ErrorAttachmentLog.AttachmentWithText(messageTemplate, exception.GetType().Name));
        }
    }
}
