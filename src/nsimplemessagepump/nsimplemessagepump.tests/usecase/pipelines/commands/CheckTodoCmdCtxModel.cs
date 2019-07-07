using nsimplemessagepump.contract.messagecontext;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class CheckTodoCmdCtxModel : IMessageContextModel {
        public string[] Ids;
    }
}