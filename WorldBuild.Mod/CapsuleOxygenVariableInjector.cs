using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using SFS;
using SFS.Parts;
using SFS.Variables;
using SFS.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Mod.Modules;

namespace WorldBuild.Mod
{
    public class CapsuleOxygenVariableInjector : BaseManager<CapsuleOxygenVariableInjector>
    {
        private void Start()
        {
            var capsules = Base.partsLoader.parts.Values
                .Where(part => part.GetComponent<CrewModule>() != null);

            foreach (Part capsule in capsules)
            {
                Debug.Log("chuj kurwa");
                var varMod = capsule.GetComponent<VariablesModule>();

                varMod.doubleVariables.SetValue("oxygen", CapsuleOxygen.MaxOxygen, (true, true));

                // the workaround for the variables bug
                varMod.boolVariables.SetValue("oxygenInitialized", false, (true, true));
            }
        }
    }
}
