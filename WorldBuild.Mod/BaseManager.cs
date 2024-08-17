using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldBuild.Mod
{
    public class BaseManager<T> : MonoBehaviour
        where T : BaseManager<T>
    {
        public static T main;

        public void Awake()
        {
            if (this is T)
            {
                main = this as T;
            }
            else
                Debug.LogError("NOSZ KURWA");
        }
    }
}
