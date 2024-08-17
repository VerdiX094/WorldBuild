using SFS.UI.ModGUI;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void GenerateGUI() 
        {
            window = Builder.CreateWindow(holder.transform, Builder.GetRandomID(), 256, 96, 300, 300, true, true, 0.95f, "Astronaut");

        }
    }
}
