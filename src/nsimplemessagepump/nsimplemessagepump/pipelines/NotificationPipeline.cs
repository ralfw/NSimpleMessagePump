using System;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class NotificationPipeline : IPipeline
    {
        private readonly IMessagePump _pump;
        private readonly LoadContext _load;
        private readonly ProcessNotification _process;

        public NotificationPipeline(IMessagePump pump, LoadContext load, ProcessNotification process) {
            _pump = pump;
            _load = load;
            _process = process;
        }
            
        public (Response Msg, Notification[] Notifications) Handle(IMessage msg) {
            var (ctx,_) = _load(msg);
            var commands = _process(msg, ctx);
            foreach (var cmd in commands) _pump.Handle(cmd);
            return (new NoResponse(), new Notification[0]);
        }
    }
}