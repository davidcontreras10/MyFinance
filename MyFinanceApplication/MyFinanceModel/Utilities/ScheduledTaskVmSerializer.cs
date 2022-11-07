using System;
using MyFinanceModel.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyFinanceModel.Utilities
{
	public class ScheduledTaskVmSerializer : JsonConverter<BaseScheduledTaskVm>
	{
		public override bool CanRead => true;
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer, BaseScheduledTaskVm value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override BaseScheduledTaskVm ReadJson(
			JsonReader reader,
			Type objectType,
			BaseScheduledTaskVm existingValue,
			bool hasExistingValue,
			JsonSerializer serializer
		)
		{
			return (BaseScheduledTaskVm) serializer.Deserialize(reader, objectType);
		}
	}
}