using System;
using nsimpleeventstore;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.contract
{
    public interface IMessagePump
    {
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand);
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessCommand processCommand);
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, ICommandProcessor processor);
        
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessQuery processQuery);
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, IQueryProcessor processor);
        
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, ProcessNotification processNotification);
        void Register<TMessage>(IMessageContextModelManager ctxModelManager, INotificationProcessor processor);
        
        
        void Register<TMessage>(LoadContextModel load, Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessCommand processCommand, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessQuery processQuery, UpdateContextModel update);
        void Register<TMessage>(LoadContextModel load, ProcessNotification processNotification, UpdateContextModel update);
        
        
        (Response Msg, Notification[] Notifications) Handle(IIncoming inputMessage);
    }
}