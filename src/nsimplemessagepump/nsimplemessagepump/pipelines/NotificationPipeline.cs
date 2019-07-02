using System;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class NotificationPipeline : IPipeline
    {
        private readonly IMessagePump _pump;
        private readonly Func<IMessage, (IMessageContext,string)> _load;
        private readonly Func<IMessage, IMessageContext, Command[]> _process;

        public NotificationPipeline(IMessagePump pump, Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, Command[]> process) {
            _pump = pump;
            _load = load;
            _process = process;
        }
            
        public (Response,Notification[]) Handle(IMessage msg) {
            var (ctx,_) = _load(msg);
            var commands = _process(msg, ctx);
            foreach (var cmd in commands) _pump.Handle(cmd);
            return (new NoResponse(), new Notification[0]);
        }
    }
}