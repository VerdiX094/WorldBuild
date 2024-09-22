using SFS.UI.ModGUI;
using SFS.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBuild.Mod.UI;
using WorldBuild.Mod.Managers;
using SFS.UI;

namespace WorldBuild.Mod.Modules
{
    public class Astronaut : InjectEverywhereWith<Astronaut_EVA>
    {
        private double startTime;
        
        public double oxygenSeconds = 300;

        public float materialLeft = 0;

        public double GetOxygenSecondsLeft()
        {
            return oxygenSeconds - (WorldTime.main.worldTime - startTime);
        }

        private void Start()
        {
            startTime = WorldTime.main.worldTime;

            //StartCoroutine(nameof(TimeEstimateTextCoro));
        }

        private void Update()
        {
            if (TargetComponent.CanPickItselfUp && TargetComponent.ragdollTime > 2)
            {
                TargetComponent.SetRagdoll(false);
            }

            if (GetOxygenSecondsLeft() <= 0 || !TargetComponent.astronaut.alive)
            {
                MsgDrawer.main.Log("WorldBuild astronaut is dead");
                AstronautSpawner.main.EndEVAAndReturnToRocket(true);
            }

            Location loc = TargetComponent.location.Value;

            var planet = loc.planet;

            double atmoDensity = planet.GetAtmosphericDensity(TargetComponent.location.Value.Height);

            // I assume that earth's 0.005 atmo density = 1 atm, the atmo breathing limits are 0.5-2 atm
            if (atmoDensity > 0.0025 && atmoDensity < 0.01 && planet.data.atmosphereVisuals.GRADIENT.texture == "Atmo_Earth")
            {
                startTime += WorldTime.main.timewarpSpeed * Time.deltaTime;
            }

            AstronautResourcesHelper.data.position = loc.position;
            AstronautResourcesHelper.data.speed = loc.velocity;
            AstronautResourcesHelper.data.planetName = loc.planet.codeName;
            AstronautResourcesHelper.data.inEva = true;
            AstronautResourcesHelper.data.fuelPercent = TargetComponent.resources.fuelPercent.Value;
            AstronautResourcesHelper.data.oxygen = GetOxygenSecondsLeft();
            AstronautResourcesHelper.data.temperature = TargetComponent.resources.temperature.Value;
            AstronautResourcesHelper.data.rotationSpeed = TargetComponent.rb2d.angularVelocity;
            AstronautResourcesHelper.data.materialLeft = 0f; // TODO

            //(AstronautManagementGUI.main.Elements["oxygenBarSlider"] as Slider).Value = (float) (GetOxygenSecondsLeft() / oxygenSeconds) * 100;
        }

        //private IEnumerator TimeEstimateTextCoro()
        //{
        //    while (true)
        //    {
        //        int secondsLeft = (int) (GetOxygenSecondsLeft() * Random.Range(0.95f, 1.05f));
        //        int minutesLeft = secondsLeft / 60;
        //        int hoursLeft = secondsLeft / 3600;

        //        try
        //        {
        //        } catch (KeyNotFoundException)
        //        {
        //            Debugger.Log("this is fucked up");
        //        }
        //        yield return new WaitForSecondsRealtime(1f);
        //    }
        //}
    }
}
