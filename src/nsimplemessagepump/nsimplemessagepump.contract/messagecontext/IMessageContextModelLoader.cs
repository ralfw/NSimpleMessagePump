namespace nsimplemessagepump.contract.messagecontext
{
    public delegate (IMessageContextModel Ctx, string Version) LoadContextModel(IMessage input);
    
    public interface IMessageContextModelLoader {
        (IMessageContextModel Ctx, string Version) Load(IMessage msg);
    }
}