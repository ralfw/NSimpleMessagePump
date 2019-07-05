using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipeline
{
    public delegate (CommandStatus,Event[],string,Notification[]) ProcessCommand(IMessage msg, IMessageContextModel ctx, string version);

    
    public interface ICommandProcessor
    {
        (CommandStatus, Event[], string, Notification[]) Process(IMessage msg, IMessageContextModel ctx, string version);
    }
}