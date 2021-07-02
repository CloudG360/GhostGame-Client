namespace me.cg360.spookums.core.eventsys.type
{
    public interface ICancellable
    {
        bool IsCancelled();
        void SetCancelled(bool cancelled);
    }
}