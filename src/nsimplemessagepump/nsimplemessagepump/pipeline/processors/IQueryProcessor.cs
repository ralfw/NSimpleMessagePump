using nsimplemessagepump.messagecontext;

namespace nsimplemessagepump.pipeline.processors
{
    public delegate QueryResult ProcessQuery(IMessage msg, IMessageContextModel ctx);

    
    public interface IQueryProcessor
    {
        QueryResult Process(IMessage msg, IMessageContextModel ctx);
    }
}