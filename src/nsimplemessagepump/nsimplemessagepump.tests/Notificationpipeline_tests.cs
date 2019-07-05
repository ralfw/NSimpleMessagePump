using System;
using System.Collections.Generic;
using System.Linq;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class Notificationpipeline_tests
    {
        class MyNotification : Notification {
            public string Prefix;
        }

        class MyCommand : Command
        {
            public string Parameter;
        }

        class YourCommand : Command
        {
            public string Parameter;
        }

        class MyNotificationCtx : IMessageContext {
            public string Value;
        }
        
        
        [Fact]
        public void Run()
        {
            var pump = new MockPump();
            var sut = new NotificationPipeline(pump, loadContext, processNotification);

            var result = sut.Handle(new MyNotification{Prefix = ":"});

            Assert.IsType<NoResponse>(result.Msg);
            Assert.Empty(result.Notifications);
            
            Assert.Equal(2, pump.Commands.Count);
            Assert.Equal(":foo", (pump.Commands[0] as MyCommand).Parameter);
            Assert.Equal(":foo", (pump.Commands[1] as YourCommand).Parameter);


            (IMessageContext Ctx, string Version) loadContext(IMessage msg) {
                return (new MyNotificationCtx() {Value = "foo"}, "");
            }

            Command[] processNotification(IMessage msg, IMessageContext ctx)
            {
                var prefix = (msg as MyNotification).Prefix;
                var value = (ctx as MyNotificationCtx).Value;
                
                return new[] {new MyCommand{Parameter = prefix+value}, (Command) new YourCommand{Parameter = prefix+value}};
            }
        }


        class MockPump : IMessagePump
        {
            public List<Command> Commands = new List<Command>();
            
            public void Register<TMessage>(LoadContext load, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string)> processCommand, UpdateContext update) {}
            public void Register<TMessage>(LoadContext load, ProcessCommand processCommand, UpdateContext update){}
            public void Register<TMessage>(LoadContext load, ProcessQuery process){}
            public void Register<TMessage>(LoadContext load, ProcessNotification process){}

            public (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage){
                Commands.Add((Command)inputMessage);
                return (null, null);
            }
        }
    }
}