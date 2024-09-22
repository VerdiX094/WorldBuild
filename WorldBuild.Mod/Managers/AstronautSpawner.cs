using SFS.Career;
using SFS.Translations;
using SFS.UI;
using SFS.World;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Mod.Modules;
using WorldBuild.Mod.UI;

namespace WorldBuild.Mod.Managers
{
    public class AstronautSpawner : WorldManager<AstronautSpawner>
    {
        private Rocket lastRocket;

        public bool isRocket
        {
            get
            {
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
            PlayerController.main.player.OnChange += (o, n) =>
            {
                if (n is Rocket rocket)
                    lastRocket = rocket;

                if (n is Astronaut_EVA eva)
                    Manager.main.EnterBuild();
                else
                    Manager.main.ExitBuild();
            };
        }

        private void Update()
        {
        }

        public void StartEVA()
        {
            if (CapsuleScanner.main.selectedCapsule.Value.cm == null)
            {
                MsgDrawer.main.Log("No capsule selected!");
                return;
            }

            var player = PlayerController.main.player.Value;
            var loc = player.location.Value;

            var eva = StartAndGetEVA(new Location(loc.planet, WorldView.ToGlobalPosition(CapsuleScanner.main.selectedCapsule.Value.GetGlobalPosition()), loc.velocity), player.transform.rotation.z);

            Astronaut astronaut = eva.GetComponent<Astronaut>();
            AstronautManagementGUI.main.OnFrame(); // refresh gui so the elements can be added to dict

            if (PlayerController.main.player.Value is Rocket rocket)
            {
                // negotiate oxygen amount with the rocket
                astronaut.oxygenSeconds = rocket.GetComponent<RocketOxygen>().RequestOxygen(astronaut.oxygenSeconds);
            }
            else
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

        public Astronaut_EVA StartAndGetEVA(Location loc, float rotation)
        {
            // wtf was I thinking when writing this exact line? like wtf
            //AstronautManager manager = GameObject.Find("Astronaut Manager").GetComponent<AstronautManager>();

            if (AstronautState.main.GetAstronautByName("WorldBuild EVA") == null)
                AstronautState.main.CreateAstronaut("WorldBuild EVA");


            var eva = AstronautManager.main.SpawnEVA("WorldBuild EVA",
                loc,
                rotation, 0, false, 1, 0);

            IEWInjector.ForceRefresh();

            return eva;
        }

        private bool CheckWorld() => Utility.CheckSceneLoaded("World_PC");
    }
}
