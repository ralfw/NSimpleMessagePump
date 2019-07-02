using System;
using System.Collections.Generic;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;

namespace nsimplemessagepump
{
    public class MessagePump : IMessagePump
    {
        private readonly IEventstore _es;
        private readonly Dictionary<Type, IPipeline> _pipelines = new Dictionary<Type, IPipeline>();

        public MessagePump(IEventstore es) { _es = es; }
        
        
        public void Register<TMessage>(Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, string, (CommandStatus,Event[],string)> processCommand, Action<Event[],string,long> update)
            => _pipelines[typeof(TMessage)] = new CommandPipeline(this, _es, load, processCommand, update);

        public void Register<TMessage>(Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, string, (CommandStatus,Event[],string,Notification[])> processCommand, Action<Event[],string,long> update)
            => _pipelines[typeof(TMessage)] = new CommandPipeline(this, _es, load, processCommand, update);

        public void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, QueryResult> processQuery)
            => _pipelines[typeof(TMessage)] = new QueryPipeline(load, processQuery);

        public void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, Command[]> processNotification)
            => _pipelines[typeof(TMessage)] = new NotificationPipeline(this, load, processNotification);


        public IMessage Handle(IMessage inputMessage)
            => _pipelines[inputMessage.GetType()].Handle(inputMessage);
    }
}