using WorldBuild.Mod.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS.UI.ModGUI;
using UnityEngine.UI;
using SFS.Parts.Modules;

namespace WorldBuild.Mod.UI
{
    public class PartControlsGUI : GUIBase
    {
        public override Func<bool> GOActiveCondition => () => BuildManager.main.worldBuildActive && BuildManager.main.heldPart != null;
        public override string SceneToAttach => "World_PC";

        public const int width = 540;
        public const int height = 480;

        public override void GenerateGUI()
        {
            var coords = WindowPositionHelper.GenerateWindowCoords(-16, 16, width, height, Anchor.BottomRight, Origin.BottomRight);
            window = Builder.CreateWindow(holder.transform, WindowID, width, height, coords.x, coords.y, opacity: 0.5f, titleText: "Selected Part");
            VerticalDefGroup();
            window.EnableScrolling(SFS.UI.ModGUI.Type.Vertical);

            elements["actionsHolder"] = Builder.CreateContainer(window);
            elements["actionsHolder"].As<Container>().CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, spacing: 8);
            elements["placeBtn"] = Builder.CreateButton(elements["actionsHolder"], width / 2 - 16, 45, onClick: () => BuildManager.main.TryBuildPart(), text: "Place");
            elements["destroyBtn"] = Builder.CreateButton(elements["actionsHolder"], width / 2 - 16, 45, onClick: () => BuildManager.main.DestroyHeldPart(), text: "Delete");

            elements["transformHld"] = Builder.CreateContainer(window);
            elements["transformHld"].As<Container>().CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, spacing: 8);

            elements["flipHoriz"] = Builder.CreateButton(elements["transformHld"], width / 4 - 12, 45, onClick: () => {
                var val = BuildManager.main.heldPart.orientation.orientation.Value;
                BuildManager.main.heldPart.orientation.orientation.Value = new SFS.Parts.Modules.Orientation(val.x * -1, val.y, val.z);
                BuildManager.main.heldPart.RegenerateMesh();
            }, text: "Horiz");
            elements["flipVert"] = Builder.CreateButton(elements["transformHld"], width / 4 - 12, 45, onClick: () => {
                var val = BuildManager.main.heldPart.orientation.orientation.Value;
                BuildManager.main.heldPart.orientation.orientation.Value = new SFS.Parts.Modules.Orientation(val.x, val.y * -1, val.z);
                BuildManager.main.heldPart.RegenerateMesh();
            }, text: "Vert");

            elements["rotLeft"] = Builder.CreateButton(elements["transformHld"], width / 4 - 12, 45, onClick: () => {
                var val = BuildManager.main.heldPart.orientation.orientation.Value;
                BuildManager.main.heldPart.orientation.orientation.Value = new SFS.Parts.Modules.Orientation(val.x, val.y, val.z + 90);
                BuildManager.main.heldPart.RegenerateMesh();
            }, text: "Left");
            elements["rotRight"] = Builder.CreateButton(elements["transformHld"], width / 4 - 12, 45, onClick: () => {
                var val = BuildManager.main.heldPart.orientation.orientation.Value;
                BuildManager.main.heldPart.orientation.orientation.Value = new SFS.Parts.Modules.Orientation(val.x, val.y, val.z - 90);
                BuildManager.main.heldPart.RegenerateMesh();
            }, text: "Right");


            elements["openEditor"] = Builder.CreateButton(window, width - 24, 45, onClick: () => GUIManager.main.GetUI<VariableEditorGUI>().OpenEditor(), text: "Edit values");

            elements["sep"] = Builder.CreateSeparator(window, width - 24);

            var part = BuildManager.main.heldPart;

            elements["info"] = Builder.CreateLabel(window, width - 24, 0, text: $"--- Part Info ---\nName: {part.displayName.Field.subs[0]}\nMass: {part.mass.Value}t\n--- Stats ---\n{Utility.GetStats(part)}");

            elements["info"].As<Label>().AutoFontResize = false;
            elements["info"].As<Label>().FontSize = 32;
            elements["info"].As<Label>().gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        }
    }
}
