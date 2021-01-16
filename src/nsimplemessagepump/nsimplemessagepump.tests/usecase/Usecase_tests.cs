using System.Linq;
using nsimpleeventstore;
using nsimpleeventstore.adapters.eventrepositories;
using nsimplemessagepump.contract;
using nsimplemessagepump.tests.usecase.messages.commands;
using nsimplemessagepump.tests.usecase.messages.notifications.outgoing;
using nsimplemessagepump.tests.usecase.messages.queries;
using nsimplemessagepump.tests.usecase.pipelines.commands;
using nsimplemessagepump.tests.usecase.pipelines.queries;
using Xunit;

namespace nsimplemessagepump.tests.usecase
{
    public class Usecase_tests
    {
        [Fact]
        public void Fluent()
        {
            var es = new Eventstore<InMemoryEventRepository>();
            var sut = new MessagePump(es);


            sut.On<AddTodoCmd>().Use(new AddTodoCmdCtxModelManager()).Do(new AddTodoCmdProcessor());
            sut.On<AllTodosQuery>().Use(new AllTodosQueryCtxModelManager()).Do(new AllTodosQueryProcessor());
            var checkToDoCtxModelManager = new CheckTodoCmdCtxModelManager();
            sut.On<CheckTodoCmd>().Load(checkToDoCtxModelManager).Finally(checkToDoCtxModelManager).Do(new CheckTodoCmdProcessor());

            
            // add a couple of tasks
            var response = sut.Handle(new AddTodoCmd {Subject = "foo"});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);
            
            response = sut.Handle(new AddTodoCmd {Subject = "bar"});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);
            
            response = sut.Handle(new AddTodoCmd {Subject = "baz"});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);

            // what are the currently active tasks?
            response = sut.Handle(new AllTodosQuery());
            Assert.Empty(response.Notifications);
            var result = response.Msg as AllTodosQueryResult;
            Assert.Equal(new[]{"foo", "bar", "baz"}, result.Todos.Select(x=>x.Description));

            // check a task as done
            response = sut.Handle(new CheckTodoCmd {Id = result.Todos[0].Id});
            Assert.IsType<Success>(response.Msg);
            Assert.Empty(response.Notifications);
            
            // ...and it still gets listed
            response = sut.Handle(new AllTodosQuery());
            Assert.Empty(response.Notifications);
            result = response.Msg as AllTodosQueryResult;
            Assert.Equal(new[]{"foo, done", "bar", "baz"}, result.Todos.Select(x=>x.Description));
            
            // ...unless done tasks are explicitly excluded
            response = sut.Handle(new AllTodosQuery{ActiveOnly = true});
            Assert.Empty(response.Notifications);
            result = response.Msg as AllTodosQueryResult;
            Assert.Equal(new[]{"bar", "baz"}, result.Todos.Select(x=>x.Description));
            
            // check all remaining tasks - and a notification gets created, once all are done
            sut.Handle(new CheckTodoCmd {Id = result.Todos[0].Id});
            response = sut.Handle(new CheckTodoCmd {Id = result.Todos[1].Id});
            Assert.IsType<Success>(response.Msg);
            Assert.Single(response.Notifications);
            Assert.IsType<FreeCapacityAvailableNotification>(response.Notifications[0]);
        }
    }
}