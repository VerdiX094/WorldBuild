using ModLoader.Helpers;
using SFS;
using SFS.Parts;
using SFS.World;
using SFS.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WorldBuild.Mod.Variables;

namespace WorldBuild.Mod
{
    public class CapsuleScanner : BaseManager<CapsuleScanner>
    {
        bool enab = false;
        private void Start()
        {
            SceneHelper.OnWorldSceneLoaded += () =>
            {
                enab = true;

                Screen_Game sg = GameManager.main.world_Input;

                sg.onInputStart = (OnInputStartData oisd) =>
                {

                };

                sg.onInputEnd += (OnInputEndData oied) =>
                {
                    if (!oied.click || oied.inputType != InputType.MouseRight) return;

                    OnRightClick(oied.position.World(0f));
                };
            };

            SceneHelper.OnWorldSceneUnloaded += () =>
            {
                enab = false;
            };
        }

        public Observable<BestCapsuleData> selectedCapsule;

        private void OnRightClick(Vector2 mouseWorldPosition)
        {
            #region Old Code
            //if (!(PlayerController.main.player.Value is Rocket rocket)) return;

            //Vector2 closestCapsule = new Vector2(float.MaxValue, float.MaxValue);
            //Vector2 mp = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //selectedMarkPos = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            //CrewModule bestModule = null;

            //foreach (Part part in rocket.partHolder.parts)
            //{
            //    if (part.GetComponent<CrewModule>() == null) continue;

            //    // assuming that no custom capsule-related parts will be created

            //    Vector2 pt = (Vector2)part.transform.TransformPoint(part.transform.localPosition + Vector3.up);

            //    var thisDist = (mp - pt).magnitude;
            //    Debug.Log(thisDist);


            //    var prevBestDist = float.MaxValue;
            //    if (closestCapsule != null && thisDist < prevBestDist)
            //        prevBestDist = (closestCapsule - pt).magnitude;

            //    if (thisDist > Math.Max(part.orientation.orientation.Value.x, part.orientation.orientation.Value.y) || thisDist > prevBestDist) continue;

            //    selectedMarkPos = pt;
            //    bestModule = part.GetComponent<CrewModule>();
            //}

            //if (bestModule == selectedCrewModule.Value) bestModule = null;

            //selectedCrewModule.Value = bestModule;
            #endregion

            //Double2 mouseWorldPosition = WorldView.ToGlobalPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            var best = FindBest(PlayerController.main.player.Value as Rocket, mouseWorldPosition);

            selectedCapsule.Value = best.cm == selectedCapsule.Value.cm ? new BestCapsuleData() : best;
        }

        private BestCapsuleData FindBest(Rocket rocket, Vector2 position, float maxDistanceMultiplier = 1f)
        {
            if (rocket == null) return new BestCapsuleData();

            BestCapsuleData capsuleData = new BestCapsuleData();

            foreach (Part part in rocket.partHolder.parts.Where(part => part.GetComponent<CrewModule>() != null))
            {
                CrewModule crew = part.GetComponent<CrewModule>();

                double dist = (position - GetGlobalCapsuleCenter(crew)).magnitude;

                Debug.Log(dist);

                if (dist >= capsuleData.GetDistanceTo(position) || dist > 1f * maxDistanceMultiplier) continue;

                capsuleData.cm = crew;
            }

            return capsuleData;
        }

        private static Vector2 GetGlobalCapsuleCenter(CrewModule capsule) => capsule.transform.TransformPoint(Vector3.up);

        public struct BestCapsuleData
        {
            public CrewModule cm;

            public double GetDistanceTo(Vector2 position)
            {
                if (cm == null) return double.PositiveInfinity;

                return (position - GetGlobalCapsuleCenter(cm)).magnitude;
            }

            public bool CMNotNull() => cm != null;
            public Vector2 GetGlobalPosition()
            {
                if (cm == null) return Vector2.positiveInfinity;

                return GetGlobalCapsuleCenter(cm);
            }
        }

        private void Update()
        {
            if (!enab) return;

            if (selectedCapsule.Value.CMNotNull()) 
            {
                GLDrawer.DrawCircle(selectedCapsule.Value.GetGlobalPosition(), 0.25f, 32, Color.yellow);
            }
        }
    }
}
