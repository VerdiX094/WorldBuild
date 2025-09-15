using System.Linq;
using SFS;
using SFS.Cameras;
using SFS.Parts;
using SFS.World;
using SFS.World.Maps;
using UnityEngine;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod.UI
{
    public class CapsuleTooltip : WorldManager<CapsuleTooltip>
    {
        private Vector3 mouseWorld;

        private readonly string[] tooltip =
        {
            $"{Keybindings.main.StartEVA.key.ToString()} to start EVA",
        };

        public Part hovered;
        
        void OnGUI()
        {
            if (Map.manager.mapMode.Value) return;
            
            mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (!(PlayerController.main.player.Value is Rocket rocket)) return;

            foreach (var part in rocket.partHolder.parts)
            {
                if (part.name == "Capsule")
                {
                    if ((part.transform.position - mouseWorld).magnitude < 1f *
                        Mathf.Max(part.orientation.orientation.Value.x, part.orientation.orientation.Value.y))
                    {
                        Vector3 coord = Input.mousePosition - new Vector3(0f, -16f);
                        GUI.Label(new Rect(coord.x, coord.y, 480f, 1600f), string.Join("\n", tooltip));
                        hovered = part;
                    }
                }
            }
        }
    }
}