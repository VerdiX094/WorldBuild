using SFS.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod
{
    public class FuckingDebugCodeFFS : BaseManager<FuckingDebugCodeFFS>
    {
        private void Awake()
        {
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                if (s.name != "Build_PC") return;

                foreach (GameObject go in s.GetRootGameObjects())
                {
                    foreach (VariablesModule vm in go.GetComponentsInChildren<VariablesModule>())
                    {
                        foreach (VariableSave vs in vm.doubleVariables.saves)
                        {
                            var variable = (VariableList<double>.Variable)vs.runtimeVariable;
                            
                            variable.onValueChangeOldNew += (double o, double n) =>
                            {
                                change(vs.name, o, n);
                            };
                        }
                    }
                }
            };
        }

        private void change(string name, double o, double n)
        {
            Debug.Log($"{name}: {o} chuj {n}");
        }
    }
}
