using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class AddTodoCmdCtxModelManager : IMessageContextModelManager
    {
        public (IMessageContextModel Ctx, string Version) Load(IMessage msg) {
            return (new AddToDoCmdCtxModel(), "");
        }

        public void Update(Event[] events, string version, long finalEventNumber) {}
    }
}