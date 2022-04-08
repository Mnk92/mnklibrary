namespace Mnk.Library.CodePlex
{
    public interface ILocalFolderUpdater
    {
        void PrepareConfigs();
        string Update(bool updateFromSharedlFolder, string directory, string version);
    }
}