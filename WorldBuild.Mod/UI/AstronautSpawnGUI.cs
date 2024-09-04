using SFS.UI.ModGUI;
using SFS.World;
using System;
using System.Collections.Generic;
using SFS.Variables;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UITools;

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
                window, 352, 32, text: "Available oxygen: 0m 0s"
            ));

            elements.Add("spawnBtn", Builder.CreateButton(window, 352, 45, onClick: () =>
            {
                AstronautSpawner.main.StartEVA();
            }, text: "Start EVA"));
        }
    }
}
