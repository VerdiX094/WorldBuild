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
            }

            // workaround for pickgrid parts holder messing this shit up
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                UniTask.RunOnThreadPool(async () =>
                {
                    await UniTask.Yield();
                    await UniTask.Yield();
                    await UniTask.Yield();
                    await UniTask.Yield();
                    await UniTask.Yield();
                    await UniTask.Yield();

                    if (s.name != "Build_PC") return;

                    Debug.Log("pain");

                    var pgph = GameObject.Find("PickGrid Parts Holder");

                    for (int i = 0; i < pgph.transform.childCount; i++)
                    {
                        Debug.Log(i);
                        var cm = pgph.transform.GetChild(i).GetComponent<CrewModule>();

                        if (cm == null) return;

                        Debug.Log("oxygen set");
                        cm.GetComponent<VariablesModule>().doubleVariables.SetValue("oxygen", CapsuleOxygen.MaxOxygen, (true, true));
                    }
                });
            };
        }
    }
}
