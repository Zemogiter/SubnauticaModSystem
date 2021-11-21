private IEnumerator TryExtractItems()
		{
			if (extractingItems)
			{
				yield break;
			}
			if (!enableCheckbox.toggled)
			{
				yield break;
			}

			bool extractedAnything = false;
			Dictionary<string, int> extractionResults = new Dictionary<string, int>();

#if SN1
			List<Vehicle> localVehicles = vehicles.ToList();
			foreach (var vehicle in localVehicles)
			{
				var vehicleName = vehicle.GetName();
				extractionResults[vehicleName] = 0;
				var vehicleContainers = vehicle.gameObject.GetComponentsInChildren<StorageContainer>().Select((x) => x.container).ToList();
				vehicleContainers.AddRange(GetSeamothStorage(vehicle));
#elif BZ
			foreach (Dockable dockable in vehicles)
			{
				if (dockable.gameObject == null)
					continue;

				string vehicleName = "";
				NamePlate namePlate = dockable.gameObject.GetComponent<NamePlate>();
				if (namePlate != null)
				{
					vehicleName = namePlate.namePlateText;
				}
				else
				{
					vehicleName = dockable.gameObject.name;
				}

				extractionResults[vehicleName] = 0;
				List<ItemsContainer> vehicleContainers = new List<ItemsContainer>();
				vehicleContainers.AddRange(dockable.gameObject.GetComponentsInChildren<StorageContainer>().Select((x) => x.container).ToList());
				List<ItemsContainer> dockableContainers = GetSeamothStorage(dockable);
				if (dockableContainers != null)
					vehicleContainers.AddRange(dockableContainers);
				
#endif
				bool couldNotAdd = false;
				foreach (var vehicleContainer in vehicleContainers)
				{
					foreach (var item in vehicleContainer.ToList())
					{
						if (!enableCheckbox.toggled)
						{
							break;
						}

						if (container.container.HasRoomFor(item.item))
						{
							var success = container.container.AddItem(item.item);
							if (success != null)
							{
								extractionResults[vehicleName]++;
								if (extractingItems == false)
								{
									ErrorMessage.AddDebug("Extracting items from vehicle storage...");
								}
								extractedAnything = true;
								extractingItems = true;
								yield return new WaitForSeconds(Mod.config.ExtractInterval);
							}
							else
							{
								couldNotAdd = true;
								break;
							}
						}
						else
						{
							couldNotAdd = true;
							break;
						}
					}

					if (couldNotAdd || !enableCheckbox.toggled)
					{
						break;
					}
				}
			}

			if (extractedAnything)
			{
				NotifyExtraction(extractionResults);
			}
			extractingItems = false;
		}