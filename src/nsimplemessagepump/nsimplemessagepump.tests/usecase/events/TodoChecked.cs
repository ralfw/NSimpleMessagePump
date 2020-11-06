using nsimpleeventstore.contract;

namespace nsimplemessagepump.tests.usecase.events
{
    class TodoChecked : Event {
        public string TodoId;
    }
}