using System.Reflection;

namespace me.cg360.spookums.core.eventsys.handler
{
    public class HandlerMethodPair
    {
        private GEventHandler annotation;
        private MethodInfo method;

        public HandlerMethodPair(GEventHandler annotation, MethodInfo method) {
            this.annotation = annotation;
            this.method = method;
        }

        public GEventHandler getAnnotation() { return annotation; }
        public MethodInfo getMethod() { return method; }
    }
}