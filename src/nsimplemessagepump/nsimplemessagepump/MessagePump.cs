using System;
using System.Collections.Generic;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;
using nsimplemessagepump.pipeline;

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

        
        public void Register<TMessage>(LoadContextModel load, ProcessCommand processCommand, UpdateContextModel update) { 
            _broadcast.Subscribe(update);
            _pipelines[typeof(TMessage)] = new CommandPipeline(_es, load, processCommand, _broadcast.Update);
        }

        public void Register<TMessage>(LoadContextModel load, ProcessQuery processQuery, UpdateContextModel update) {
            _broadcast.Subscribe(update);
            _pipelines[typeof(TMessage)] = new QueryPipeline(load, processQuery);
        }

        public void Register<TMessage>(LoadContextModel load, ProcessNotification processNotification, UpdateContextModel update) {
            _broadcast.Subscribe(update);
            _pipelines[typeof(TMessage)] = new NotificationPipeline(this, load, processNotification);
        }


        public MessagePumpFluentRegistration<TIncomingMessage> On<TIncomingMessage>() where TIncomingMessage : IIncoming
            => new MessagePumpFluentRegistration<TIncomingMessage>(this);

        
        public (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage)
            => _pipelines[inputMessage.GetType()].Handle(inputMessage);
    }


    public class EventBroadcast
    {
        private readonly HashSet<UpdateContextModel> _subscribers = new HashSet<UpdateContextModel>();

        public void Subscribe(UpdateContextModel update) => _subscribers.Add(update);

        public void Update(IEvent[] events, EventId lastEventId) {
            foreach (var sub in _subscribers) sub(events, lastEventId);
        }
    }
}