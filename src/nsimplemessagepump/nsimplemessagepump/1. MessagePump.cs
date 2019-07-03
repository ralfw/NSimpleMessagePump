using System;
using System.Collections.Generic;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;

namespace nsimplemessagepump
{
    public partial class MessagePump : IMessagePump
    {
        private readonly IEventstore _es;
        private readonly Dictionary<Type, IPipeline> _pipelines = new Dictionary<Type, IPipeline>();
        private readonly EventBroadcast _broadcast;

        public MessagePump(IEventstore es) {
            _es = es;
            _broadcast = new EventBroadcast();
        }


        public void Register<TMessage>(LoadContext load, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string)> processCommand, UpdateContext update) {
            _broadcast.Subscribe(update);
            _pipelines[typeof(TMessage)] = new CommandPipeline(_es, load, processCommand, _broadcast.Update);
        }
        public void Register<TMessage>(LoadContext load, ProcessCommand processCommand, UpdateContext update) { 
            _broadcast.Subscribe(update);
            _pipelines[typeof(TMessage)] = new CommandPipeline(_es, load, processCommand, _broadcast.Update);
        }
        
        public void Register<TMessage>(LoadContext load, ProcessQuery processQuery)
            => _pipelines[typeof(TMessage)] = new QueryPipeline(load, processQuery);

        public void Register<TMessage>(LoadContext load, ProcessNotification processNotification)
            => _pipelines[typeof(TMessage)] = new NotificationPipeline(this, load, processNotification);


        public (IMessage, Notification[]) Handle(IMessage inputMessage)
            => _pipelines[inputMessage.GetType()].Handle(inputMessage);
    }


    class EventBroadcast
    {
        private readonly HashSet<UpdateContext> _subscribers = new HashSet<UpdateContext>();

        public void Subscribe(UpdateContext update) => _subscribers.Add(update);

        public void Update(Event[] events, string version, long finalEventNumber) {
            foreach (var sub in _subscribers) sub(events, version, finalEventNumber);
        }
    }
}