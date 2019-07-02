using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class CommandNotificationHandlerPipeline : IHandlerPipeline
    {
        private readonly IMessagePump _pump;
        private readonly IEventstore _es;
        private readonly Func<IMessage, IMessageContext> _load;
        private readonly Func<IMessage, IMessageContext, (CommandStatus, Event[], Notification[])> _process;
        private readonly Action<Event[]> _update;

        public CommandNotificationHandlerPipeline(IMessagePump pump, IEventstore es, Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, (CommandStatus, Event[], Notification[])> process, Action<Event[]> update) {
            _pump = pump;
            _es = es;
            _load = load;
            _process = process;
            _update = update;
        }
            
        public Response Handle(IMessage msg) {
            var ctx = _load(msg);
            (var status, Event[] events, Notification[] notifications) = _process(msg, ctx);
            _es.Record(events);
            _update(events);
            foreach (var notification in notifications) _pump.Handle(notification);
            return status;
        }
    }
}