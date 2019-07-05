using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipeline
{
    public delegate Command[] ProcessNotification(IMessage msg, IMessageContextModel ctx);

    
    public interface INotificationProcessor
    {
        Command[] Process(IMessage msg, IMessageContextModel ctx);
    }
}