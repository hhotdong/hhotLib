namespace hhotLib.Save
{
    public interface ISavable
    {
        void Register();
        void Unregister();
        void OnSave();
        void OnLoad();
        void OnReset();
    }
}