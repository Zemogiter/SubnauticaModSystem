using Common.Mod;
using Common.Utility;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using QModManager.API;
using SMLHelper.V2.Handlers;
using SMLHelper.V2.Json;
#if SUBNAUTICA
    using RecipeData = SMLHelper.V2.Crafting.TechData;
    using Sprite = Atlas.Sprite;
#elif BELOWZERO
using TMPro;
#endif

namespace CustomBeacons
{
	[Serializable]
	public class ColorInfo
	{
		public List<SerializableColor> Colors = new List<SerializableColor>();
	}

	static class Mod
	{
#if !BELOWZERO
		public static Config config;
#else
#pragma warning disable IDE1006 // Naming Styles
		public static Config config { get; } = OptionsPanelHandler.RegisterModOptions<Config>();
#pragma warning restore IDE1006 // Naming Styles
#endif
		private const int StartingPingIndex = 100;

		public static ColorInfo colorInfo;

		private static string modDirectory;

		public static void Patch(string modDirectory = null)
		{
			Logger.Log("Starting patching");

			Mod.modDirectory = modDirectory ?? "Subnautica_Data/Managed";
			LoadConfig();

			new Harmony("com.CustomBeacons.mod").PatchAll(Assembly.GetExecutingAssembly());

			foreach (var color in colorInfo.Colors)
			{
				CustomPings.AddPingColor(color.ToColor());
			}

			AddCustomPings();

			CustomPings.Initialize();

			Logger.Log("Patched");
		}

		private static void AddCustomPings()
		{
			var assetDir = GetAssetPath("Pings");

			int pingIndex = StartingPingIndex;
			foreach (var file in Directory.GetFiles(assetDir))
			{
				var name = Path.GetFileNameWithoutExtension(file);
				name = name.SubstringFromOccuranceOf("_", 0);
				CustomPings.AddPingType(pingIndex, name, sprite: new AtlasPopulationMode());
				ImageUtils.LoadSprite(file, new Vector2(0.5f, 0.5f)); //this used to go one line above, keeping just in case

				pingIndex++; 
			}
		}

		public static string GetModPath()
		{
			return Environment.CurrentDirectory + "/" + modDirectory;
		}

		public static string GetAssetPath(string filename)
		{
			return GetModPath() + "/Assets/" + filename;
		}
				
		private static void LoadConfig()
		{
#if !BELOWZERO
			config = ModUtils.LoadConfig<Config>(GetModPath() + "/config.json");
#endif	
		}
	}
}