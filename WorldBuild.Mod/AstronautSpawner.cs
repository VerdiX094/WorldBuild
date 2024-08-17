using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WorldBuild.Mod
{
    public class AstronautSpawner : BaseManager<AstronautSpawner>
    {
        private bool enab;

        private Rocket lastRocket;

        public bool isRocket
        {
            get
            {
                if (!enab) return false;

                return PlayerController.main.player.Value is Rocket;
            }
        }

        public bool CanSpawnEVA()
        {
            if (!(PlayerController.main.player.Value is Rocket rocket))
                return false;

            return rocket.partHolder.parts.Any(part => part.Name == "Capsule");
        }

        public void EndEVAAndReturnToRocket()
        {
            if (!lastRocket || (!(PlayerController.main.player.Value is Astronaut_EVA eva))) return;

            PlayerController.main.player.Value = lastRocket;

            AstronautManager.DestroyEVA(eva, false);
        }


        private void Start()
        {
            SceneManager.sceneLoaded += (Scene s, LoadSceneMode lsm) =>
            {
                enab = CheckWorld();

                if (!enab) return;

                // lazy :3
                // fuck the fact that it will run as many checks as the number of times World was loaded during that game run

                PlayerController.main.player.OnChange += (o, n) =>
                {
                    if (n is Rocket rocket)
                        lastRocket = rocket;
                };
            };
        }

        private void Update()
        {
            if (!enab) return;


        }

        public void StartEVA()
        {
            AstronautManager manager = GameObject.Find("Astronaut Manager").GetComponent<AstronautManager>();

            var eva = manager.SpawnEVA("WorldBuild EVA", PlayerController.main.player.Value.location.Value, PlayerController.main.player.Value.transform.rotation.z, 0, false, 1, 0);

            PlayerController.main.SmoothChangePlayer(eva);
        }

        private bool CheckWorld()
        {
            return SceneManager.GetAllScenes().Any(scene => scene == SceneManager.GetSceneByName("World_PC"));
        }
    }
}
