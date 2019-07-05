using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class Usecase_tests
    {
        class AddTodoCmd : Command
        {
            public string Subject;
        }
        class AddToDoCmdCtxModel : IMessageContext {}

        class CheckTodoCmd : Command
        {
            public string Id;
        }
        class CheckTodoCmdCtxModel : IMessageContext
        {
            public string[] Ids;
        }

        class AllTodosQuery : Query
        {
            public bool NotDoneYetOnly;
        }
        class AllTodosQueryResult : QueryResult
        {
            public class TodoInfo
            {
                public string Id;
                public string Subject;
                public bool Done;
            }
            
            public TodoInfo[] Todos;
        }

        class TodoCreated : Event
        {
            public string Subject;
        }

        class TodoChecked : Event
        {
            public string Id;
        }


        class AddTodoCmdContextModelManager : IMessageContextManager
        {
            public (IMessageContext Ctx, string Version) Load(IMessage input) {
                return (new AddToDoCmdCtxModel(), "");
            }

            public void Update(Event[] events, string version, long finalEventNumber) {}
        }
        
        
        
        
        [Fact]
        public void Run()
        {
            var es = new InMemoryEventstore();
            var sut = new MessagePump(es);
            
            sut.Register<AddTodoCmd>(new AddTodoCmdContextModelManager(), 
                                    (msg, ctx, version) => (new Success(),
                                                            new Event[]{new TodoCreated{Subject = (msg as AddTodoCmd).Subject}},
                                                            ""));
            
        }
    }
}