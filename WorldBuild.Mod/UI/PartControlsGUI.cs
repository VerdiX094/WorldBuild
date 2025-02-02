using WorldBuild.Mod.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS.UI.ModGUI;

namespace WorldBuild.Mod.UI
{
    public class PartControlsGUI : GUIBase
    {
        public override Func<bool> GOActiveCondition => () => BuildManager.main.worldBuildActive;
        public override string SceneToAttach => "World_PC";

        public Window win;

        public override void GenerateGUI()
        {
            int width = 280;
            int height = 360;
            var coords = Utility.GenerateWindowCoords(-16, 16, width, height, Anchor.BottomRight, Origin.BottomRight);
            win = Builder.CreateWindow(holder.transform, WindowID, width, height, coords.x, coords.y, opacity: 0.5f, titleText: "Selected Part");
        }
    }
}
