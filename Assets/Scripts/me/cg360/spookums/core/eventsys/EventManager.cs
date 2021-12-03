using System;
using System.Collections.Generic;
using me.cg360.spookums.core.eventsys.handler;
using me.cg360.spookums.core.eventsys.type;
using me.cg360.spookums.utility;
using UnityEngine;
using me.cg360.spookums.core.eventsys.filter;

namespace me.cg360.spookums.core.eventsys
{
    public class EventManager
    {
        private static EventManager _primaryManager;

        protected List<IEventFilter> Filters; // Filter for EVERY listener.
        protected List<Listener> Listeners;
        protected List<EventManager> Children; // Send events to children too. Only sent if filter is passed.


        public EventManager(params IEventFilter[] filters)
        {
            this.Filters = new List<IEventFilter>();
            this.Listeners = new List<Listener>();
            this.Children = new List<EventManager>();

            if (filters != null)
            {
                this.Filters.AddRange(filters);
            }
        }


        /**
         * Sets the manager the result provided from EventManager#get() and
         * finalizes the instance to an extent.
         *
         * Cannot be changed once initially called.
         */
        public EventManager SetAsPrimaryManager()
        {
            _primaryManager = this;
            return this;
        }

        public void Call(BaseEvent e)
        {
            List<HandlerMethodPair> callList = new List<HandlerMethodPair>();
            List<Listener> callListeners = new List<Listener>(); // Ordered to match call list
            
            // I don't know C# well enough to fix
            // Should let superclass types of the generic arguments accept the event
            // But it only accepts the specified type.
            
            // And this is the part where it's probably the least efficient.
            // Would be great to bake this but then I can't really use the FilteredListener.
            // Could maybe filter each method as I go?
            foreach (Listener listener in Listeners)
            {
                foreach (HandlerMethodPair pair in listener.GetEventMethods(e))
                {
                    bool added = false;
                    int pairPriority = Util.GetPriorityID(pair.getAnnotation().Priority);
                    int originalSize = callList.Count;

                    for (int i = 0; i < originalSize; i++)
                    {
                        HandlerMethodPair p = callList[i];

                        if (pairPriority > Util.GetPriorityID(p.getAnnotation().Priority))
                        {
                            callList.Insert(i, pair);
                            callListeners.Insert(i, listener);
                            added = true;
                            break;
                        }
                    }

                    if (!added)
                    {
                        callList.Add(pair);
                        callListeners.Add(listener);
                    }
                }
            }

            // Separating them to save a tiny bit more time on each iteration.
            if (e is ICancellable cancellable)
            {
                for (int i = 0; i < callList.Count; i++)
                {
                    HandlerMethodPair methodPair = callList[i];
                    Listener sourceListener = callListeners[i];

                    // Skip if cancelled and ignoring cancelled.
                    if (cancellable.IsCancelled() && methodPair.getAnnotation().IgnoreIfCancelled) continue;
                    InvokeEvent(sourceListener, e, methodPair);
                }
            }
            else
            {
                for (int i = 0; i < callList.Count; i++)
                {
                    HandlerMethodPair methodPair = callList[i];
                    Listener sourceListener = callListeners[i];

                    InvokeEvent(sourceListener, e, methodPair);
                }
            }
        }


        public Listener AddListener(Listener listener)
        {
            RemoveListener(listener, true);
            // Check that it isn't duped by clearing it.
            // If someone has used the same object instance to create two objects and overrided
            // #equals(), that's their problem.
            Listeners.Add(listener);
            return listener;
        }

        public void RemoveListener(Listener listener)
        {
            RemoveListener(listener, true);
        }

        public void RemoveListener(Listener listener, bool removeFromChildren)
        {
            Listeners.Remove(listener);

            if (removeFromChildren)
            {
                foreach (EventManager child in Children)
                {
                    child.RemoveListener(listener, true); // Ensure children don't include it either.
                }
            }
        }


        private static void InvokeEvent(Listener owningListener, BaseEvent e, HandlerMethodPair methodPair)
        {
            try
            {
                methodPair.getMethod().Invoke(owningListener.sourceObject, new object[] { e });
            }
            catch (Exception err)
            {
                Debug.LogError("An error was thrown during the invocation of an event:");
                Debug.LogException(err);
            }
        }


        /** @return the primary instance of the EventManager. */
        public static EventManager Get()
        {
            return _primaryManager;
        }

        public IEventFilter[] GetFilters()
        {
            return Filters.ToArray();
        }

        public Listener[] GetListeners()
        {
            return Listeners.ToArray();
        }

        public EventManager[] GetChildren()
        {
            return Children.ToArray();
        }
    }
}