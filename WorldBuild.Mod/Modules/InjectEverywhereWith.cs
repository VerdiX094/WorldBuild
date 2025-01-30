using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldBuild.Mod.Modules
{
    public class InjectEverywhereWith<T> : MonoBehaviour where T : Component 
    {
        protected T TargetComponent => GetComponent<T>();
    }
}
