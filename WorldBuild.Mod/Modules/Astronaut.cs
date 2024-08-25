using SFS.UI.ModGUI;
using SFS.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldBuild.Mod.UI;

namespace WorldBuild.Mod.Modules
{
    public class Astronaut : InjectEverywhereWith<Astronaut_EVA>
    {
        private double startTime;

        public double oxygenSeconds = 300;

        public double GetOxygenSecondsLeft()
        {
            return oxygenSeconds - (WorldTime.main.worldTime - startTime);
        }

        private void Start()
        {
            startTime = WorldTime.main.worldTime;

            StartCoroutine(nameof(TimeEstimateTextCoro));
        }

        private void Update()
        {
            if (TargetComponent.CanPickItselfUp && TargetComponent.ragdollTime > 2)
            {
                TargetComponent.SetRagdoll(false);
            }

            if (GetOxygenSecondsLeft() <= 0)
            {
                AstronautSpawner.main.EndEVAAndReturnToRocket(true);
            }

            //(AstronautManagementGUI.main.Elements["oxygenBarSlider"] as Slider).Value = (float) (GetOxygenSecondsLeft() / oxygenSeconds) * 100;
        }

        private IEnumerator TimeEstimateTextCoro()
        {
            while (true)
            {
                int secondsLeft = (int) (GetOxygenSecondsLeft() * Random.Range(0.95f, 1.05f));
                int minutesLeft = secondsLeft / 60;
                int hoursLeft = secondsLeft / 3600;

                try
                {
                    (AstronautManagementGUI.main.Elements["oxygenLeftApprox"] as Label).Text =
                        $"Oxygen left: {minutesLeft}m {secondsLeft - minutesLeft * 60}s";
                } catch (KeyNotFoundException)
                {
                    Debug.Log("this is fucked up");
                }
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
}
