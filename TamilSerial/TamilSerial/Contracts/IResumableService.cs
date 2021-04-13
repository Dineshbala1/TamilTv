namespace TamilTv.Contracts
{
    public interface IResumableService
    {
        void OnSleep();
        void OnResume();
        void OnStart();
    }
}