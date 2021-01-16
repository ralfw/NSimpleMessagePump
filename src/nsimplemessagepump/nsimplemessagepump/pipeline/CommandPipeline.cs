using System;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.pipeline
{
    public class CommandPipeline : IPipeline
    {
        private readonly IEventstore _es;
        private readonly LoadContextModel _load;
        private readonly ProcessCommand _process;
        private readonly UpdateContextModel _update;

        public CommandPipeline(IEventstore es, LoadContextModel load, ProcessCommand process, UpdateContextModel update)
        {
            _es = es;
            _load = load;
            _process = process;
            _update = update;
        }
            
        
        public (Response Msg, Notification[] Notifications) Handle(IMessage msg) {
            var (ctx, loadedVersion) = _load(msg);
            var (status, events, expectedVersion, notifications) = _process(msg, ctx, loadedVersion);
            _es.Record(expectedVersion, events); //TODO: handle ES version conflict
            _update(events, events[events.Length - 1]?.Id);
            return (status, notifications);
        }
    }
}