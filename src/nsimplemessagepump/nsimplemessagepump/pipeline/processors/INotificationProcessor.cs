using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipeline.processors
{
    public delegate Command[] ProcessNotification(IMessage msg, IMessageContextModel ctx);

    
    public interface INotificationProcessor
    {
        Command[] Process(IMessage msg, IMessageContextModel ctx);
    }
}