namespace nsimplemessagepump.pipelines
{
    interface IHandlerPipeline
    {
        Response Handle(IMessage msg);
    }
}