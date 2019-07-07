using System;
using System.Collections.Generic;
using nsimpleeventstore;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;
using nsimplemessagepump.pipeline;
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

        class MyNotificationCtx : IMessageContextModel {
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


            (IMessageContextModel Ctx, string Version) loadContext(IMessage msg) {
                return (new MyNotificationCtx() {Value = "foo"}, "");
            }

            Command[] processNotification(IMessage msg, IMessageContextModel ctx)
            {
                var prefix = (msg as MyNotification).Prefix;
                var value = (ctx as MyNotificationCtx).Value;
                
                return new[] {new MyCommand{Parameter = prefix+value}, (Command) new YourCommand{Parameter = prefix+value}};
            }
        }


        class MockPump : IMessagePump
        {
            public List<Command> Commands = new List<Command>();

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand)
            {}

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessCommand processCommand)
            {}

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ICommandProcessor processor)
            {}

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessQuery processQuery)
            {}

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, IQueryProcessor processor)
            {}

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessNotification processNotification)
            {}

            public void Register<TMessage>(IMessageContextModelManager ctxModelManager, INotificationProcessor processor)
            {}

            public void Register<TMessage>(LoadContextModel load, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand, UpdateContextModel update) {}
            public void Register<TMessage>(LoadContextModel load, ProcessCommand processCommand, UpdateContextModel update){}
            public void Register<TMessage>(LoadContextModel load, ProcessQuery process, UpdateContextModel update){}
            public void Register<TMessage>(LoadContextModel load, ProcessNotification process, UpdateContextModel update){}

            public (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage){
                Commands.Add((Command)inputMessage);
                return (null, null);
            }
        }
    }
}