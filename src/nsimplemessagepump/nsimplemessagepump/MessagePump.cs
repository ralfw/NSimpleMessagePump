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
        private readonly Dictionary<Type, IHandlerPipeline> _pipelines = new Dictionary<Type, IHandlerPipeline>();

        public MessagePump(IEventstore es) { _es = es; }
        
        public void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, (CommandStatus, Event[])> processCommand, Action<Event[]> update)
            => _pipelines[typeof(TMessage)] = new CommandHandlerPipeline(_es, load, processCommand, update);

        public void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, (CommandStatus, Event[], Notification[])> processCommand, Action<Event[]> update)
            => _pipelines[typeof(TMessage)] = new CommandNotificationHandlerPipeline(this, _es, load, processCommand, update);

        public void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, QueryResult> processQuery)
            => _pipelines[typeof(TMessage)] = new QueryHandlerPipeline(load, processQuery);

        public void Register<TMessage>(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, Command[]> processNotification)
            => _pipelines[typeof(TMessage)] = new NotificationHandlerPipeline(this, load, processNotification);


        public IMessage Handle(IMessage inputMessage)
            => _pipelines[inputMessage.GetType()].Handle(inputMessage);
    }
}