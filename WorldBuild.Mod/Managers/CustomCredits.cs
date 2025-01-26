using UnityEngine;
using UnityEngine.UI;

namespace WorldBuild.Mod.Managers
{
    public class CustomCredits : BaseManager<CustomCredits>
    {
        public string[] lines =
        {
            "\n\n<size=80>--- WorldBuild developers ---</size>",
            "",
            "<size=70>Heroix</size>",
            "<size=55>Project coordinator</size>",
            "",
            "<size=70>Dahzito</size>",
            "<size=55>Part pack developer</size>",
            "",
            "<size=70>Astro The Rabbit/pixelgaming579</size>",
            "<size=55>Programmer (World build)</size>",
            "",
            "<size=70>VerdiX094/N2O4</size>",
            "<size=55>Programmer (Astronauts)</size>",
        };

        public void Update()
        {
            GameObject go = GameObject.Find("Read Menu");

            if (!go) return;

            Text text = go.GetComponentInChildren<Text>();

            if (!text) return;

            if (text.text.Contains(lines[0]) || !text.text.Contains("Designer - Programmer - Artist")) return;

            text.text = string.Concat(text.text, string.Join("\n", lines));
        }
    }
}
