using System;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class NotificationHandlerPipeline : IHandlerPipeline
    {
        private readonly IMessagePump _pump;
        private readonly Func<IMessage, IMessageContext> _load;
        private readonly Func<IMessage, IMessageContext, Command[]> _process;

        public NotificationHandlerPipeline(IMessagePump pump, Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, Command[]> process) {
            _pump = pump;
            _load = load;
            _process = process;
        }
            
        public Response Handle(IMessage msg) {
            var ctx = _load(msg);
            var commands = _process(msg, ctx);
            foreach (var cmd in commands) _pump.Handle(cmd);
            return new NoResponse();
        }
    }
}