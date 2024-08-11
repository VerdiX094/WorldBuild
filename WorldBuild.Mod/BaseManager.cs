using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldBuild.Mod
{
    public class BaseManager : MonoBehaviour {
        public static BaseManager firstInstance;
        private bool passed;

        private void Awake()
        {
            if (firstInstance == null)
                firstInstance = this;
        }

        private void Update()
        {
            if (passed) return;
            if (firstInstance != this) return;
            
            passed = true;
            if (!Utility.CheckPackLoaded())
            {
                Debug.Log("Pack not loaded, destroying managers holder!");
                Destroy(Entrypoint.BaseGO);
                return;
            }
        }
    }
}
