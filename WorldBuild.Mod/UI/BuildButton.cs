using System.Linq;
using SFS.UI;
using SFS.World;
using UnityEngine;
using WorldBuild.Mod.Managers;
using UnityEngine.UI;
using WorldBuild.Mod.Build;

namespace WorldBuild.Mod.UI
{
    public class BuildButton : WorldManager<BuildButton>
    {
        private GameObject helpBtn;
        private ButtonPC bpc;
        private Sprite buildIcon;

        private void Start()
        {
            BuildManager.main.ExitBuild();
            
            var panel = GameObject.Find("Top Left Panel");
            foreach (TextAdapter text in panel.GetComponentsInChildren<TextAdapter>())
            {
                if (text.Text == "Help")
                {
                    helpBtn = text.transform.parent.gameObject;
                }
            }
            
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(ResourceFile.Hammer);
            
            buildIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
            helpBtn.GetComponentInChildren<TextAdapter>().Text = "Build";
            helpBtn.GetComponentsInChildren<Image>().First(i => i.gameObject.name == "Icon").sprite = buildIcon;
            
            var buttonPC = helpBtn.GetComponent<ButtonPC>();
            buttonPC.onClick.Clear();
            buttonPC.onClick += BuildManager.main.ToggleBuild;
            
            bpc = buttonPC;
        }
        
        private void Update()
        {
            helpBtn.SetActive(PlayerController.main.player.Value is Astronaut_EVA);
            bpc.SetSelected(BuildManager.main.worldBuildActive);
        }
    }
}