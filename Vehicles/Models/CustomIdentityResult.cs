using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SystemTextJsonSamples;

namespace Vehicles.Models
{
	public class CustomIdentityResult{
			public bool Succeeded {get;set;}
			public string [] Errors{get;set;}
	}
}

