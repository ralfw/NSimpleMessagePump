using System;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class QueryPipeline : IPipeline
    {
        private readonly Func<IMessage, (IMessageContext,string)> _load;
        private readonly Func<IMessage, IMessageContext, QueryResult> _process;

        public QueryPipeline(Func<IMessage, (IMessageContext,string)> load, Func<IMessage, IMessageContext, QueryResult> process) {
            _load = load;
            _process = process;
        }
            
        public (Response,Notification[]) Handle(IMessage msg) {
            var (ctx,_) = _load(msg);
            var result = _process(msg, ctx);
            return (result, new Notification[0]);
        }
    }
}