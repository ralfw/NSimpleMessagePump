using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.pipeline
{
    interface IPipeline
    {
        (Response Msg, Notification[] Notifications) Handle(IMessage msg);
    }
}