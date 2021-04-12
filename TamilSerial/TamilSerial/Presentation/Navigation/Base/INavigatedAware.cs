using System.Threading.Tasks;

namespace TamilSerial.Presentation.Navigation.Base
{
    public interface INavigatedAware
    {
        /// <summary>
        /// Called when the implementer has been navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedFrom(INavigationParameters parameters);

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedTo(INavigationParameters parameters);
    }

    public interface IInitialize
    {
        void Initialize(INavigationParameters navigationParameters);
    }

    public interface IInitializeAsync
    {
        Task InitializeAsync(INavigationParameters parameters);
    }
}