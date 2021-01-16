using nsimpleeventstore.contract;

namespace nsimplemessagepump.contract.messagecontext
{
    public delegate void UpdateContextModel(IEvent[] events, EventId lastEventId);
    
    public interface IMessageContextModelBuilder {
        void Update(IEvent[] events, EventId lastEventId);
    }
}