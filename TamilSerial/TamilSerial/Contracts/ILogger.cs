using System;

namespace TamilTv.Contracts
{
    public interface ILogger
    {
        void Verbose(string messageTemplate);

        void Information(string messageTemplate);

        void Warning(string messageTemplate);

        void Error(Exception exception, string messageTemplate);
    }
}
