using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CactusFrontEnd.Cosmos.utils;

/// <summary>
///     A <see cref="CosmosSerializer" /> that internally uses Newtonsoft JSON.NET. Adapted from
///     the decompiled source of the internal class <see cref="CosmosJsonDotNetSerializer" /> .
/// </summary>
public class CosmosNewtonsoftJsonSerializer : CosmosSerializer
{
	private static readonly Encoding               defaultEncoding = new UTF8Encoding(false, true);
	private readonly        JsonSerializerSettings serializerSettings;

	/// <summary>
	///     Initializes a new instance of <see cref="CosmosNewtonsoftJsonSerializer" /> with the given parameters.
	/// </summary>
	public CosmosNewtonsoftJsonSerializer([NotNull] JsonSerializerSettings jsonSerializerSettings)
	{
		serializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof(jsonSerializerSettings));
	}

	/// <summary>
	///     Convert a Stream to the passed in type.
	/// </summary>
	/// <typeparam name="T">The type of object that should be deserialized</typeparam>
	/// <param name="stream">An open stream that is readable that contains JSON</param>
	/// <returns>The object representing the deserialized stream</returns>
	public override T FromStream<T>(Stream stream)
	{
		using (stream)
		{
			if (typeof(Stream).IsAssignableFrom(typeof(T)))
			{
				return (T)(object)stream;
			}

			using StreamReader   sr             = new(stream);
			using JsonTextReader jsonTextReader = new(sr);
			JsonSerializer       jsonSerializer = getSerializer();
			return jsonSerializer.Deserialize<T>(jsonTextReader)!;
		}
	}

	/// <summary>
	///     Converts an object to a open readable stream
	/// </summary>
	/// <typeparam name="T">The type of object being serialized</typeparam>
	/// <param name="input">The object to be serialized</param>
	/// <returns>An open readable stream containing the JSON of the serialized object</returns>
	public override Stream ToStream<T>(T input)
	{
		MemoryStream       streamPayload = new();
		using StreamWriter streamWriter  = new(streamPayload, defaultEncoding, 1024, true);
		// ReSharper disable once UsingStatementResourceInitialization
		using JsonWriter writer = new JsonTextWriter(streamWriter)
		                          {
			                          Formatting = Formatting.None
		                          };
		JsonSerializer jsonSerializer = getSerializer();
		jsonSerializer.Serialize(writer, input);
		writer.Flush();
		streamWriter.Flush();

		streamPayload.Position = 0;
		return streamPayload;
	}

	/// <summary>
	///     JsonSerializer has hit a race conditions with custom settings that cause null reference exception.
	///     To avoid the race condition a new JsonSerializer is created for each call
	/// </summary>
	private JsonSerializer getSerializer()
	{
		return JsonSerializer.Create(serializerSettings);
	}
}