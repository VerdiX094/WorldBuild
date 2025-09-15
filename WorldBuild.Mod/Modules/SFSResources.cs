using System.Collections.Generic;
using SFS.Parts.Modules;

namespace WorldBuild.Mod.Modules
{
    public static class SFSResources
    {
        public static HashSet<ResourceType> resourceTypes = new HashSet<ResourceType>();

        public static ResourceType GetByName(string name)
        {
            foreach (ResourceType resourceType in resourceTypes)
            {
                if (resourceType.displayName.Field == name)
                {
                    return resourceType;
                }
            }
            
            return null;
        }
    }
}