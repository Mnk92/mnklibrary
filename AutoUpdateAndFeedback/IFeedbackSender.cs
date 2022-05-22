namespace Mnk.Library.AutoUpdateAndFeedback
{
    public interface IFeedbackSender
    {
        bool Send(string title, string body);
    }
}
