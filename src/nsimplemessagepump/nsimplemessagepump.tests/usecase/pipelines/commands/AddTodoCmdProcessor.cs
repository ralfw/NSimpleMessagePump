using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;
using nsimplemessagepump.tests.usecase.events;
using nsimplemessagepump.tests.usecase.messages.commands;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class AddTodoCmdProcessor : ICommandProcessor
    {
        public (CommandStatus, IEvent[], EventId, Notification[]) Process(IMessage msg, IMessageContextModel ctx, EventId lastEventId)
            => (new Success(),
                new Event[] {new TodoCreated {Subject = (msg as AddTodoCmd).Subject}},
                null,
                new Notification[0]);
    }
}