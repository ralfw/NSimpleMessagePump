using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipeline;

namespace nsimplemessagepump
{
    public interface IMessagePump
    {
        void Register<TMessage>(LoadContextModel load, Func<IMessage, IMessageContextModel, string, (CommandStatus,Event[],string)> processCommand, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessCommand processCommand, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessQuery process, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessNotification process, UpdateContextModel update);
        
        (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage);
    }
}