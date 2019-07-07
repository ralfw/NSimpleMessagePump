using nsimplemessagepump.contract.messagecontext;

namespace nsimplemessagepump.contract.messageprocessing
{
    public delegate Command[] ProcessNotification(IMessage msg, IMessageContextModel ctx);

    
    public interface INotificationProcessor
    {
        Command[] Process(IMessage msg, IMessageContextModel ctx);
    }
}