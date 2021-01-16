using System;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.pipeline
{
    public class QueryPipeline : IPipeline
    {
        private readonly LoadContextModel _load;
        private readonly ProcessQuery _process;

        public QueryPipeline(LoadContextModel load, ProcessQuery process) {
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