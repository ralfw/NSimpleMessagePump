using System.Collections.Generic;
using nsimpleeventstore;

namespace nsimplemessagepump.messagecontext
{
    public delegate void UpdateContext(Event[] events, string version, long finalEventNumber);
    
    public interface IMessageContextBuilder {
        void Update(Event[] events, string version, long finalEventNumber);
    }
}