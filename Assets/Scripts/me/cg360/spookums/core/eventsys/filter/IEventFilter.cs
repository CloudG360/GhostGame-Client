namespace me.cg360.spookums.core.eventsys.filter
{
    public interface IEventFilter
    {
        bool CheckEvent(BaseEvent eventIn);
    }
}