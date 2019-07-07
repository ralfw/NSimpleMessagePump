using System.Linq;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;
using nsimplemessagepump.tests.usecase.messages.queries;

namespace nsimplemessagepump.tests.usecase.pipelines.queries
{
    class AllTodosQueryProcessor : IQueryProcessor
    {
        public QueryResult Process(IMessage msg, IMessageContextModel ctx)
        {
            var model = ctx as AllTodosQueryCtxModel;
            var todos = model.Todos.Select(Map);
            return new AllTodosQueryResult {Todos = todos.ToArray()};
        }
  
        AllTodosQueryResult.TodoInfo Map(AllTodosQueryCtxModel.Todo todo)
            => new AllTodosQueryResult.TodoInfo {
                Id = todo.Id,
                Description = $"{todo.Subject}" + (todo.Done ? ", done" : "")
            };

    }
}