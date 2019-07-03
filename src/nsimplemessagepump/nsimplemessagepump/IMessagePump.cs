using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;

namespace nsimplemessagepump
{
    public interface IMessagePump
    {
        void Register<TMessage>(LoadContext load, Func<IMessage, IMessageContext, string, (CommandStatus,Event[],string)> processCommand, UpdateContext update);
        void Register<TMessage>(LoadContext load, ProcessCommand processCommand, UpdateContext update);
        void Register<TMessage>(LoadContext load, ProcessQuery process);
        void Register<TMessage>(LoadContext load, ProcessNotification process);
        
        (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage);
    }
}