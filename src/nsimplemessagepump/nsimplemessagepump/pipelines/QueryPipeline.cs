using System;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    class QueryPipeline : IPipeline
    {
        private readonly LoadContext _load;
        private readonly ProcessQuery _process;

        public QueryPipeline(LoadContext load, ProcessQuery process) {
            _load = load;
            _process = process;
        }
            
        public (Response Msg, Notification[] Notifications) Handle(IMessage msg) {
            var (ctx,_) = _load(msg);
            var result = _process(msg, ctx);
            return (result, new Notification[0]);
        }
    }
}