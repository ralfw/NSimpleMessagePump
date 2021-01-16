using System;
using System.Collections.Generic;
using System.Linq;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.pipeline;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class CommandPipeline_tests
    {
        class MyCommand : Command
        {
            public string Parameter;
        }

        class MyCommandCtx : IMessageContextModel
        {
            public string Value;
        }

        class MyEvent : Event
        {
            public string Reversed;
        }

        class YourEvent : Event
        {
            public int Count;
        }

        class MyNotification : Notification
        {
            public string Original;
        }


        [Fact]
        public void Run()
        {
            IEvent[] updateEvents = null;
            var es = new MockEventStore();
            var sut = new CommandPipeline(es, Load, Process_with_notifications, Update);

            var cmd = new MyCommand {Parameter = "123"};
            var result = sut.Handle(cmd);
            
            Assert.IsType<Success>(result.Msg);
            Assert.Equal(2, es.Events.Count);
            Assert.Equal("54321", (es.Events[0] as MyEvent).Reversed);
            Assert.Equal(5, (es.Events[1] as YourEvent).Count);
            Assert.Equal(2, updateEvents.Length);
            Assert.IsType<MyNotification>(result.Notifications[0]);
            

            (CommandStatus, IEvent[], EventId, Notification[]) Process_with_notifications(IMessage msg, IMessageContextModel ctx, EventId lastEventId) {
                var value = (msg as MyCommand).Parameter + (ctx as MyCommandCtx).Value;
                return (new Success(), 
                        new[]{
                                new MyEvent{Reversed = new string(value.Reverse().ToArray())}, 
                                (Event) new YourEvent{Count = value.Length}}, 
                        null, 
                        new[]{new MyNotification{Original = (msg as MyCommand).Parameter}});
            }

            (IMessageContextModel Ctx, EventId lastEventId) Load(IMessage input) {
                return (new MyCommandCtx {Value = "45"}, null);
            }
            
            void Update(IEvent[] events, EventId lastEventId) {
                updateEvents = events;
            }
        }


        class MockEventStore : IEventstore
        {
            public List<IEvent> Events = new List<IEvent>();
            
            public void Dispose() {}
         
            public void Record(EventId expectedLastEventId, params IEvent[] events) {
                Events.AddRange(events);
            }

            public IEnumerable<IEvent> Replay() { throw new NotImplementedException(); }
            public IEnumerable<IEvent> Replay(EventId startEventId) { throw new NotImplementedException(); }

            public EventId LastEventId { get; }
            public event Action<IEvent[]> OnRecorded;
        }
    }
}