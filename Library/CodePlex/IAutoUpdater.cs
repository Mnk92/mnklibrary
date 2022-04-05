namespace Mnk.Library.CodePlex
{
    public interface IAutoUpdater
    {
        bool? TryUpdate(bool manual = false);
    }
}
