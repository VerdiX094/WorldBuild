using System;
using HarmonyLib;
using SFS.UI;
using SFS.World;
using SFS.Parts.Modules;

namespace WorldBuild.Mod.Patches
{
	// Displays current remaining amounts for every resource bar using proper units
    [HarmonyPatch(typeof(ResourceBar), nameof(ResourceBar.UpdatePercent))]
    static class ResourceBarPatch
    {
        [HarmonyPostfix]
        static void Postfix(ResourceBar __instance, float percent)
        {
            if (PlayerController.main == null) return;

			// EVA fuel bar shows remaining delta-v in m/s
            var eva = PlayerController.main.player.Value as Astronaut_EVA;
            if (eva != null && eva.resources != null)
            {
				double dvLeft = Math.Max(0, eva.resources.fuelPercent.Value) * EVA_Resources.Fuel_DeltaV;
				__instance.percentText.Text = dvLeft.ToString("0.00") + " m/s";
				__instance.countText.Text = "";
                return;
            }

            var rocket = PlayerController.main.player.Value as Rocket;
            if (rocket == null || rocket.resources == null) return;

			// Title info and bar index within the title section
			int titleIndex;
			string title = FindTitleAbove(__instance, out titleIndex);
			if (string.IsNullOrEmpty(title)) { __instance.percentText.Text = ""; __instance.countText.Text = ""; return; }
			string titleLower = title.ToLowerInvariant();
			int barIndex = GetBarIndexInSection(__instance, titleIndex);

			// Build ordered list of groups for this type - align with SFSControl: ONLY localGroups
			var groups = new System.Collections.Generic.List<ResourceModule>();
            if (rocket.resources.localGroups != null)
            {
				foreach (var g in rocket.resources.localGroups)
					if (IsTypeMatch(g.resourceType, titleLower)) groups.Add(g);
			}

			// Determine bars count and bars-per-group to map index -> group precisely
			int barsCountUnderTitle = CountBarsInSection(__instance, titleIndex);
			int barsPerGroup = 1;
			if (groups.Count > 0 && barsCountUnderTitle >= groups.Count && barsCountUnderTitle % groups.Count == 0)
				barsPerGroup = barsCountUnderTitle / groups.Count;

			bool compressedSingleBarForManyGroups = (groups.Count > 1 && barsCountUnderTitle == 1);

			// Compute current amount
			double currentAmount = 0;
			string unit = "";
			bool isOxygenType = false;

			if (compressedSingleBarForManyGroups)
			{
				// Aggregate all groups into the single bar (sum of current amounts)
				foreach (var r in groups)
				{
					unit = unit == "" ? (r.resourceType.resourceUnit.Field ?? "") : unit;
					string name = (r.resourceType.displayName.Field ?? r.resourceType.name ?? "").ToLowerInvariant();
					isOxygenType = isOxygenType || unit == "s" || name.Contains("oxygen") || name.Contains("氧");
					currentAmount += r.ResourceAmount;
				}
			}
			else if (groups.Count > 0)
			{
				int groupIndex = 0;
				if (barsPerGroup > 1)
					groupIndex = Math.Max(0, Math.Min(groups.Count - 1, barIndex / barsPerGroup));
				else if (groups.Count > 1)
					groupIndex = Math.Max(0, barIndex % groups.Count);
				var r = groups[groupIndex];
				unit = r.resourceType.resourceUnit.Field ?? "";
				string name = (r.resourceType.displayName.Field ?? r.resourceType.name ?? "").ToLowerInvariant();
				isOxygenType = unit == "s" || name.Contains("oxygen") || name.Contains("氧");
				currentAmount = r.ResourceAmount;
            }
            else
            {
				__instance.percentText.Text = "";
				__instance.countText.Text = "";
				return;
			}

			// Display
			if (isOxygenType || unit == "s")
			{
				int seconds = (int)Math.Round(Math.Max(0, currentAmount));
				__instance.percentText.Text = FormatTime(seconds);
				__instance.countText.Text = "";
			}
			else
			{
				__instance.percentText.Text = Math.Max(0, currentAmount).ToString("0.00") + unit;
				__instance.countText.Text = "";
			}
		}

		// Finds the title label immediately above the current bar and returns its sibling index
		static string FindTitleAbove(ResourceBar bar, out int titleSiblingIndex)
		{
			titleSiblingIndex = -1;
			var parent = bar.transform.parent;
			if (parent == null) return null;
			for (int i = bar.transform.GetSiblingIndex() - 1; i >= 0; i--)
			{
				var node = parent.GetChild(i);
				var t = node.GetComponentInChildren<TextAdapter>();
				if (t != null && !string.IsNullOrEmpty(t.Text) && t.Text.Contains(":"))
				{
					titleSiblingIndex = i;
					string title = t.Text.Trim();
					if (title.EndsWith(":")) title = title.Substring(0, title.Length - 1);
					return title;
				}
			}
			return null;
		}

		// Counts how many bars exist between title and this bar to get the index in section
		static int GetBarIndexInSection(ResourceBar bar, int titleSiblingIndex)
		{
			if (titleSiblingIndex < 0) return 0;
			var parent = bar.transform.parent;
			int target = bar.transform.GetSiblingIndex();
			int index = 0;
			for (int i = titleSiblingIndex + 1; i < target; i++)
			{
				var rb = parent.GetChild(i).GetComponent<ResourceBar>();
				if (rb != null) index++;
			}
			return index;
		}

		// Counts total bars under the same title section (until next title)
		static int CountBarsInSection(ResourceBar bar, int titleSiblingIndex)
		{
			if (titleSiblingIndex < 0) return 1;
			var parent = bar.transform.parent;
			int count = 0;
			for (int i = titleSiblingIndex + 1; i < parent.childCount; i++)
			{
				var node = parent.GetChild(i);
				var t = node.GetComponentInChildren<TextAdapter>();
				if (t != null && !string.IsNullOrEmpty(t.Text) && t.Text.Contains(":")) break;
				if (node.GetComponent<ResourceBar>() != null) count++;
			}
			return count;
		}

		// Checks if a module resource type matches the title text
		static bool IsTypeMatch(ResourceType type, string titleLower)
		{
			string display = (type.displayName.Field ?? type.name ?? "").Trim().ToLowerInvariant();
			return display.Contains(titleLower) || titleLower.Contains(display);
		}

		// Formats seconds as XmYs or Xmin
        static string FormatTime(int totalSeconds)
        {
			if (totalSeconds <= 0) return "0s";
			if (totalSeconds < 60) return totalSeconds + "s";
			int m = totalSeconds / 60; int s = totalSeconds % 60;
			return s > 0 ? (m + "min" + s + "s") : (m + "min");
		}
	}
}