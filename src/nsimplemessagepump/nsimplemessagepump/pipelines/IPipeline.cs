namespace nsimplemessagepump.pipelines
{
    interface IPipeline
    {
        Response Handle(IMessage msg);
    }
}