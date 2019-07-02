namespace nsimplemessagepump.messagecontext
{
    public interface IMessageContextLoader {
        IMessageContext Load(IMessage input);
    }
}