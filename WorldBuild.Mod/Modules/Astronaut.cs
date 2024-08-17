using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldBuild.Mod.Modules
{
    public class Astronaut : MonoBehaviour
    {
        private Astronaut_EVA eva;


        private void Awake()
        {
            eva = GetComponent<Astronaut_EVA>();
        }
        private void Update()
        {
            if (eva.CanPickItselfUp && eva.ragdollTime > 2)
            {
                eva.SetRagdoll(false);
            }
        }
    }
}
