using System;
using System.Collections.Generic;
using System.Reflection;
using me.cg360.spookums.core.eventsys.handler;

namespace me.cg360.spookums.core.eventsys
{
    public class Listener
    {
        // Event Class Name -> Method+Annotation list
        private Dictionary<Type, List<HandlerMethodPair>> listenerMethods;
        public Object sourceObject { get; }

        public Listener(Object sourceObject)
        {
            this.listenerMethods = new Dictionary<Type, List<HandlerMethodPair>>();
            this.sourceObject = sourceObject;

            // Get event listening methods.
            foreach (MethodInfo method in this.sourceObject.GetType().GetMethods())
            {
                GEventHandler[] attributes = (GEventHandler[]) method.GetCustomAttributes(typeof(GEventHandler), true);

                if (attributes.Length > 0)
                {
                    GEventHandler attrib = attributes[0];
                    ParameterInfo[] parameters = method.GetParameters();

                    if (parameters.Length == 1)
                    {
                        Type type = parameters[0].ParameterType;
                        List<Type> eventTypes = new List<Type>();
                        

                        HandlerMethodPair pair = new HandlerMethodPair(attrib, method);
                        CheckAndAdd(type, eventTypes); // Get all the categories this method would be in.

                        foreach (Type cls in eventTypes)
                        {
                            if (!listenerMethods.ContainsKey(cls))
                            {
                                listenerMethods.Add(cls, new List<HandlerMethodPair>()); // Create new handler list if it doesn't exist.
                            }

                            listenerMethods[cls].Add(pair);
                        }
                    }
                }
            }
        }

        /**
         * Calls EventManager's #addListener() method.
         * @param manager the manager to add this listener to.
         */
        public void RegisterListener(EventManager manager)
        {
            manager.AddListener(this);
        }

        /**
         * Calls EventManager's #removeListener() method.
         * @param manager the manager to remove this listener from.
         */
        public void UnregisterListener(EventManager manager)
        {
            UnregisterListener(manager, true);
        }

        /**
         * Calls EventManager's #removeListener() method.
         * @param manager the manager to remove this listener from.
         * @param removeFromChildren should child managers have this listener removed?
         */
        public void UnregisterListener(EventManager manager, bool removeFromChildren)
        {
            manager.RemoveListener(this, removeFromChildren);
        }

        private static void CheckAndAdd(Type typeIn, List<Type> typeList)
        {
            if (typeIn == null) return;

            if (typeof(BaseEvent).IsAssignableFrom(typeIn))
            {
                typeList.Add(typeIn);
                CheckAndAdd(typeIn.BaseType, typeList);

                foreach (Type cls in typeIn.GetInterfaces())
                {
                    CheckAndAdd(cls, typeList);
                }
            }
        }

        public virtual List<HandlerMethodPair> GetEventMethods(BaseEvent e)
        {
            return listenerMethods.ContainsKey(e.GetType())
                ? new List<HandlerMethodPair>(listenerMethods[e.GetType()])
                : new List<HandlerMethodPair>(); // Empty list if no methods exist. Otherwise clone.
        }

        public override int GetHashCode()
        {
            // Should be based off the object but not the same.
            return sourceObject.GetHashCode() * 2; 
        }

        public override bool Equals(Object o)
        {
            if (o is Listener)
            {
                Listener oListener = (Listener) o;

                // Compare the source objects instead.
                return oListener.sourceObject.Equals(sourceObject);
            }

            return false;
        }
    }
}