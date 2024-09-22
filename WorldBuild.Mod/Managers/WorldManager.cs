using UnityEngine;

namespace WorldBuild.Mod.Managers
{
    public class WorldManager<T> : Manager<T>
        where T : WorldManager<T>
    {
        public static new string[] ScenesToAttach => new string[] { "World_PC" };
    }
}
