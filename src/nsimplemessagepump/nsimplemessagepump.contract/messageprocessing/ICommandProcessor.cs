using nsimpleeventstore.contract;
using nsimplemessagepump.contract.messagecontext;

namespace nsimplemessagepump.contract.messageprocessing
{
    public delegate (CommandStatus,Event[],string,Notification[]) ProcessCommand(IMessage msg, IMessageContextModel ctx, string version);

    
    public interface ICommandProcessor
    {
        (CommandStatus, Event[], string, Notification[]) Process(IMessage msg, IMessageContextModel ctx, string version);
    }
}