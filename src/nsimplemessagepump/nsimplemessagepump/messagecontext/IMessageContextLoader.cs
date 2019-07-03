namespace nsimplemessagepump.messagecontext
{
    public delegate (IMessageContext Ctx, string Version) LoadContext(IMessage input);
    
    public interface IMessageContextLoader {
        (IMessageContext Ctx, string Version) Load(IMessage input);
    }
}