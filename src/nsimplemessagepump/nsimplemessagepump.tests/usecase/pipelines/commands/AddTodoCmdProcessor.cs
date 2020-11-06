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
        public (CommandStatus, Event[], string, Notification[]) Process(IMessage msg, IMessageContextModel ctx, string version)
            => (new Success(),
                new Event[] {new TodoCreated {Subject = (msg as AddTodoCmd).Subject}},
                "",
                new Notification[0]);
    }
}