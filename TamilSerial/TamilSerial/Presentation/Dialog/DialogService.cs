using System;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace TamilSerial.Presentation.Dialog
{
    public class DialogService : IDialogService
    {
        public Task ShowAlertAsync(string message, string title, string buttonLabel)
        {
            return UserDialogs.Instance.AlertAsync(message, title, buttonLabel);
        }

        public async Task LoadingAsync(Func<Task> actionToPerform, string loadingMessage = null)
        {
            using (UserDialogs.Instance.Loading(loadingMessage ?? "Downloading", null, null, true, MaskType.Black))
            {
                await actionToPerform();
            }
        }
    }
}
