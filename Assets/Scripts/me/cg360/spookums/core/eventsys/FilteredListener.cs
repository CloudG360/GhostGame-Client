using System.Collections.Generic;
using me.cg360.spookums.core.eventsys.filter;
using me.cg360.spookums.core.eventsys.handler;

namespace me.cg360.spookums.core.eventsys
{
    public class FilteredListener : Listener
    {
        private readonly List<IEventFilter> filters;

        public FilteredListener(object sourceObject, params IEventFilter[] filters) : base(sourceObject) {
            this.filters = new List<IEventFilter>();
            this.filters.AddRange(filters);
        }
        
        public override List<HandlerMethodPair> GetEventMethods(BaseEvent e) {
            List<HandlerMethodPair> methods = base.GetEventMethods(e);
            // Pass to filters. They can edit the map.
            return methods;
        }

        public IEventFilter[] GetFilters() {
            return filters.ToArray();
        }
    }
}