namespace nsimplemessagepump.messagecontext
{
    public interface IMessageContextLoader {
        (IMessageContext Ctx, string Version) Load(IMessage input);
    }
}