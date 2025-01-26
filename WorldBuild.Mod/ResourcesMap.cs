using System.Collections.Generic;

namespace WorldBuild.Mod
{
    public struct PlanetResourceData
    {
        public float oilMultiplier;
        public float oreMultiplier;

        public Dictionary<float, float> oilAmounts;
        public Dictionary<float, float> oreAmounts;
    }

    public static class ResourcesMap
    {
        private static Dictionary<string, PlanetResourceData> planetResources =
            new Dictionary<string, PlanetResourceData>()
            {
                {
                    "Mercury", new PlanetResourceData()
                    {
                        oilMultiplier = 1.2f,
                        oreMultiplier = 1.2f,
                        
                        oreAmounts = new Dictionary<float, float>()
                        {
                            {0, 0},
                            {43, 0},
                            {45, 1},
                            {47, 0}
                        }
                    }
                }
            };


        public static float GetOreAt(string planetName, float angle)
        {
            if (!planetResources.ContainsKey(planetName)) return 0;
            if (angle < 0) return 0;
            if (angle > 360) return 0;
            
            var data = planetResources[planetName];
            
            //(float, float) FindClosestAngles()
            {
                
            }
            
            switch (data.oreAmounts.Count)
            {
                case 0:
                    return 0;
                case 1:
                    return data.oreAmounts[0];
                default:
                    
                    break;
            }

            return 0;
        }
    }
}