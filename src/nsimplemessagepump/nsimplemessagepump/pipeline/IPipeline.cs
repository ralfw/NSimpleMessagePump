namespace nsimplemessagepump.pipeline
{
    interface IPipeline
    {
        (Response Msg, Notification[] Notifications) Handle(IMessage msg);
    }
}