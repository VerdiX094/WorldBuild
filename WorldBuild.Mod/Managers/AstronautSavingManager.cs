using ModLoader.Helpers;
using SFS.Career;
using SFS.IO;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WorldBuild.Mod.Modules;

namespace WorldBuild.Mod.Managers
{
    public class AstronautSavingManager : BaseManager<AstronautSavingManager>
    {
        private AstronautSaveData saveData;
        public string lastPath;

        public bool LASRequest;

        private bool BlockSwitch;

        public void Save(string path, WorldSave save)
        {
            // debug

            var st = new StackTrace();
            for (int i = 0; i < st.FrameCount; i++)
            {
                Debugger.Log(st.GetFrame(i).GetMethod().Name);
            }

            var data = AstronautResourcesHelper.data;

            Debugger.Log(string.Join(
                "\n",
                   data.inEva,
                   data.position,
                   data.speed,
                   data.planetName,
                   data.rotation,
                   data.rotationSpeed,
                   data.oxygen,
                   data.materialLeft,
                   data.fuelPercent,
                   data.temperature
            ));

            new FolderPath(path).ExtendToFile("WorldBuildEVA.txt").WriteBytes(
                SerializeAstronautSaving(
                    data
                    //AstronautManager.main != null ? AstronautManager.main.eva.FirstOrDefault() : null
            ));
        }

        public void LoadAndSwitch(string path)
        {
            if (!Utility.CheckSceneLoaded("World_PC")) return;
            
            Debugger.Log("LAS called");
            AstronautSaveData data;
            try
            {
                data = Deserialize(File.ReadAllBytes(Path.Combine(path, "WorldBuildEVA.txt")));
            } catch (Exception e)
            {
                Debugger.LogException(e);
                return;
            }

            AstronautState.main.selfManageSaving = false;

            if (!data.inEva) return;

            Debugger.Log(string.Join(
                "\n",
                   data.inEva,
                   data.position,
                   data.speed,
                   data.planetName,
                   data.rotation,
                   data.rotationSpeed,
                   data.oxygen,
                   data.materialLeft,
                   data.fuelPercent,
                   data.temperature
            ));

            saveData = data;

            if (BlockSwitch)
            {
                BlockSwitch = false;
                return;
            }

            shouldChangeToAstronaut = true;
        }

        bool shouldChangeToAstronaut;
        public void Update()
        {

            if (Utility.CheckSceneLoaded("World_PC") && LASRequest)
            {
                LASRequest = false;
                
                LoadAndSwitch(lastPath);
            }

            if (shouldChangeToAstronaut && Utility.CheckSceneLoaded("World_PC"))
            {
                shouldChangeToAstronaut = false;

                AstronautState.main.FireAstronaut("WorldBuild EVA");
                AstronautState.main.CreateAstronaut("WorldBuild EVA");

                var eva = AstronautSpawner.main.StartAndGetEVA(
                    new Location(
                        SFS.Base.planetLoader.planets.Where(planet => planet.Value.codeName == saveData.planetName).FirstOrDefault().Value,
                        saveData.position,
                        saveData.speed),
                    saveData.rotation);//, 0, false, data.fuelPercent, data.temperature);
                eva.resources.fuelPercent.Value = saveData.fuelPercent;
                eva.resources.temperature.Value = saveData.temperature;
                eva.rb2d.angularVelocity = saveData.rotationSpeed;

                IEWInjector.ForceRefresh();

                Debugger.Log(eva);

                eva.GetComponent<Astronaut>().oxygenSeconds = saveData.oxygen;

                Debugger.Log("Switching to astronaut");

                PlayerController.main.player.Value = eva;
            }
        }

