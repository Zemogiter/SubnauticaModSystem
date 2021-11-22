using System;
#if SUBNAUTICA
using Oculus.Newtonsoft.Json;
#elif BELOWZERO
using Newtonsoft.Json;
using SMLHelper.V2.Json;
#endif

namespace CustomBeacons
{
#if !BELOWZERO
	[JsonObject]
	internal class Config
#else
	internal class Config : ConfigFile
#endif
	{
#if !BELOWZERO
		[JsonIgnore]
#endif
	}
}
