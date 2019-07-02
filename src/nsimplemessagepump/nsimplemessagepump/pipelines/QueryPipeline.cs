using System;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class QueryPipeline : IPipeline
    {
        private readonly Func<IMessage, IMessageContext> _load;
        private readonly Func<IMessage, IMessageContext, QueryResult> _process;

        public QueryPipeline(Func<IMessage, IMessageContext> load, Func<IMessage, IMessageContext, QueryResult> process) {
            _load = load;
            _process = process;
        }
            
        public Response Handle(IMessage msg) {
            var ctx = _load(msg);
            return _process(msg, ctx);
        }
    }
}