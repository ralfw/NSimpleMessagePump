using nsimpleeventstore;
using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipelines
{
    public delegate (CommandStatus,Event[],string,Notification[]) ProcessCommand(IMessage msg, IMessageContext ctx, string version);
    public delegate QueryResult ProcessQuery(IMessage msg, IMessageContext ctx);
    public delegate Command[] ProcessNotification(IMessage msg, IMessageContext ctx);    
    
    
    interface IPipeline
    {
        (Response, Notification[]) Handle(IMessage msg);
    }
}