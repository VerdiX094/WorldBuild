using UnityEngine;
using UnityEngine.UI;

namespace WorldBuild.Mod.Managers
{
    public class CustomCredits : BaseManager<CustomCredits>
    {
        public string[] lines =
        {
            "\n\n<size=90>--- WorldBuild developers ---</size>",
            "",
            "<size=70>Heroix</size>",
            "<size=55>Project manager/coordinator</size>",
            "",
            "<size=70>Dahzito</size>",
            "<size=55>Part pack developer</size>",
            "",
            "<size=70>VerdiX094/N2O4</size>",
            "<size=55>Lead programmer</size>",
            "",
            "<size=70>Astro The Rabbit</size>",
            "<size=55>Programmer</size>",
            "",
            "<size=70>Cratior</size>",
            "<size=55>Programmer</size>",
            "",
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
