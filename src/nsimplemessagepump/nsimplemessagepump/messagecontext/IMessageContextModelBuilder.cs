using System.Collections.Generic;
using nsimpleeventstore;

namespace nsimplemessagepump.messagecontext
{
    public delegate void UpdateContextModel(Event[] events, string version, long finalEventNumber);
    
    public interface IMessageContextModelBuilder {
        void Update(Event[] events, string version, long finalEventNumber);
    }
}