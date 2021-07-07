using System;

namespace me.cg360.spookums.utility
{
    public class Constants
    {
        public const byte VERSION_MAJOR = 0, VERSION_MINOR = 1, VERSION_PATCH = 0;
        public const VersionBranch VERSION_BRANCH = VersionBranch.DEV;
        
        public static readonly string VERSION_STRING = $"{VERSION_MAJOR}.{VERSION_MINOR}.{VERSION_PATCH}-{VERSION_BRANCH}";

        
        
        public enum VersionBranch
        {
            DEV, BETA, RELEASE
        }
    }
}