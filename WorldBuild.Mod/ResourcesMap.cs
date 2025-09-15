using System.Collections.Generic;

namespace WorldBuild.Mod
{
    public struct PlanetResourceData
    {
        public float OilMultiplier;
        public float OreMultiplier;
        public float OxygenMultiplier;
    }

    public static class ResourcesMap
    {
        private static Dictionary<string, PlanetResourceData> planetResources =
            new Dictionary<string, PlanetResourceData>()
            {
                {
                    "Mercury", new PlanetResourceData()
                    {
                        OilMultiplier = 1.5f,
                        OreMultiplier = 0.9f,
                        OxygenMultiplier = 0f,
                    }
                },
                {
                    "Venus", new PlanetResourceData()
                    {
                        OilMultiplier = 1.2f,
                        OreMultiplier = 1.2f,
                        OxygenMultiplier = 0f,
                    }
                },
                {
                    "Earth", new PlanetResourceData()
                    {
                        OilMultiplier = 0.05f,
                        OreMultiplier = 0.05f,
                        OxygenMultiplier = 2f,
                    }
                },
                    {
                        "Moon", new PlanetResourceData()
                        {
                            OilMultiplier = 0.5f,
                            OreMultiplier = 2.5f,
                            OxygenMultiplier = 0.01f
                        }
                    },
                    {
                        "Captured Asteroid", new PlanetResourceData()
                        {
                            OilMultiplier = 0.5f,
                            OreMultiplier = 4.5f,
                            OxygenMultiplier = 0f
                        }
                    },
                {
                    "Mars", new PlanetResourceData()
                    {
                        OilMultiplier = 1.2f,
                        OreMultiplier = 2.1f,
                        OxygenMultiplier = 0.1f
                    }
                },
                    {
                        "Phobos", new PlanetResourceData()
                        {
                            OilMultiplier = 0.6f,
                            OreMultiplier = 1.4f,
                            OxygenMultiplier = 0f
                        }
                    },
                    {
                        "Deimos", new PlanetResourceData()
                        {
                            OilMultiplier = 0.5f,
                            OreMultiplier = 1.3f,
                            OxygenMultiplier = 0f
                        }
                    },
                {
                    "Jupiter", new PlanetResourceData() // how the fuck is one even supposed to get there
                    {
                        OilMultiplier = 0f,
                        OreMultiplier = 0f,
                        OxygenMultiplier = 0f
                    }
                },
                    {
                        "Io", new PlanetResourceData()
                        {
                            OilMultiplier = 1.7f,
                            OreMultiplier = 0.5f,
                            OxygenMultiplier = 0.1f
                        }
                    },
                    {
                        "Europa", new PlanetResourceData()
                        {
                            OilMultiplier = 1.55f,
                            OreMultiplier = 0.2f,
                            OxygenMultiplier = 0f
                        }
                    },
                    {
                        "Ganymede", new PlanetResourceData()
                        {
                            OilMultiplier = 1.3f,
                            OreMultiplier = 0.7f,
                            OxygenMultiplier = 0f
                        }
                    },
                    {
                        "Callisto", new PlanetResourceData()
                        {
                            OilMultiplier = 1.2f,
                            OreMultiplier = 0.8f,
                            OxygenMultiplier = 0f
                        }
                    },
            };


        public static float GetOreAt(string planetName, float angle = 0f)
        {
            if (!planetResources.TryGetValue(planetName, out var resource)) return 0;
            
            return resource.OreMultiplier;
        }

        public static float GetOilAt(string planetName, float angle = 0f)
        {
            if (!planetResources.TryGetValue(planetName, out var resource)) return 0;

            return resource.OilMultiplier;
        }
        
        public static float GetOxygenAt(string planetName, float angle = 0f)
        {
            if (!planetResources.TryGetValue(planetName, out var resource)) return 0;
            
            return resource.OxygenMultiplier;
        }
    }
}