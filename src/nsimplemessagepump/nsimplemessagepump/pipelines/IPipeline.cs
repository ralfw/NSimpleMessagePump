namespace nsimplemessagepump.pipelines
{
    interface IPipeline
    {
        (Response, Notification[]) Handle(IMessage msg);
    }
}