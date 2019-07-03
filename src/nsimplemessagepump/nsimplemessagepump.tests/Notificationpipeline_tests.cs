using System;
using System.Collections.Generic;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;
using Xunit;

namespace nsimplemessagepump.tests
{
    public class Notificationpipeline_tests
    {
        class MyNotification : Notification {
            public string Bar;
        }

        class MyCommand : Command {}
        class YourCommand : Command {}

        class MyQueryCtx : IMessageContext {
            public string Foo;
        }
        
        
        [Fact]
        public void Run()
        {
            var log = new List<string>();
            var pump = new MockPump();
            var sut = new NotificationPipeline(pump, loadContext, processNotification);

            var result = sut.Handle(new MyNotification{Bar = "not"});

            Assert.IsType<NoResponse>(result.Msg);
            Assert.Empty(result.Notifications);
            
            Assert.Equal(new[]{typeof(MyCommand), typeof(YourCommand)}, pump.Commands.ToArray());
            Assert.Equal(new[]{"load-not","process-not","ctx"}, log);


            (IMessageContext Ctx, string Version) loadContext(IMessage msg) {
                log.Add("load-" + ((MyNotification)msg).Bar);
                return (new MyQueryCtx {Foo = "ctx"}, "");
            }

            Command[] processNotification(IMessage msg, IMessageContext ctx)
            {
                log.Add("process-" + ((MyNotification)msg).Bar);
                log.Add(((MyQueryCtx)ctx).Foo);
                return new[] {new MyCommand(), (Command) new YourCommand()};
            }
        }


        class MockPump : IMessagePump
        {
            public List<Type> Commands = new List<Type>();
            
            public void Register<TMessage>(LoadContext load, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string)> processCommand, UpdateContext update) {}
            public void Register<TMessage>(LoadContext load, ProcessCommand processCommand, UpdateContext update){}
            public void Register<TMessage>(LoadContext load, ProcessQuery process){}
            public void Register<TMessage>(LoadContext load, ProcessNotification process){}

            public (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage){
                Commands.Add(inputMessage.GetType());
                return (null, null);
            }
        }
    }
}