        private void Start()
        {
            SceneHelper.OnBuildSceneUnloaded += () =>
            {
                Debugger.Log("Blocked");
                BlockSwitch = true;
            };
            SceneHelper.OnHubSceneLoaded += () =>
            {
                Debugger.Log("Unblocked (Hub)");
                BlockSwitch = false;
            };
            SceneHelper.OnHomeSceneLoaded += () =>
            {
                Debugger.Log("Unblocked (Home)");
                BlockSwitch = false;
            };

            SceneHelper.OnWorldSceneLoaded += () =>
            {
                PlayerController.main.player.OnChange += (n) =>
                {
                    if (n is Astronaut_EVA eva)
                    {
                        AstronautResourcesHelper.data.inEva = true;
                    }
                    else if (n != null)
                    {
                        AstronautResourcesHelper.data.inEva = false;
                    }
                };

                if (!LASRequest) LoadAndSwitch(lastPath);
            };
        }

        public byte[] SerializeAstronautSaving(AstronautSaveData data)
        {
            List<byte> bytes = new List<byte>() { (byte)(data.evaActive ? 1 : 0) };

            if (!data.evaActive)
            {
                Debugger.Log("EVA was null while serializing!");
                return bytes.ToArray();
            }

            bytes.Add((byte)(data.inEva ? 1 : 0));

            var x = BitConverter.GetBytes(data.position.x);
            var y = BitConverter.GetBytes(data.position.y);

            var speedX = BitConverter.GetBytes(data.speed.x);
            var speedY = BitConverter.GetBytes(data.speed.y);

            var rot = BitConverter.GetBytes(data.rotation);
            var rotSpeed = BitConverter.GetBytes(data.rotationSpeed);

            var oxygen = BitConverter.GetBytes(data.oxygen);
            var materialLeft = BitConverter.GetBytes(data.materialLeft);
            var fuelLeft = BitConverter.GetBytes(data.fuelPercent);
            var temperature = BitConverter.GetBytes(data.temperature);

            byte planetNameLength = (byte)data.planetName.Length;

            bytes.AddRange(x);
            bytes.AddRange(y);
            bytes.AddRange(speedX);
            bytes.AddRange(speedY);
            bytes.AddRange(rot);
            bytes.AddRange(rotSpeed);
            bytes.AddRange(oxygen);
            bytes.AddRange(materialLeft);
            bytes.AddRange(fuelLeft);
            bytes.AddRange(temperature);
            bytes.Add(planetNameLength);
            bytes.AddRange(Encoding.UTF8.GetBytes(data.planetName));

            return bytes.ToArray();
        }
    
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

        public AstronautSaveData Deserialize(byte[] bytes)
        {
            int dataPointer = 0;
            int IncrementPointer(int bytesLength)
            {
                dataPointer += bytesLength;
                return dataPointer - bytesLength;
            }

            var result = new AstronautSaveData();
            if (bytes.Length <= 1) throw new Exception("EVA was not active!");

            result.evaActive = bytes[IncrementPointer(1)] != 0;
            result.inEva = bytes[IncrementPointer(1)] != 0;

            result.position = new Double2(
                BitConverter.ToDouble(bytes, IncrementPointer(8)),
                BitConverter.ToDouble(bytes, IncrementPointer(8))
            );

            result.speed = new Double2(
                BitConverter.ToDouble(bytes, IncrementPointer(8)),
                BitConverter.ToDouble(bytes, IncrementPointer(8))
            );

            result.rotation = BitConverter.ToSingle(bytes, IncrementPointer(4));
            result.rotationSpeed = BitConverter.ToSingle(bytes, IncrementPointer(4));

            result.oxygen = BitConverter.ToDouble(bytes, IncrementPointer(8));
            result.materialLeft = BitConverter.ToSingle(bytes, IncrementPointer(4));
            result.fuelPercent = BitConverter.ToDouble(bytes, IncrementPointer(8));

            result.temperature = BitConverter.ToSingle(bytes, IncrementPointer(4));

            byte planetNameLength = bytes[IncrementPointer(1)];

            result.planetName = Encoding.UTF8.GetString(Slice(bytes, dataPointer, planetNameLength));
            IncrementPointer(planetNameLength);

            return result;
        }

        private byte[] Slice(byte[] bytes, int startIndex, int length) 
        {
            var result = new byte[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = bytes[i + startIndex];
            }

            return result;
        }
    }
}
