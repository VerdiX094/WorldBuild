using SFS.Variables;
using SFS.World;

namespace WorldBuild.Mod.Modules
{
    public class CapsuleResources : InjectEverywhereWith<CrewModule>
    {
        private VariablesModule varMod;

        public static double MaxOxygen => 1000;
        public static double MaxEVARes => 50;

        public double Oxygen
        {
            get => varMod.doubleVariables.GetValue("oxygen");
            set => varMod.doubleVariables.SetValue("oxygen", value, (true, true));
        }
        
        public double EVARes
        {
            get => varMod.doubleVariables.GetValue("evares");
            set => varMod.doubleVariables.SetValue("evares", value, (true, true));
        }

        private void Awake()
        {
            varMod = GetComponent<VariablesModule>();

            if (!varMod.boolVariables.GetValue("oxygenInitialized"))
            {
                Oxygen = MaxOxygen;
                varMod.boolVariables.SetValue("oxygenInitialized", true);
            }
            if (!varMod.boolVariables.GetValue("evaresInitialized"))
            {
                EVARes = MaxEVARes;
                varMod.boolVariables.SetValue("evaresInitialized", true);
            }
        }
    }
}
