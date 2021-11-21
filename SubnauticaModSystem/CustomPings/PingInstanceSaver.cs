﻿using Common.Mod;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomBeacons
{
	[Serializable]
	class PingInstanceSaveData
	{
		public int PingType = 0;
	}

	class PingInstanceSaver : MonoBehaviour, IProtoEventListener
	{
		private PingInstance ping;
		private string id;

		private void Awake()
		{
			ping = GetComponent<PingInstance>();
			var uniqueIdentifier = GetComponent<PrefabIdentifier>();
			if (uniqueIdentifier != null)
			{
				id = uniqueIdentifier.Id;
			}
			else if (gameObject.name == "EscapePod")
			{
				id = "EscapePod";
			}
			else
			{
				Logger.Error("Created a PingInstance with no uniqueIdentifier (" + gameObject.name + ")");
				Destroy(this);
			}
		}

		private string GetSaveDataPath()
		{
			var saveFile = Path.Combine("CustomBeacons", id + ".json");
			return saveFile;
		}

		private PingInstanceSaveData CreateSaveData()
		{
			var saveData = new PingInstanceSaveData {
				PingType = (int)ping.pingType
			};

			return saveData;
		}

		public void OnProtoSerialize(ProtobufSerializer serializer)
		{
			var userStorage = PlatformUtils.main.GetUserStorage();
			userStorage.CreateContainerAsync(Path.Combine(SaveLoadManager.main.GetCurrentSlot(), "CustomBeacons"));

			var saveDataFile = GetSaveDataPath();
			var saveData = CreateSaveData();
			ModUtils.Save(saveData, saveDataFile);
		}

		public void OnProtoDeserialize(ProtobufSerializer serializer)
		{
			var saveDataFile = GetSaveDataPath();
			ModUtils.LoadSaveData<PingInstanceSaveData>(saveDataFile, OnLoadSaveData);
		}

		private void OnLoadSaveData(PingInstanceSaveData saveData)
		{
			if (ping != null && saveData != null)
			{
				ping.pingType = (PingType)saveData.PingType;
				PingManager.NotifyColor(ping);
				PingManager.NotifyVisible(ping);
			}
		}
	}
}
