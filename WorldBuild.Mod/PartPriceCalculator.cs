using SFS.Parts;
using SFS.Parts.Modules;
using UnityEngine;
using SFS.Sharing;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBuild.Mod
{
    public static class PartPriceCalculator
    {
        public static int Calculate(Part part)
        {
            float multiplier = GetPriceMultiplier(part);

            return (int) (part.mass.Value * multiplier * Mathf.Abs(part.orientation.orientation.Value.x * part.orientation.orientation.Value.y));
        }

        private static float GetPriceMultiplier(Part part)
        {
            if (part.GetComponent<ParachuteModule>())
                return 1.5f;
            if (part.GetComponent<CrewModule>())
                return 2f;
            if (part.GetComponent<DetachModule>())
                return 2f;
            if (part.GetComponent<EngineModule>())
                return 3f * Mathf.Sqrt(part.GetComponent<EngineModule>().thrust.Value * part.GetComponent<EngineModule>().ISP.Value / (120f * 240f));
            if (part.GetComponent<WheelModule>())
                return 10f;
            if (part.GetComponent<RcsModule>())
                return 10f;
            if (part.displayName.Field == "Probe")
                return 10f;
            if (part.displayName.Field == "Landing Leg")
                return 10f;
            if (part.displayName.Field.subs[0].Contains("Panel"))
                return 10f;

            return 1f;
        }
    }
}
