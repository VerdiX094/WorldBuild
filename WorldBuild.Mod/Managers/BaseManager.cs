using UnityEngine;

namespace WorldBuild.Mod.Managers
{
    public class BaseManager<T> : Manager<T>
        where T : BaseManager<T>
    {
        public static new string[] ScenesToAttach => new string[] { "Base_PC" };
    }
}
