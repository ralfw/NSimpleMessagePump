using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump
{
    public interface IMessagePump
    {
        void Register<TMessage>(Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, string, (CommandStatus,Event[],string)> processCommand, Action<Event[],string,long> update);
        void Register<TMessage>(Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, string, (CommandStatus,Event[],string,Notification[])> processCommand, Action<Event[],string,long> update);
        void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, QueryResult> processQuery);
        void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, Command[]> processNotification);
        
        IMessage Handle(IMessage inputMessage);
    }
}