using System.Collections.Generic;
using nsimpleeventstore;

namespace nsimplemessagepump.messagecontext
{
    public interface IMessageContextBuilder {
        void Update(IEnumerable<Event> events, string version, long finalEventNumber);
    }
}