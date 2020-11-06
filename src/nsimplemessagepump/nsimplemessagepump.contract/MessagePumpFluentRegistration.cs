using System;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;

namespace nsimplemessagepump.contract
{
    public class MessagePumpFluentRegistration<TIncomingMessage> where TIncomingMessage : IIncoming
    {
        private readonly IMessagePump _messagePump;
        
        private LoadContextModel _load = msg => (new EmptyContextModel(), "");
        private UpdateContextModel _update = (events, version, finalEventNumber) => { };

        
        internal MessagePumpFluentRegistration(IMessagePump messagePump) { _messagePump = messagePump; }
        
        
        public MessagePumpFluentRegistration<TIncomingMessage> Use(IMessageContextModelManager ctxModelManager) {
            _load = ctxModelManager.Load;
            _update = ctxModelManager.Update;
            return this;
        }

        public MessagePumpFluentRegistration<TIncomingMessage> Load(IMessageContextModelLoader loader) => Load(loader.Load);
        public MessagePumpFluentRegistration<TIncomingMessage> Load(LoadContextModel load) {
            _load = load;
            return this;
        }

        public MessagePumpFluentRegistration<TIncomingMessage> Finally(IMessageContextModelBuilder builder) =>  Finally(builder.Update);
        public MessagePumpFluentRegistration<TIncomingMessage> Finally(UpdateContextModel update) {
            _update = update;
            return this;
        }


        public void Do(Func<IMessage, IMessageContextModel, string, (CommandStatus, Event[], string)> processCommand)  {
            _messagePump.Register<TIncomingMessage>(
                _load,
                (msg, ctx, ver) => {
                    var (status,events,version) = processCommand(msg, ctx, ver);
                    return (status, events, version, new Notification[0]);
                }, 
                _update);
        }
        public void Do(ICommandProcessor processor) => Do(processor.Process);
        public void Do(ProcessCommand process) => _messagePump.Register<TIncomingMessage>(_load, process, _update);

        public void Do(IQueryProcessor processor) => Do(processor.Process);
        public void Do(ProcessQuery process) => _messagePump.Register<TIncomingMessage>(_load, process, _update);

        public void Do(INotificationProcessor processor) => Do(processor.Process);
        public void Do(ProcessNotification process) => _messagePump.Register<TIncomingMessage>(_load, process, _update);
    }
}