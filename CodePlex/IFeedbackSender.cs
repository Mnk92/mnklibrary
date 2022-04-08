namespace Mnk.Library.CodePlex
{
    public interface IFeedbackSender
    {
        bool Send(string title, string body);
    }
}
