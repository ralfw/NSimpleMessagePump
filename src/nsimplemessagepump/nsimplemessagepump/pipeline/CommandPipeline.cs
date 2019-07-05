using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipeline.processors;

namespace nsimplemessagepump.pipeline
{
    class CommandPipeline : IPipeline
    {
        private readonly IEventstore _es;
        private readonly LoadContextModel _load;
        private readonly ProcessCommand _process;
        private readonly UpdateContextModel _update;


        // ctor for processor which does not generate notifications
        public CommandPipeline(IEventstore es, LoadContextModel load, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> process, UpdateContextModel update)
            : this(es, 
                   load, 
                   (msg, ctx, version) => {
                       var (status, events, expectedVersion) = process(msg, ctx, version);
                       return (status, events, expectedVersion, new Notification[0]); // always return no notifications
                   }, 
                   update) {}
        
        // ctor for processor which generates notifications
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
            var (version, finalEventNumber) =_es.Record(events, expectedVersion); //TODO: handle ES version conflict
            _update(events, version, finalEventNumber);
            return (status, notifications);
        }
    }
}