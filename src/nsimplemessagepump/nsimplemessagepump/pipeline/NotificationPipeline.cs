using System;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.pipeline
{
    public class NotificationPipeline : IPipeline
    {
        private readonly IMessagePump _pump;
        private readonly LoadContextModel _load;
        private readonly ProcessNotification _process;

        public NotificationPipeline(IMessagePump pump, LoadContextModel load, ProcessNotification process) {
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