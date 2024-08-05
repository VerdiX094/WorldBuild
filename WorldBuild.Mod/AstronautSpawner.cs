using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod
{
    public class AstronautSpawner : BaseManager
    {
        private bool enab;

        public GameObject guiHolder;

        private void Start()
        {
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                enab = CheckWorld();

                if (!enab) return;


            };
        }

        private void Update()
        {
            if (!enab) return;


        }

        private bool CheckWorld()
        {
            return SceneManager.GetAllScenes().Any(scene => scene == SceneManager.GetSceneByName("World_PC"));
        }
    }
}
