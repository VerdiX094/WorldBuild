using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldBuild.Mod.UI
{
    public class AstronautManagementGUI : GUIBase
    {
        public override string SceneToAttach => "World_PC";

        public override Func<bool> GOActiveCondition => 
            () => {
                if (PlayerController.main != null)
                    return PlayerController.main.player.Value is Astronaut_EVA;
                return false;
            };

        public static AstronautManagementGUI main;

        public AstronautManagementGUI()
        {
            main = this;
        }

        public override void GenerateGUI() 
        {
            window = Builder.CreateWindow(holder.transform, Builder.GetRandomID(), 384, 256, 300, 300, true, true, 0.95f, "Astronaut");
            VerticalDefGroup();

            elements.Add(
                "plantFlag",
                Builder.CreateButton(window, 352, 48, text: "Plant Flag", onClick: () =>
                {
                    AstronautManager.main.PlantFlag();
                }
            ));

            elements.Add(
                "endEVA",
                Builder.CreateButton(window, 352, 48, text: "End EVA", onClick: () =>
                {
                    double distanceThreshold = 50f;

                    Rocket bestRocket = null;
                    double bestDistance = double.MaxValue;

                    foreach (Rocket rocket in GameManager.main.rockets)
                    {
                        if (!rocket.partHolder.parts.Any(part => part.Name == "Capsule")) continue;

                        double distance = (rocket.location.position.Value - PlayerController.main.player.Value.location.position.Value).magnitude;

                        if (distance > distanceThreshold) continue;
                        
                        if (distance > bestDistance) continue;
                        
                        bestRocket = rocket;
                        bestDistance = distance;
                    }
                    
                    if (bestRocket == null)
                    {
                        MsgDrawer.main.Log("No rocket with a capsule nearby!");
                        return;
                    }

                    AstronautSpawner.main.EndEVA(bestRocket);
                }
            ));

            #region Oxygen Bar

            //elements.Add(
            //    "oxygenBarContainer",W
            //    Builder.CreateContainer(window)
            //);

            //(elements["oxygenBarContainer"] as Container).CreateLayoutGroup(
            //    SFS.UI.ModGUI.Type.Horizontal, 
            //    padding: new RectOffset(16, 16, 0, 0));

            //elements.Add(
            //    "oxygenBarLabel",
            //    Builder.CreateLabel(elements["oxygenBarContainer"], 128, 48, text: "O2 left:")
            //);

            //elements.Add(
            //    "oxygenBarSlider",
            //    Builder.CreateSlider(elements["oxygenBarContainer"], 204, 100, (0, 100), 
            //    getValueWithUnits: (float percent) =>
            //    {
            //        return "";
            //    })
            //);

            //(elements["oxygenBarSlider"] as Slider)
            //    .gameObject.GetComponentsInChildren<RectTransform>()
            //    .Where(transform => transform.gameObject.name == "Handle").First().gameObject.SetActive(false);

            #endregion

            elements.Add("oxygenLeftApprox", Builder.CreateLabel(window, 352, 32, text: "Oxygen left: 0m 0s"));
        }
    }
}
