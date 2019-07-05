using System.Collections.Generic;
using System.Linq;
using System.Threading;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class Usecase_tests
    {
        class AddTodoCmd : Command {
            public string Subject;
        }
        class AddToDoCmdCtxModel : IMessageContextModel {}
        

        class CheckTodoCmd : Command {
            public string Id;
        }
        class CheckTodoCmdCtxModel : IMessageContextModel {
            public string[] Ids;
        }

        
        class AllTodosQuery : Query {
            public bool ActiveOnly;
        }
        class AllTodosQueryResult : QueryResult {
            public class TodoInfo {
                public string Id;
                public string Description;
            }
            
            public TodoInfo[] Todos;
        }

        class AllTodosQueryCtxModel : IMessageContextModel {
            public class Todo
            {
                public string Id;
                public string Subject;
                public bool Done;
            }
            
            public Todo[] Todos;
        }
        
        
        class FreeCapacityAvailable : Notification {}

        
        class TodoCreated : Event {
            public string Subject;
        }

        class TodoChecked : Event {
            public string TodoId;
        }
        


        class AddTodoCmdContextModelManager : IMessageContextModelManager
        {
            public (IMessageContextModel Ctx, string Version) Load(IMessage msg) {
                return (new AddToDoCmdCtxModel(), "");
            }

            public void Update(Event[] events, string version, long finalEventNumber) {}
        }

        
        class CheckTodoCmdContextModelManager : IMessageContextModelManager
        {
            private List<string> _ids = new List<string>();

            public (IMessageContextModel Ctx, string Version) Load(IMessage msg)
                => (new CheckTodoCmdCtxModel {Ids = _ids.ToArray()}, "");

            public void Update(Event[] events, string version, long finalEventNumber)
                => _ids = events.Aggregate(_ids, Apply);
            
            private List<string> Apply(List<string> ids, Event e) {
                switch (e) {
                    case TodoCreated cr:
                        ids.Add(cr.Id);
                        break;
                    case TodoChecked ch:
                        ids.Remove(ch.TodoId);
                        break;
                }
                return ids;
            }
        }
        
        
        class AllTodosQueryCtxModelManager : IMessageContextModelManager
        {
            private List<AllTodosQueryCtxModel.Todo> _todos = new List<AllTodosQueryCtxModel.Todo>();
            
            public (IMessageContextModel Ctx, string Version) Load(IMessage msg) {
                var query = msg as AllTodosQuery;
                var relevantTodos = query.ActiveOnly ? _todos.Where(x => !x.Done) : _todos;
                return (new AllTodosQueryCtxModel {Todos = relevantTodos.ToArray()}, "");
            }

            public void Update(Event[] events, string version, long finalEventNumber)
                => _todos = events.Aggregate(_todos, Apply);

            private List<AllTodosQueryCtxModel.Todo> Apply(List<AllTodosQueryCtxModel.Todo> todos, Event e) {
                switch (e) {
                    case TodoCreated cr:
                        todos.Add(new AllTodosQueryCtxModel.Todo{Id = cr.Id, Subject = cr.Subject, Done = false});
                        break;
                    case TodoChecked ch:
                        todos.First(x => x.Id == ch.TodoId).Done = true;
                        break;
                }
                return todos;
            }
        }
        
        
        
        
        [Fact]
        public void Run()
        {
            var es = new InMemoryEventstore();
            var sut = new MessagePump(es);
            
            sut.Register<AddTodoCmd>(new AddTodoCmdContextModelManager(), 
                (msg, ctx, _) => (new Success(), 
                                  new Event[]{new TodoCreated{Subject = (msg as AddTodoCmd).Subject}},
                                  "")
            );

            sut.Register<CheckTodoCmd>(new CheckTodoCmdContextModelManager(),
                (msg, ctx, _) => {
                    var cmd = msg as CheckTodoCmd;
                    var model = ctx as CheckTodoCmdCtxModel;
                    if (model.Ids.Contains(cmd.Id) is false) return (new Failure("Todo item not registered as active."), new Event[0], "", new Notification[0]);

                    var notifications = new Notification[0];
                    if (model.Ids.Length == 1)
                        notifications = new Notification[] {new FreeCapacityAvailable()};
                    
                    return (new Success(), new Event[] {new TodoChecked {TodoId = cmd.Id}}, "", notifications);
                }
            );
            
            sut.Register<AllTodosQuery>(new AllTodosQueryCtxModelManager(),
                (_, ctx) => {
                    var model = ctx as AllTodosQueryCtxModel;
                    var todos = model.Todos.Select(Map);
                    return new AllTodosQueryResult {Todos = todos.ToArray()};
                }
            );



            var response = sut.Handle(new AddTodoCmd {Subject = "foo"});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);
            
            response = sut.Handle(new AddTodoCmd {Subject = "bar"});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);
            
            response = sut.Handle(new AddTodoCmd {Subject = "baz"});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);


            response = sut.Handle(new AllTodosQuery());
            Assert.Empty(response.Notifications);
            var result = response.Msg as AllTodosQueryResult;
            Assert.Equal(new[]{"foo", "bar", "baz"}, result.Todos.Select(x=>x.Description));

            response = sut.Handle(new CheckTodoCmd {Id = result.Todos[0].Id});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);
            
            response = sut.Handle(new AllTodosQuery());
            Assert.Empty(response.Notifications);
            result = response.Msg as AllTodosQueryResult;
            Assert.Equal(new[]{"foo, done", "bar", "baz"}, result.Todos.Select(x=>x.Description));
            
            response = sut.Handle(new AllTodosQuery{ActiveOnly = true});
            Assert.Empty(response.Notifications);
            result = response.Msg as AllTodosQueryResult;
            Assert.Equal(new[]{"bar", "baz"}, result.Todos.Select(x=>x.Description));
            
            sut.Handle(new CheckTodoCmd {Id = result.Todos[0].Id});
            response = sut.Handle(new CheckTodoCmd {Id = result.Todos[1].Id});
            Assert.IsType<Success>(response.Msg);
            Assert.Single(response.Notifications);
            Assert.IsType<FreeCapacityAvailable>(response.Notifications[0]);
            

            AllTodosQueryResult.TodoInfo Map(AllTodosQueryCtxModel.Todo todo)
                => new AllTodosQueryResult.TodoInfo {
                    Id = todo.Id,
                    Description = $"{todo.Subject}" + (todo.Done ? ", done" : "")
                };
        }
    }
}