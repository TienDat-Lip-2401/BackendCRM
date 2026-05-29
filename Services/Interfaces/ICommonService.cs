namespace RedmineApp.Services.Interfaces
{
    public interface ICommonService
    {
        void SetUserId(int userId);
        int GetUserId();
        void SetPosition(string position);
        string GetPosition();
    }
}
