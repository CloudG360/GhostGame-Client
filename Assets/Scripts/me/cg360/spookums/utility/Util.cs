using me.cg360.spookums.core.eventsys.handler;
using System;

namespace me.cg360.spookums.utility
{
    public class Util
    {

        public static string GenTypeName(Type type)
        {
            string name = "";
            Type process = type;
            while(true)
            {
                string tempName = process.Name;
                int genericChar = tempName.IndexOf("`");
                if(genericChar != -1)
                {
                    tempName = tempName.Substring(0, genericChar); // remove the weird " ` " from the end of generic types.
                }

                // if name isn't 0 length, add a "."
                name += ((name.Length > 0 ? "." : "") + tempName);

                if(process == typeof(Object))
                {
                    break;
                }
            }

            return name;
        }

        public static int GetPriorityID(Priority priority)
        {
            switch (priority)
            {
                case Priority.LOWEST:
                    return 3;
                case Priority.LOWER:
                    return 2;
                case Priority.LOW:
                    return 1;
                case Priority.NORMAL:
                    return 0;
                case Priority.HIGH:
                    return -1;
                case Priority.HIGHER:
                    return -2;
                case Priority.HIGHEST:
                    return -3;
            }

            return 0; // Unsupported? Go with 0.
        }
        
    }
}