namespace Mnk.Library.AutoUpdateAndFeedback
{
    public interface ILocalFolderUpdater
    {
        void PrepareConfigs();
        string Update(bool updateFromSharedFolder, string directory, string version);
    }
}