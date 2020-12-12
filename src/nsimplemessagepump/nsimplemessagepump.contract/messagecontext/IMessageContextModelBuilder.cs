using nsimpleeventstore.contract;

namespace nsimplemessagepump.contract.messagecontext
{
    public delegate void UpdateContextModel(Event[] events, string version, long finalEventNumber);
    
    public interface IMessageContextModelBuilder {
        void Update(Event[] events, string version, long finalEventNumber);
    }
}