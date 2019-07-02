using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump
{
    public interface IMessagePump
    {
        void Register<TMessage>(Func<IMessage, IMessageContext> load,
                                Func<IMessage, IMessageContext, (CommandStatus,Event[])> processCommand, 
                                Action<Event[]> update);
        
        void Register<TMessage>(Func<IMessage, IMessageContext> load,
                                Func<IMessage, IMessageContext, (CommandStatus,Event[],Notification[])> processCommand, 
                                Action<Event[]> update);
        
        void Register<TMessage>(Func<IMessage, IMessageContext> load,
                                Func<IMessage, IMessageContext, QueryResult> processQuery);
        
        void Register<TMessage>(Func<IMessage, IMessageContext> load,
                                Func<IMessage, IMessageContext, Command[]> processNotification);

        IMessage Handle(IMessage inputMessage);
    }
}