using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class AddTodoCmdCtxModelManager : IMessageContextModelManager
    {
        public (IMessageContextModel Ctx, EventId lastEventId) Load(IMessage msg) {
            return (new AddToDoCmdCtxModel(), null);
        }

        public void Update(IEvent[] events, EventId lastEventId) {}
    }
}