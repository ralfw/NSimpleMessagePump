using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.tests.usecase.pipelines.queries
{
    class AllTodosQueryCtxModel : IMessageContextModel {
        public class Todo
        {
            public string Id;
            public string Subject;
            public bool Done;
        }
            
        public Todo[] Todos;
    }
}