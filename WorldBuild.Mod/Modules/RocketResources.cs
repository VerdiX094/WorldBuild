using SFS.UI;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS.Parts.Modules;
using UnityEngine;

namespace WorldBuild.Mod.Modules
{
    public class RocketResources : InjectEverywhereWith<Rocket>
    {
        public enum ResourceType
        {
            Oxygen,
            BuildResource,
            Oil
        }

        public const double OXYGEN_TONS_TO_EVA_SECONDS = 300.0;

        public static string ResourceTypeToString(ResourceType resourceType)
        {
            switch (resourceType)
            {
                case ResourceType.Oxygen:
                    return "Oxygen";
                case ResourceType.BuildResource:
                    return "Steel";
                case ResourceType.Oil:
                    return "Oil";
            }

            return "???";
        }
        
        // this may bug out when docking new capsules, idk, i don't give a fuck anymore

        private List<ResourceModule> GetAllResourceModules(ResourceType type)
        {
            var resourceType = SFSResources.GetByName(ResourceTypeToString(type));
            
            var result = new List<ResourceModule>();
            
            foreach (var rm in TargetComponent.resources.globalGroups)
            {
                if (rm == null) continue;
                if (rm.resourceType == null) continue;
                if (rm.resourceType == resourceType) 
                    result.Add(rm);
            }

            return result;
            
            return TargetComponent.resources.globalGroups.Where((resourceModule) => resourceModule != null && resourceModule.resourceType != null && resourceModule.resourceType.displayName == resourceType.displayName).ToList();
        }
        
        private double GetResourceAmountLeft(ResourceType type)
        {
            double amount = 0;
            
            foreach (var resourceModule in GetAllResourceModules(type))
            {
                amount += resourceModule.ResourceAmount;
            }

            return amount;
        }

        private void TakeResource(double amount, ResourceType type, out bool success)
        {
            var resourceAmount = GetResourceAmountLeft(type);
            if (resourceAmount < amount) success = false;

            double left = amount;
            
            foreach (ResourceModule rm in GetAllResourceModules(type))
            {
                double toTake = Math.Min(rm.ResourceAmount, left);
                rm.TakeResource(toTake);
                left -= toTake;
            }
            
            success = true;
        }

        private void ReturnResource(double amount, ResourceType type, out double wastedAmount)
        {
            wastedAmount = amount;

            foreach (var resourceModule in GetAllResourceModules(type))
            {
                double toReturn = Math.Min(resourceModule.ResourceSpace, amount);
                resourceModule.AddResource(toReturn);
                wastedAmount -= toReturn;
            }
        }

        /// <summary>
        /// Looks around the capsules and tries to match the requested amount.
        /// </summary>
        /// <param name="amount">The amount of requested oxygen</param>
        /// <returns>The actual amount of oxygen granted</returns>
        public double RequestEVASeconds(double amount, ResourceType resourceType = ResourceType.Oxygen)
        {
            if (CalculateEVASecondsAvailable() < 30)
            {
                return -1;
            }
            double result = Math.Min(amount, GetResourceAmountLeft(resourceType));
            
            TakeResource(result, resourceType, out _);
            
            return result.Round(3);
        }

        public double CalculateEVASecondsAvailable(ResourceType resourceType = ResourceType.Oxygen)
        {
            return GetResourceAmountLeft(resourceType) * OXYGEN_TONS_TO_EVA_SECONDS;
        }

        /// <summary>
        /// Looks around capsules and returns a given amount of oxygen to them.
        /// </summary>
        /// <param name="amount">The amount of oxygen to return</param>
        /// <returns>The amount of oxygen wasted</returns>
        public double ReturnEVASeconds(double amount, bool logWaste = true, ResourceType resourceType = ResourceType.Oxygen)
        {
            ReturnResource(amount, resourceType, out double resourceLeft);

            if (logWaste && resourceLeft > 1)
                MsgDrawer.main.Log($"The rocket's {(resourceType == ResourceType.Oxygen ? "oxygen" : "resource")} tanks are full, {resourceLeft.Round(1)}{(resourceType == ResourceType.Oxygen ? "s of oxygen" : " of resources")} was wasted.");


            return resourceLeft.Round(3);
        }
    }
}
