using System;
using System.Threading.Tasks;

namespace TamilSerial.Presentation.Dialog
{
    public interface IDialogService
    {
        Task ShowAlertAsync(string message, string title, string buttonLabel);

        Task LoadingAsync(Func<Task> actionToPerform, string loadingMessage = null);
    }
}
