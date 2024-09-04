using SFS.Career;
using SFS.World;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Mod.Modules;
using WorldBuild.Mod.UI;
using static SFS.World.WorldSave.Astronauts;

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

        public void EndEVAAndReturnToRocket(bool death = false)
        {
            if (!lastRocket || (!(PlayerController.main.player.Value is Astronaut_EVA eva))) return;

            AstronautManager.DestroyEVA(eva, death);

            PlayerController.main.SmoothChangePlayer(lastRocket);
        }

        public void EndEVA(Rocket rocket)
        {
            if (!(PlayerController.main.player.Value is Astronaut_EVA eva)) return;

            rocket.GetComponent<RocketOxygen>().ReturnOxygen(eva.GetComponent<Astronaut>().GetOxygenSecondsLeft());

            AstronautManager.DestroyEVA(eva, false);
            
            PlayerController.main.SmoothChangePlayer(rocket);
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
            // wtf was I thinking when writing this exact line? like wtf
            //AstronautManager manager = GameObject.Find("Astronaut Manager").GetComponent<AstronautManager>();

            if (AstronautState.main.GetAstronautByName("WorldBuild EVA") == null)
                AstronautState.main.CreateAstronaut("WorldBuild EVA");
            
            var eva = AstronautManager.main.SpawnEVA("WorldBuild EVA", PlayerController.main.player.Value.location.Value, PlayerController.main.player.Value.transform.rotation.z, 0, false, 1, 0);
            
            var pos = CapsuleScanner.main.selectedCapsule.Value.GetGlobalPosition();

            if (pos == null)
            {
                AstronautManager.DestroyEVA(eva, false);

                return;
            }

            eva.transform.position = pos;

            IEWInjector.ForceRefresh();

            Astronaut astronaut = eva.GetComponent<Astronaut>();
            AstronautManagementGUI.main.OnFrame(); // refresh gui so the elements can be added to dict

            if (PlayerController.main.player.Value is Rocket rocket)
            {
                // negotiate oxygen amount with the rocket
                astronaut.oxygenSeconds = rocket.GetComponent<RocketOxygen>().RequestOxygen(astronaut.oxygenSeconds);
            } else 
            { 
                return; 
            }

            if (astronaut.oxygenSeconds == -1)
            {
                AstronautManager.DestroyEVA(eva, false);
                return;
            }

            PlayerController.main.SmoothChangePlayer(eva);
        }

        private bool CheckWorld() => Utility.CheckSceneLoaded("World_PC");
    }
}
