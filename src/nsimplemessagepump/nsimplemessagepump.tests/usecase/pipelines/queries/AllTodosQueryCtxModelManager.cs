using System.Collections.Generic;
using System.Linq;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.tests.usecase.events;
using nsimplemessagepump.tests.usecase.messages.queries;

namespace nsimplemessagepump.tests.usecase.pipelines.queries
{
    class AllTodosQueryCtxModelManager : IMessageContextModelManager
    {
        private List<AllTodosQueryCtxModel.Todo> _todos = new List<AllTodosQueryCtxModel.Todo>();
            
        public (IMessageContextModel Ctx, EventId lastEventId) Load(IMessage msg) {
            var query = msg as AllTodosQuery;
            var relevantTodos = query.ActiveOnly ? _todos.Where(x => !x.Done) : _todos;
            return (new AllTodosQueryCtxModel {Todos = relevantTodos.ToArray()}, null);
        }

        public void Update(IEvent[] events, EventId lastEventId)
            => _todos = events.Aggregate(_todos, Apply);

        private List<AllTodosQueryCtxModel.Todo> Apply(List<AllTodosQueryCtxModel.Todo> todos, IEvent e) {
            switch (e) {
                case TodoCreated cr:
                    todos.Add(new AllTodosQueryCtxModel.Todo{Id = cr.Id.Value.ToString(), Subject = cr.Subject, Done = false});
                    break;
                case TodoChecked ch:
                    todos.First(x => x.Id == ch.TodoId).Done = true;
                    break;
            }
            return todos;
        }
    }
}