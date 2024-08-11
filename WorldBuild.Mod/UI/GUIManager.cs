using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WorldBuild.Mod.UI
{
    public class GUIManager : BaseManager
    {
        public HashSet<GUIBase> bases;

        void Update()
        {
            bases.ForEach(Base => Base.OnFrame());
        }
    }
}
