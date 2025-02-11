using SFS.UI.ModGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod.UI
{
    public class PerformanceTestingGUI : GUIBase
    {
        public override Func<bool> GOActiveCondition => () => Debugger.IsDebugEnabled;
        public override string SceneToAttach => "World_PC";

        public override void Update()
        {
            var lab = elements["info"].As<Label>();
            lab.Text = $"Frame: {(Time.unscaledDeltaTime * 1000).Round(2)}ms ({(1 / Time.unscaledDeltaTime).Round(0)} fps)\n";
            lab.Text += $"Physics: {(Time.fixedUnscaledDeltaTime * 1000).Round(2)}ms ({(1 / Time.fixedUnscaledDeltaTime).Round(0)} pfps)";
        }

        public override void LateUpdate()
        {
            var lab = elements["info"].As<Label>();

            StringBuilder text = new StringBuilder();

            foreach (var time in DebugPatch.times.OrderBy(kvp => kvp.Key))
            {
                text.Append("\n" + time.Key + " -> " + (time.Value * 1000).Round(3).ToString() + "ms");
            }
            lab.Text += text.ToString();

            DebugPatch.times.Clear();
        }

        public override void GenerateGUI()
        {
            var coords = WindowPositionHelper.GenerateWindowCoords(640, -16, 480, 720, Anchor.TopLeft, Origin.TopLeft);
            window = Builder.CreateWindow(holder.transform, WindowID, 480, 720, coords.x, coords.y, opacity: 0.95f, titleText: "Testing");
            window.EnableScrolling(SFS.UI.ModGUI.Type.Vertical);
            VerticalDefGroup();

            elements["info"] = Builder.CreateLabel(window, 480, 0);

            elements["info"].As<Label>().AutoFontResize = false;
            elements["info"].As<Label>().FontSize = 16;
            elements["info"].As<Label>().gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
