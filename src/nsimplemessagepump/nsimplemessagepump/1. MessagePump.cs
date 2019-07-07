using System;
using System.Collections.Generic;
using nsimpleeventstore;
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

        
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand)
            => Register<TMessage>(ctxModelManager.Load, processCommand, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessCommand processCommand)
            => Register<TMessage>(ctxModelManager.Load, processCommand, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ICommandProcessor processor)
            => Register<TMessage>(ctxModelManager.Load, processor.Process, ctxModelManager.Update);        
        
        
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessQuery processQuery)
            => Register<TMessage>(ctxModelManager.Load, processQuery, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, IQueryProcessor processor)
            => Register<TMessage>(ctxModelManager.Load, processor.Process, ctxModelManager.Update);
        
        
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessNotification processNotification)
            => Register<TMessage>(ctxModelManager.Load, processNotification, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, INotificationProcessor processor)
            => Register<TMessage>(ctxModelManager.Load, processor.Process, ctxModelManager.Update);
        

        public void Register<TMessage>(LoadContextModel load, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand, UpdateContextModel update) {
            Register<TMessage>(load, 
                               (msg, ctx, ver) => {
                                    var (status,events,version) = processCommand(msg, ctx, ver);
                                    return (status, events, version, new Notification[0]);
                               }, 
                               update);
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


        
        public (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage)
            => _pipelines[inputMessage.GetType()].Handle(inputMessage);
    }


    class EventBroadcast
    {
        private readonly HashSet<UpdateContextModel> _subscribers = new HashSet<UpdateContextModel>();

        public void Subscribe(UpdateContextModel update) => _subscribers.Add(update);

        public void Update(Event[] events, string version, long finalEventNumber) {
            foreach (var sub in _subscribers) sub(events, version, finalEventNumber);
        }
    }
}