using SFS.Parts;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorldBuild.Mod.Build;
using WorldBuild.Mod.Managers;
using WorldBuild.Mod.Modules;

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

        public override void Update()
        {

            if (!(PlayerController.main.player.Value is Astronaut_EVA eva) || Elements["oxygenLeftApprox"] == null) return;

            if (eva.GetComponent<Astronaut>() == null) return;

            (Elements["oxygenLeftApprox"] as Label).Text = $"Oxygen left: {Utility.StringifyTime(eva.GetComponent<Astronaut>().GetOxygenSecondsLeft())}";
        }

        public override void GenerateGUI() 
        {
            window = Builder.CreateWindow(holder.transform, WindowID, 448, 280 + (BuildManager.main.worldBuildActive ? 56 : 0), 300, 300, true, true, 0.95f, "Astronaut");
            HorizontalDefGroup();

            elements.Add("main", Builder.CreateContainer(window));
            (elements["main"] as Container).CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical, childAlignment: TextAnchor.UpperCenter, spacing: 8, padding: new RectOffset(0, 0, 8, 0));

            elements.Add("keybindsHints", Builder.CreateContainer(window));
            (elements["keybindsHints"] as Container).CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical, childAlignment: TextAnchor.UpperCenter, spacing: 8, padding: new RectOffset(0, 0, 8, 0));

            #region Main
            var main = elements["main"] as Container;

            elements.Add(
                "plantFlag",
                Builder.CreateButton(main, 352, 48, text: "Plant Flag", onClick: () =>
                {
                    AstronautManager.main.PlantFlag();
                }
            ));

            elements.Add(
                "endEVA",
                Builder.CreateButton(main, 352, 48, text: "End EVA", onClick: () =>
                {
                    CapsuleScanner.BestCapsuleData best = new CapsuleScanner.BestCapsuleData();

                    Vector2 pos = WorldView.ToLocalPosition(PlayerController.main.player.Value.location.position);

                    foreach (Rocket rocket in GameManager.main.rockets)
                    {
                        var data = CapsuleScanner.main.FindBest(rocket, pos, 3f);

                        if (data.GetDistanceTo(pos) < best.GetDistanceTo(pos)) best.cm = data.cm;
                    }
                    
                    if (best.cm == null)
                    {
                        MsgDrawer.main.Log("No capsule nearby!");
                        return;
                    }

                    CapsuleScanner.main.selectedCapsule.Value = best;

                    AstronautSpawner.main.EndEVA(best.cm.Rocket);
                }
            ));

            elements.Add(
                "enterBuild",
                Builder.CreateButton(main, 352, 48, text: BuildManager.main.worldBuildActive ? "Exit Build" : "Enter Build", onClick: () =>
                {
                    if (BuildManager.main.worldBuildActive)
                        BuildManager.main.ExitBuild();
                    else
                        BuildManager.main.EnterBuild();

                    NewGUI();
                }
            ));

            if (BuildManager.main.worldBuildActive)
                elements.Add(
                    "placePart",
                    Builder.CreateButton(main, 352, 48, text: "Place Part", onClick: () =>
                    {
                        BuildManager.main.TryBuildPart();
                    }
                ));

            elements.Add("oxygenLeftApprox", Builder.CreateLabel(main, 352, 32, text: "Oxygen left: 0m 0s"));
            #endregion
            #region Keybind Hints

            var kh = elements["keybindsHints"] as Container;

            elements.Add("plantFlagKB",
                Builder.CreateLabel(kh, 56, 48, text: "F")
            );

            elements.Add("endEVAKB",
                Builder.CreateLabel(kh, 56, 48, text: "Del")
            );

            elements.Add("switchBuildKB",
                Builder.CreateLabel(kh, 56, 48, text: "B")
            );

            elements.Add("placePartKB",
                Builder.CreateLabel(kh, 56, 48, text: "P")
            );

            #endregion
        }
    }
}
