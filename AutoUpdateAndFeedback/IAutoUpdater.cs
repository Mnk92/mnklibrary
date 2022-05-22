namespace Mnk.Library.AutoUpdateAndFeedback
{
    public interface IAutoUpdater
    {
        bool? TryUpdate(bool manual = false);
    }
}
