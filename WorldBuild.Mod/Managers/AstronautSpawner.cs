using SFS.Career;
using SFS.Translations;
using SFS.UI;
using SFS.World;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;
using WorldBuild.Mod.Modules;
using WorldBuild.Mod.Saving;
using WorldBuild.Mod.UI;
using WorldBuild.Mod.Build;

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

            return rocket.partHolder.parts.Any(part => part.Name == "Capsule" || part.GetComponent<CrewModule>() != null);
        }

        public void EndEVAAndReturnToRocket(bool death = false)
        {
            if (!lastRocket || !(PlayerController.main.player.Value is Astronaut_EVA eva)) return;

            AstronautManager.DestroyEVA(eva, death);

            PlayerController.main.player.Value = lastRocket;
            
            AstronautDataHelper.main.SaveData.evaActive = false;
        }

        public void EndEVA(Rocket rocket)
        {
            if (!(PlayerController.main.player.Value is Astronaut_EVA eva)) return;

            rocket.GetComponent<RocketOxygen>().ReturnOxygen(eva.GetComponent<Astronaut>().GetOxygenSecondsLeft());

            AstronautManager.DestroyEVA(eva, false);

            PlayerController.main.SmoothChangePlayer(rocket);
            
            AstronautDataHelper.main.SaveData.evaActive = false;
        }

        private void Start()
        {
            PlayerController.main.player.OnChange += (o, n) =>
            {
                if (n is Rocket rocket)
                    lastRocket = rocket;

                if (!(n is Astronaut_EVA))
                    BuildManager.main.ExitBuild();
            };
            
            AstronautSavingManager.main.OnAstronautSpawnerInitialized();
        }
        
        public void StartEVA()
        {
            if (CapsuleScanner.main.selectedCapsule.Value.cm == null)
            {
                MsgDrawer.main.Log("No capsule selected!");
                return;
            }
            
            if (!(PlayerController.main.player.Value is Rocket rocket)) return;
            
            var ox = rocket.GetComponent<RocketOxygen>();

            if (!ox) return;

            if (ox.CalculateOxygenAvailable() < 30)
            {
                MsgDrawer.main.Log("Not enough oxygen for at least 30 seconds of EVA");
                return;
            }
            
            var player = PlayerController.main.player.Value;
            var loc = player.location.Value;

            var eva = StartAndGetEVA(new Location(loc.planet, WorldView.ToGlobalPosition(CapsuleScanner.main.selectedCapsule.Value.GetGlobalPosition()), loc.velocity), player.transform.rotation.z);

            Astronaut astronaut = eva.GetComponent<Astronaut>();
            AstronautManagementGUI.main.OnFrame(); // refresh gui so the elements can be added to dict
            
            astronaut.oxygenSeconds = rocket.GetComponent<RocketOxygen>().RequestOxygen(astronaut.oxygenSeconds);

            if (astronaut.oxygenSeconds == -1)
            {
                AstronautManager.DestroyEVA(eva, false);
                return;
            }

            PlayerController.main.SmoothChangePlayer(eva);
        }

        public Astronaut_EVA StartAndGetEVA(Location loc, float rotation, float angVel = 0, bool ragdoll = false, double fuelPercent = 1, float temperature = 0f)
        {
            // wtf was I thinking when writing this exact line? like wtf
            //AstronautManager manager = GameObject.Find("Astronaut Manager").GetComponent<AstronautManager>();

            if (AstronautState.main.GetAstronautByName("WorldBuild EVA") == null)
                AstronautState.main.CreateAstronaut("WorldBuild EVA");

            var eva = AstronautManager.main.SpawnEVA("WorldBuild EVA",
                loc,
                rotation, 0, false, 1, 0);

            eva.gameObject.name = "RIP IAmWater";

            IEWInjector.ForceRefresh();

            AstronautDataHelper.main.SaveData.evaActive = true;
            
            return eva;
        }

        private bool CheckWorld() => Utility.CheckSceneLoaded("World_PC");
    }
}