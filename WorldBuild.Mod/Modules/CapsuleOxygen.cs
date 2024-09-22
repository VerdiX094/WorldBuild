using SFS.Variables;
using SFS.World;

namespace WorldBuild.Mod.Modules
{
    public class CapsuleOxygen : InjectEverywhereWith<CrewModule>
    {
        private VariablesModule varMod;

        public static double MaxOxygen => 1000;

        public double Oxygen
        {
            get => varMod.doubleVariables.GetValue("oxygen");
            set => varMod.doubleVariables.SetValue("oxygen", value, (true, true));
        }

        private void Awake()
        {
            varMod = GetComponent<VariablesModule>();

            if (Utility.CheckSceneLoaded("Build_PC") && !varMod.boolVariables.GetValue("oxygenInitialized"))
            {
                Oxygen = MaxOxygen;
                varMod.boolVariables.SetValue("oxygenInitialized", true);
            }
        }
    }
}
