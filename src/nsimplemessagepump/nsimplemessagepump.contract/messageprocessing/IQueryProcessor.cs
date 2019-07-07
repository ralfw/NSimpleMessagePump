using nsimplemessagepump.contract.messagecontext;

namespace nsimplemessagepump.contract.messageprocessing
{
    public delegate QueryResult ProcessQuery(IMessage msg, IMessageContextModel ctx);

    
    public interface IQueryProcessor
    {
        QueryResult Process(IMessage msg, IMessageContextModel ctx);
    }
}