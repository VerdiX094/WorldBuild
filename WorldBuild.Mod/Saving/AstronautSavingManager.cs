using ModLoader.Helpers;
using SFS;
using SFS.IO;
using SFS.Parsers.Json;
using SFS.World;
using WorldBuild.Mod.Managers;

namespace WorldBuild.Mod.Saving
{
    public class AstronautSavingManager : BaseManager<AstronautSavingManager>
    {
        private const string SaveFileName = "AstronautSaving.txt";

        public bool astronautSwitchBlocked;
        
        public void OnAstronautSpawnerInitialized()
        {
            if (!AstronautDataHelper.main.SaveData.evaActive) return;

            var data = AstronautDataHelper.main.SaveData;
            
            var loc = new Location(Base.planetLoader.planets[data.planetName], data.position, data.speed);
            
            var eva = AstronautSpawner.main.StartAndGetEVA(loc, data.rotation, data.rotationSpeed, false, data.fuelPercent, data.temperature);

            if (astronautSwitchBlocked)
            {
                astronautSwitchBlocked = false;
                return;
            }

            if (!data.isCurrentPlayer) return;
            
            PlayerController.main.player.Value = eva;
        }
        
        public void OnSave(FolderPath path)
        {
            var saveText = JsonWrapper.ToJson(
                AstronautDataHelper.main.SaveData,
                false);
            Debugger.Log(AstronautDataHelper.main.SaveData.position.y);
            Debugger.Log(saveText);
            JsonWrapper.SaveAsJson(path.ExtendToFile(SaveFileName), AstronautDataHelper.main.SaveData, false);
        }

        public void OnLoad(FolderPath path)
        {
            var saveFile = path.ExtendToFile(SaveFileName);

            if (!saveFile.FileExists()) return;
            
            AstronautDataHelper.main.SaveData =
                JsonWrapper.FromJson<AstronautSaveData>(
                    saveFile.ReadText());
        }
    }
}
