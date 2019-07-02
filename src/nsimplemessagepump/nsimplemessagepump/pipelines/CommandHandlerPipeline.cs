using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class CommandHandlerPipeline : IHandlerPipeline
    {
        private readonly IEventstore _es;
        private readonly Func<IMessage, IMessageContext> _load;
        private readonly Func<IMessage, IMessageContext, (CommandStatus, Event[])> _process;
        private readonly Action<Event[]> _update;

        public CommandHandlerPipeline(IEventstore es, Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, (CommandStatus, Event[])> process, Action<Event[]> update) {
            _es = es;
            _load = load;
            _process = process;
            _update = update;
        }
            
        public Response Handle(IMessage msg) {
            var ctx = _load(msg);
            (var status, Event[] events) = _process(msg, ctx);
            _es.Record(events);
            _update(events);
            return status;
        }
    }
}