using nsimpleeventstore.contract;
using nsimplemessagepump.contract.messagecontext;

namespace nsimplemessagepump.contract.messageprocessing
{
    public delegate (CommandStatus,IEvent[],EventId,Notification[]) ProcessCommand(IMessage msg, IMessageContextModel ctx, EventId lastEventId);

    
    public interface ICommandProcessor
    {
        (CommandStatus, IEvent[], EventId, Notification[]) Process(IMessage msg, IMessageContextModel ctx, EventId lastEventId);
    }
}