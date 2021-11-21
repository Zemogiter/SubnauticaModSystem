﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
using TMPro;
#endif

namespace CustomBeacons
{
	public static class CustomPings
	{
		private const string TranslationPrefix = "Ping";
		private static readonly FieldInfo CachedEnumString_valueToString = typeof(CachedEnumString<PingType>).GetField("valueToString", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly FieldInfo PingManager_colorOptions = typeof(PingManager).GetField("colorOptions", BindingFlags.Public | BindingFlags.Static);

		private static Dictionary<int, string> PingNamesByType = new Dictionary<int, string>();
		private static Dictionary<string, int> PingTypesByName = new Dictionary<string, int>();
		private static Dictionary<int, AtlasPopulationMode> PingSpritesByType = new Dictionary<int, AtlasPopulationMode>();
		private static Dictionary<string, AtlasPopulationMode> PingSpritesByName = new Dictionary<string, AtlasPopulationMode>();
		
		private static bool initialized;
		private static Dictionary<PingType, string> stringCache;
		private static Dictionary<PingType, string> translationCache;
		private static List<Color> originalPingColors = PingManager.colorOptions.ToList();
		private static List<Color> customPingColors = new List<Color>(originalPingColors);

		public static Dictionary<int, string> CustomPingNames { get => PingNamesByType; }
		public static Dictionary<int, AtlasPopulationMode> CustomPingSprites { get => PingSpritesByType; }
		public static List<Color> CustomPingColors { get => customPingColors; }

		public static void Initialize()
		{
			if (initialized)
			{
				return;
			}

			stringCache = (Dictionary<PingType, string>)CachedEnumString_valueToString.GetValue(PingManager.sCachedPingTypeStrings);
			translationCache = (Dictionary<PingType, string>)CachedEnumString_valueToString.GetValue(PingManager.sCachedPingTypeTranslationStrings);

			foreach (var pingEntry in PingNamesByType)
			{
				AddPingToStringCache(pingEntry.Key, pingEntry.Value);
			}

			InjectPingColorsToGlobalList();

			initialized = true;
		}

		private static void AddPingToStringCache(int type, string name)
		{
			var pingType = (PingType)type;
			stringCache[pingType] = name;
			translationCache[pingType] = name;
		}

		public static void AddPingType(int type, string name, AtlasPopulationMode sprite)
		{
			if (PingNamesByType.ContainsKey(type))
			{
				Logger.Error("Ping already exists for type: {0} ({1})", type, name);
				return;
			}
			if (PingTypesByName.ContainsKey(name))
			{
				Logger.Error("Ping already exists for name: {0} ({1})", name, type);
				return;
			}

			PingNamesByType[type] = name;
			PingTypesByName[name] = type;
			PingSpritesByType[type] = sprite;
			PingSpritesByName[name] = sprite;

			if (initialized)
			{
				AddPingToStringCache(type, name);
			}
		}

		public static void InjectPingColorsToGlobalList()
		{
			PingManager_colorOptions.SetValue(null, customPingColors.ToArray());
		}

		public static void AddPingColor(Color color)
		{
			if (!customPingColors.Contains(color))
			{
				customPingColors.Add(color);
				if (initialized)
				{
					InjectPingColorsToGlobalList();
				}
			}
		}

		public static Color GetColor(int colorIndex)
		{
			if (colorIndex >= 0 && colorIndex < PingManager.colorOptions.Length)
			{
				return PingManager.colorOptions[colorIndex];
			}
			Logger.Error(StackTraceUtility.ExtractStackTrace());
			return Color.black;
		}

		internal static string GetPingName(int type)
		{
			if (PingNamesByType.TryGetValue(type, out string name))
			{
				return name;
			}
			return string.Empty;
		}

		public static bool PingExists(int type)
		{
			return PingNamesByType.ContainsKey(type);
		}

		public static bool PingExists(string name)
		{
			return name != null && PingTypesByName.ContainsKey(name);
		}

		public static PingType GetPingType(string name)
		{
			return name == null ? PingType.None : (PingType)GetPingTypeAsInt(name);
		}

		public static int GetPingTypeAsInt(string name)
		{
			if (name != null && PingTypesByName.TryGetValue(name, out int value))
			{
				return value;
			}
			return 0;
		}

		public static AtlasPopulationMode GetSprite(int type)
		{
			if (PingSpritesByType.TryGetValue(type, out AtlasPopulationMode sprite))
			{
				return sprite;
			}
			return 0;
		}

		public static AtlasPopulationMode GetSprite(PingType type)
		{
			return GetSprite((int)type);
		}

		public static AtlasPopulationMode GetSprite(string name)
		{
			if (name != null && PingSpritesByName.TryGetValue(name, out AtlasPopulationMode sprite))
			{
				return sprite;
			}
			return 0;
		}
	}
}
