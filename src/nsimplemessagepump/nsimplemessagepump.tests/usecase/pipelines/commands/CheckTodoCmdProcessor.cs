using System.Linq;
using nsimpleeventstore.contract;
using nsimplemessagepump.contract;
using nsimplemessagepump.contract.messagecontext;
using nsimplemessagepump.contract.messageprocessing;
using nsimplemessagepump.tests.usecase.events;
using nsimplemessagepump.tests.usecase.messages.commands;
using nsimplemessagepump.tests.usecase.messages.notifications.outgoing;

namespace nsimplemessagepump.tests.usecase.pipelines.commands
{
    class CheckTodoCmdProcessor : ICommandProcessor
    {
        public (CommandStatus, IEvent[], EventId, Notification[]) Process(IMessage msg, IMessageContextModel ctx, EventId lastEventId) {
            var cmd = msg as CheckTodoCmd;
            var model = ctx as CheckTodoCmdCtxModel;
            if (model.Ids.Contains(cmd.Id) is false) return (new Failure("Todo item not registered as active."), new Event[0], null, new Notification[0]);

            var notifications = new Notification[0];
            if (model.Ids.Length == 1)
                notifications = new Notification[] {new FreeCapacityAvailableNotification()};
                    
            return (new Success(), new Event[] {new TodoChecked {TodoId = cmd.Id}}, null, notifications);
        }
    }
}