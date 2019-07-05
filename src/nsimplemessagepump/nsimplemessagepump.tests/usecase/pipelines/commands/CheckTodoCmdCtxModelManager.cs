using System.Collections.Generic;
using System.Linq;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.tests.usecase.events;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class CheckTodoCmdCtxModelManager : IMessageContextModelManager
    {
        private List<string> _ids = new List<string>();

        public (IMessageContextModel Ctx, string Version) Load(IMessage msg)
            => (new CheckTodoCmdCtxModel {Ids = _ids.ToArray()}, "");

        public void Update(Event[] events, string version, long finalEventNumber)
            => _ids = events.Aggregate(_ids, Apply);
            
        private List<string> Apply(List<string> ids, Event e) {
            switch (e) {
                case TodoCreated cr:
                    ids.Add(cr.Id);
                    break;
                case TodoChecked ch:
                    ids.Remove(ch.TodoId);
                    break;
            }
            return ids;
        }
    }
}