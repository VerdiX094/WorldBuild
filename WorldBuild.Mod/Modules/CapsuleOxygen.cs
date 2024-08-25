using Cysharp.Threading.Tasks.Triggers;
using SFS.Parts;
using SFS.Sharing;
using SFS.Variables;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod.Modules
{
    public class CapsuleOxygen : InjectEverywhereWith<CrewModule>
    {
        private VariablesModule varMod;

        public static double MaxOxygen => 1000;

        public double Oxygen
        {
            get => varMod.doubleVariables.GetValue("oxygen");
            set => varMod.doubleVariables.SetValue("oxygen", value, (true, true));
        }

        private void Awake()
        {
            varMod = GetComponent<VariablesModule>();

            if (SceneManager.GetActiveScene().name == "Build_PC")
            {

            }
        }
    }
}
