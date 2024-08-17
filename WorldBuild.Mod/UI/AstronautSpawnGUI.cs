using SFS.UI.ModGUI;
using SFS.World;
using System;
using System.Collections.Generic;
using SFS.Variables;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

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
        }

        private void OnPlayerChange(Player oldP, Player newP)
        {
            NewGUI();
        }

        public override void GenerateGUI()
        {
            window = Builder.CreateWindow(holder.transform, Builder.GetRandomID(), 256, 96, 300, 300, true, true, 0.95f, "Astronaut Manager");
            window.CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical);

            elements.Add("spawnBtn", Builder.CreateButton(window, 224, 32, onClick: () =>
            {
                AstronautSpawner.main.StartEVA();
            }, text: "Start EVA"));
        }
    }
}
