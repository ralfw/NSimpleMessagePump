using System.Collections.Generic;
using System.Linq;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.tests.usecase.events;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class CheckTodoCmdCtxModelManager : IMessageContextModelManager
    {
        private List<string> _ids = new List<string>();

        public (IMessageContextModel Ctx, EventId lastEventId) Load(IMessage msg)
            => (new CheckTodoCmdCtxModel {Ids = _ids.ToArray()}, null);

        public void Update(IEvent[] events, EventId lastEventId)
            => _ids = events.Aggregate(_ids, Apply);
            
        private List<string> Apply(List<string> ids, IEvent e) {
            switch (e) {
                case TodoCreated cr:
                    ids.Add(cr.Id.Value.ToString());
                    break;
                case TodoChecked ch:
                    ids.Remove(ch.TodoId);
                    break;
            }
            return ids;
        }
    }
}