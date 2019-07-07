using nsimplemessagepump.contract;

namespace nsimplemessagepump.tests.usecase.messages.queries
{
    class AllTodosQueryResult : QueryResult {
        public class TodoInfo {
            public string Id;
            public string Description;
        }
            
        public TodoInfo[] Todos;
    }
}