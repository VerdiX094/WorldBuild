using SFS.UI;
using SFS.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldBuild.Mod.Modules
{
    public class RocketOxygen : InjectEverywhereWith<Rocket>
    {
        // this may bug out when docking new capsules, idk

        /// <summary>
        /// Looks around the capsules and tries to match the requested amount.
        /// </summary>
        /// <param name="amount">The amount of requested oxygen</param>
        /// <returns>The actual amount of oxygen granted</returns>
        public double RequestOxygen(double amount)
        {
            double result = 0;

            double requestedLeft = amount;

            foreach (CapsuleOxygen co in GetComponentsInChildren<CapsuleOxygen>())
            {
                double avail = Math.Min(requestedLeft, co.Oxygen);

                requestedLeft -= avail;

                result += avail;
            }

            if (result < 30)
            {
                MsgDrawer.main.Log("Not enough oxygen for at least 30 seconds of EVA");
                return -1;
            }
            else if (result < amount - 0.001)
            {
                MsgDrawer.main.Log($"Not enough oxygen for full {(int)amount.Round(0)} seconds of EVA,\nstarting EVA with {result}s instead");
            }
            else
            {
                MsgDrawer.main.Log($"The rocket has {(int)(CalculateOxygenAvailable() - result).Round(0)} seconds of oxygen time left.");
            }

            // run this again, if the check succeeded
            requestedLeft = amount;

            foreach (CapsuleOxygen co in GetComponentsInChildren<CapsuleOxygen>())
            {
                double avail = Math.Min(requestedLeft, co.Oxygen);

                requestedLeft -= avail;

                co.Oxygen -= avail;
            }

            return result.Round(3);

            return -1;
        }

        public double CalculateOxygenAvailable()
        {
            double result = 0;

            foreach (CapsuleOxygen co in GetComponentsInChildren<CapsuleOxygen>())
            {
                result += co.Oxygen;
            }

            return result;
            return 0;
        }

        /// <summary>
        /// Looks around capsules and returns a given amount of oxygen to them.
        /// </summary>
        /// <param name="amount">The amount of oxygen to return</param>
        /// <returns>The amount of oxygen wasted</returns>
        public double ReturnOxygen(double amount, bool logWaste = true)
        {
            double oxygenLeft = amount;

            foreach (CapsuleOxygen co in GetComponentsInChildren<CapsuleOxygen>())
            {
                if (oxygenLeft < 0.001) break;
                double toReturn = Math.Min(oxygenLeft, CapsuleOxygen.MaxOxygen - co.Oxygen);

                co.Oxygen += toReturn;
                oxygenLeft -= toReturn;
            }

            if (logWaste && oxygenLeft > 1)
                MsgDrawer.main.Log($"The rocket's oxygen tanks are full, {oxygenLeft}s of oxygen was wasted.");


            return oxygenLeft.Round(3);

            return 0;
        }
    }
}
