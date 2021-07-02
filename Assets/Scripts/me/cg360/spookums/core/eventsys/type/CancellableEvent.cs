namespace me.cg360.spookums.core.eventsys.type
{
    public class CancellableEvent : BaseEvent, ICancellable
    {
        private bool _isEventCancelled;

        public bool IsCancelled()
        {
            return _isEventCancelled;
        }

        public void SetCancelled(bool isCancelled)
        {
            this._isEventCancelled = isCancelled;
        }

        public void SetCancelled()
        {
            this._isEventCancelled = true;
        }
    }
}