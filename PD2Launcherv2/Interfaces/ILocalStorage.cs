using PD2Launcherv2.Models;

namespace PD2Launcherv2.Interfaces
{
    public interface ILocalStorage
    {
        void Save(AllSettings settings);
        AllSettings Load();
    }
}