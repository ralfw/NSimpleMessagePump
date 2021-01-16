using nsimpleeventstore.contract;

namespace nsimplemessagepump.contract.messagecontext
{
    public delegate (IMessageContextModel Ctx, EventId lastEventId) LoadContextModel(IMessage input);
    
    public interface IMessageContextModelLoader {
        (IMessageContextModel Ctx, EventId lastEventId) Load(IMessage msg);
    }
}