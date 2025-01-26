using System;

namespace WorldBuild.Mod.Saving
{
    [Serializable]
    public struct AstronautSaveData
    { 
        public bool evaActive;
        public bool inEva;
        public Double2 position;
        public Double2 speed;
        public string planetName;
        public float rotation;
        public float rotationSpeed;
        public double oxygen;
        public float materialLeft;
        public double fuelPercent;
        public float temperature;
    }
}