using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class CommandPipeline : IPipeline
    {
        private readonly IEventstore _es;
        private readonly Func<IMessage, (IMessageContext,string)> _load;
        private readonly Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string, Notification[])> _process;
        private readonly Action<Event[],string,long> _update;


        // ctor for processor which does not generate notifications
        public CommandPipeline(IEventstore es, Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string)> process, Action<Event[], string, long> update)
            : this(es, 
                   load, 
                   (msg, ctx, version) => {
                       var (status, events, expectedVersion) = process(msg, ctx, version);
                       return (status, events, expectedVersion, new Notification[0]); // always return no notifications
                   }, 
                   update) {}
        
        // ctor for processor which generates notifications
        public CommandPipeline(IEventstore es, Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string, Notification[])> process, Action<Event[], string, long> update)
        {
            _es = es;
            _load = load;
            _process = process;
            _update = update;
        }
            
        
        public (Response, Notification[]) Handle(IMessage msg) {
            var (ctx, loadedVersion) = _load(msg);
            var (status, events, expectedVersion, notifications) = _process(msg, ctx, loadedVersion);
            var (version, finalEventNumber) =_es.Record(events, expectedVersion); //TODO: handle ES version conflict
            _update(events, version, finalEventNumber);
            return (status, notifications);
        }
    }
}