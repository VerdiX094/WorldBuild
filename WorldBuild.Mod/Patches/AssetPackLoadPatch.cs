using System.Collections;
using HarmonyLib;
using SFS;
using SFS.Parts;
using SFS.Parts.Modules;
using WorldBuild.Mod.Managers;
using WorldBuild.Mod.Modules;

namespace WorldBuild.Mod.Patches
{
    public class AssetPackLoadPatch : BaseManager<AssetPackLoadPatch>
    {
        void Start()
        {
            StartCoroutine(nameof(WaitAssetPackLoadingComplete));
        }

        private IEnumerator WaitAssetPackLoadingComplete()
        {
            while (!CustomAssetsLoader.finishedLoading)
            {
                yield return null;
            }
            LoadIntoModule();
        }
        
        private void LoadIntoModule()
        {
            foreach (Part part in Base.partsLoader.parts.Values)
            {
                foreach (ResourceModule resMod in part.GetModules<ResourceModule>())
                {
                    SFSResources.resourceTypes.Add(resMod.resourceType);
                    Debugger.Log(resMod.resourceType.ToString());
                }
            }
        }
    }
}