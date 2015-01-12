using DougKlassen.Revit.Cron.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DougKlassen.Revit.Cron.Repositories
{
	/// <summary>
	/// Serialize and deserialize an RCronTask object
	/// </summary>
	class RCronTaskSpecConverter : JsonConverter
	{
		/// <summary>
		/// This converter is only for RCronTask objects
		/// </summary>
		/// <param name="objectType">The type to check</param>
		/// <returns>Whether the type is an HtmlString</returns>
		public override Boolean CanConvert(Type objectType)
		{
			if (objectType == typeof(RCronTaskSpec))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Read a JSON token representing an RCronTask
		/// </summary>
		/// <param name="reader">The JsonReader to use for reading</param>
		/// <param name="objectType">Type of object to serialize to. This will always RCronTask</param>
		/// <param name="existingValue">The existing value of the object being read</param>
		/// <param name="serializer">The JsonSerializer invoking the converter</param>
		/// <returns>A RCronTask deserialized from the reader. It will be a subclass of RCronTask</returns>
		public override Object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			RCronTaskSpec task;
			Console.WriteLine("reading RCronTask");
			
			//start reading the object			
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw new InvalidOperationException();
			}
			reader.Read(); //read the StartObject
			String taskType = reader.ReadAsString();
			reader.Read(); //advance past TaskType
			switch (taskType)
			{
				case "Print":
					var printTask = new RCronPrintTaskSpec();
					printTask.ProjectFile = reader.ReadAsString();
					reader.Read();
					printTask.OutputDirectory = reader.ReadAsString();
					reader.Read();
					printTask.PrintSet = reader.ReadAsString();
					reader.Read();
					printTask.OutputFileName = reader.ReadAsString();
					reader.Read();
					return printTask;
				case "Export":
					var exportTask = new RCronExportTaskSpec();
					exportTask.ProjectFile = reader.ReadAsString();
					reader.Read();
					exportTask.OutputDirectory = reader.ReadAsString();
					reader.Read();
					exportTask.PrintSet = reader.ReadAsString();
					reader.Read();
					exportTask.ExportSetup = reader.ReadAsString();
					reader.Read();
					return exportTask;
				case "ETransmit":
					var transmitTask = new RCronETransmitTaskSpec();
					transmitTask.ProjectFile = reader.ReadAsString();
					reader.Read();
					transmitTask.OutputDirectory = reader.ReadAsString();
					reader.Read();
					return transmitTask;
				case "Command":
					var commandTask = new RCronCommandTaskSpec();
					commandTask.ProjectFile = reader.ReadAsString();
					reader.Read();
					commandTask.OutputDirectory = reader.ReadAsString();
					reader.Read();
					commandTask.CommandName = reader.ReadAsString();
					reader.Read();
					return commandTask;
				case "AuditCompact":
					var auditTask = new RCronETransmitTaskSpec();
					auditTask.ProjectFile = reader.ReadAsString();
					reader.Read();
					auditTask.OutputDirectory = reader.ReadAsString();
					reader.Read();
					return auditTask;
				case "Test":
					var testTask = new RCronTestTaskSpec();
					testTask.ProjectFile = reader.ReadAsString();
					reader.Read();
					testTask.OutputDirectory = reader.ReadAsString();
					reader.Read();
					return testTask;
				default:
					throw new InvalidOperationException("Unrecognized task type: " + taskType);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="value"></param>
		/// <param name="serializer"></param>
		/// <remarks>The converter isn't used for serialization</remarks>
		public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}
