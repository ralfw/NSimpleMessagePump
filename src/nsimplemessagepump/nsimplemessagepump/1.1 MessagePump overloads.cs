using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipelines;

namespace nsimplemessagepump
{
    public partial class MessagePump
    {
        public void Register<TMessage>(IMessageContextManager ctxManager, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string)> processCommand)
            => Register<TMessage>(ctxManager.Load, processCommand, ctxManager.Update);
        public void Register<TMessage>(IMessageContextLoader loader, Func<IMessage, IMessageContext, string, (CommandStatus, Event[], string)> processCommand, IMessageContextBuilder builder)
            => Register<TMessage>(loader.Load, processCommand, builder.Update);
        
        public void Register<TMessage>(IMessageContextManager ctxManager, ProcessCommand processCommand)
            => Register<TMessage>(ctxManager.Load, processCommand, ctxManager.Update);
        public void Register<TMessage>(IMessageContextLoader loader, ProcessCommand processCommand, IMessageContextBuilder builder)
            => Register<TMessage>(loader.Load, processCommand, builder.Update);
        
        public void Register<TMessage>(IMessageContextLoader loader, ProcessQuery processQuery)
            => Register<TMessage>(loader.Load, processQuery);
        
        public void Register<TMessage>(IMessageContextLoader loader, ProcessNotification processNotification)
            => Register<TMessage>(loader.Load, processNotification);
    }
}