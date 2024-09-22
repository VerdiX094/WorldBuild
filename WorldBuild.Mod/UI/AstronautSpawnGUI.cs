using SFS.UI.ModGUI;
using SFS.World;
using System;
using UITools;
using WorldBuild.Mod.Managers;
using WorldBuild.Mod.Modules;

namespace WorldBuild.Mod.UI
{
    public class AstronautSpawnGUI : GUIBase
    {
        public override string SceneToAttach => "World_PC";

        public override Func<bool> GOActiveCondition => () => 
        { 
            return AstronautSpawner.main.CanSpawnEVA(); 
        };

        public override void Begin()
        {
            PlayerController.main.player.OnChange += OnPlayerChange;
            CapsuleScanner.main.selectedCapsule.ValueChanged += (CapsuleScanner.BestCapsuleData o, CapsuleScanner.BestCapsuleData n) =>
            {
                NewGUI();
            };
        }

        public override void Update()
        {
            if (!elements.ContainsKey("oxygenAvail")) return;

            var plr = PlayerController.main.player.Value;
            if (plr == null) return;

            var rox = plr.GetComponent<RocketOxygen>();
            if (rox == null) return;

            double timeLeft = rox.CalculateOxygenAvailable();

            (elements["oxygenAvail"] as Label).Text = $"Available oxygen: {Utility.StringifyTime(timeLeft)}";
        }

        private void OnPlayerChange(Player oldP, Player newP)
        {
            NewGUI();
        }

        public override void GenerateGUI()
        {
            window = UIToolsBuilder.CreateClosableWindow(holder.transform, Builder.GetRandomID(), 384, 256, 300, 300, true, true, 0.95f, "Astronaut Manager");
            VerticalDefGroup();

            if (CapsuleScanner.main.selectedCapsule.Value.cm == null)
            {
                elements.Add("selectNote", Builder.CreateLabel(window, 352, 32, text: "Select a capsule first! (click one with RMB)"));
                window.Size = new UnityEngine.Vector2(window.Size.x, 128);
                return;
            }

            elements.Add("oxygenAvail", Builder.CreateLabel(
                window, 352, 32, text: "Available oxygen: [not calculated yet]"
            ));

            elements.Add("spawnBtn", Builder.CreateButton(window, 352, 45, onClick: () =>
            {
                AstronautSpawner.main.StartEVA();
            }, text: "Start EVA"));
        }
    }
}
