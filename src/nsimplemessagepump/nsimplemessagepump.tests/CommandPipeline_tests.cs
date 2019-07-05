using System;
using System.Collections.Generic;
using System.Linq;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
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
            Event[] updateEvents = null;
            var es = new MockEventStore();
            var sut = new CommandPipeline(es, Load, Process_without_notifications, Update);

            var cmd = new MyCommand {Parameter = "123"};
            var result = sut.Handle(cmd);
            
            Assert.IsType<Success>(result.Msg);
            Assert.Empty(result.Notifications);

            Assert.Equal(2, es.Events.Count);
            Assert.Equal("54321", (es.Events[0] as MyEvent).Reversed);
            Assert.Equal(5, (es.Events[1] as YourEvent).Count);
            Assert.Equal(2, updateEvents.Length);
            
            
            sut = new CommandPipeline(es, Load, Process_with_notifications, Update);
            
            result = sut.Handle(cmd);
            
            Assert.Single(result.Notifications);
            Assert.Equal("123", (result.Notifications[0] as MyNotification).Original);
            Assert.Equal(4, es.Events.Count);
            

            (CommandStatus, Event[], string) Process_without_notifications(IMessage msg, IMessageContextModel ctx, string version)
            {
                var value = (msg as MyCommand).Parameter + (ctx as MyCommandCtx).Value;
                return (new Success(), new[] {
                    new MyEvent{Reversed = new string(value.Reverse().ToArray())}, 
                    (Event) new YourEvent{Count = value.Length}
                }, version);
            }
            
            (CommandStatus, Event[], string, Notification[]) Process_with_notifications(IMessage msg, IMessageContextModel ctx, string version)
            {
                var r = Process_without_notifications(msg, ctx, version);
                return (r.Item1, r.Item2, r.Item3, new[]{new MyNotification{Original = (msg as MyCommand).Parameter}});
            }

            (IMessageContextModel Ctx, string Version) Load(IMessage input)
            {
                return (new MyCommandCtx {Value = "45"}, "");
            }
            
            void Update(Event[] events, string version, long finaleventnumber) {
                updateEvents = events;
            }
        }


        class MockEventStore : IEventstore
        {
            public List<Event> Events = new List<Event>();
            
            public void Dispose() {}
            public (string Version, long FinalEventNumber) Record(Event e, string expectedVersion = "") { throw new NotImplementedException(); }
            public (string Version, long FinalEventNumber) Record(Event[] events, string expectedVersion = "") {
                Events.AddRange(events);
                return (Events.Count.ToString(), Events.Count - 1);
            }

            public (string Version, Event[] Events) Replay(long firstEventNumber = -1) { throw new NotImplementedException(); }
            public (string Version, Event[] Events) Replay(params Type[] eventTypes) { throw new NotImplementedException(); }
            public (string Version, Event[] Events) Replay(long firstEventNumber, params Type[] eventTypes) { throw new NotImplementedException(); }

            public (string Version, long FinalEventNumber) State { get; }
            public event Action<string, long, Event[]> OnRecorded;
        }
    }
}