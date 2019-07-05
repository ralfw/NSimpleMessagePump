using System;
using nsimpleeventstore;
using nsimplemessagepump.messagecontext;
using nsimplemessagepump.pipeline;

namespace nsimplemessagepump
{
    public partial class MessagePump
    {
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand)
            => Register<TMessage>(ctxModelManager.Load, processCommand, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessCommand processCommand)
            => Register<TMessage>(ctxModelManager.Load, processCommand, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ICommandProcessor processor)
            => Register<TMessage>(ctxModelManager.Load, processor.Process, ctxModelManager.Update);        
        
        
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessQuery processQuery)
            => Register<TMessage>(ctxModelManager.Load, processQuery, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, IQueryProcessor processor)
            => Register<TMessage>(ctxModelManager.Load, processor.Process, ctxModelManager.Update);
        
        
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessNotification processNotification)
            => Register<TMessage>(ctxModelManager.Load, processNotification, ctxModelManager.Update);
        public void Register<TMessage>(IMessageContextModelManager ctxModelManager, INotificationProcessor processor)
            => Register<TMessage>(ctxModelManager.Load, processor.Process, ctxModelManager.Update);
    }
}