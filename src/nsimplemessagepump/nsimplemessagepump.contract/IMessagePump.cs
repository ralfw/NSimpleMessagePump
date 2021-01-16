using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.contract
{
    public interface IMessagePump
    {
        void Register<TMessage>(LoadContextModel load, ProcessCommand processCommand, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessQuery processQuery, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessNotification processNotification, UpdateContextModel update);
        
        
        (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage);
    }
}