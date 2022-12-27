using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.Utilities;
using UnityEngine;

namespace Sirenix.Serialization
{
	public static class SerializationUtility
	{
		public static IDataWriter CreateWriter(Stream stream, SerializationContext context, DataFormat format)
		{
			switch (format)
			{
			case DataFormat.Binary:
				return new BinaryDataWriter(stream, context);
			case DataFormat.JSON:
				return new JsonDataWriter(stream, context);
			case DataFormat.Nodes:
				Debug.LogError(string.Concat("Cannot automatically create a writer for the format '", DataFormat.Nodes, "', because it does not use a stream."));
				return null;
			default:
				throw new NotImplementedException(format.ToString());
			}
		}

		public static IDataReader CreateReader(Stream stream, DeserializationContext context, DataFormat format)
		{
			switch (format)
			{
			case DataFormat.Binary:
				return new BinaryDataReader(stream, context);
			case DataFormat.JSON:
				return new JsonDataReader(stream, context);
			case DataFormat.Nodes:
				Debug.LogError(string.Concat("Cannot automatically create a reader for the format '", DataFormat.Nodes, "', because it does not use a stream."));
				return null;
			default:
				throw new NotImplementedException(format.ToString());
			}
		}

		public static void SerializeValueWeak(object value, IDataWriter writer)
		{
			Serializer.GetForValue(value).WriteValueWeak(value, writer);
			writer.FlushToStream();
		}

		public static void SerializeValueWeak(object value, IDataWriter writer, out List<UnityEngine.Object> unityObjects)
		{
			using (Cache<UnityReferenceResolver> cache = Cache<UnityReferenceResolver>.Claim())
			{
				writer.Context.IndexReferenceResolver = cache.Value;
				Serializer.GetForValue(value).WriteValueWeak(value, writer);
				writer.FlushToStream();
				unityObjects = cache.Value.GetReferencedUnityObjects();
			}
		}

		public static void SerializeValue<T>(T value, IDataWriter writer)
		{
			Serializer.Get<T>().WriteValue(value, writer);
			writer.FlushToStream();
		}

		public static void SerializeValue<T>(T value, IDataWriter writer, out List<UnityEngine.Object> unityObjects)
		{
			using (Cache<UnityReferenceResolver> cache = Cache<UnityReferenceResolver>.Claim())
			{
				writer.Context.IndexReferenceResolver = cache.Value;
				Serializer.Get<T>().WriteValue(value, writer);
				writer.FlushToStream();
				unityObjects = cache.Value.GetReferencedUnityObjects();
			}
		}

		public static void SerializeValueWeak(object value, Stream stream, DataFormat format, SerializationContext context = null)
		{
			if (context != null)
			{
				SerializeValueWeak(value, CreateWriter(stream, context, format));
				return;
			}
			using (Cache<SerializationContext> cache = Cache<SerializationContext>.Claim())
			{
				SerializeValueWeak(value, CreateWriter(stream, cache, format));
			}
		}

		public static void SerializeValueWeak(object value, Stream stream, DataFormat format, out List<UnityEngine.Object> unityObjects, SerializationContext context = null)
		{
			if (context != null)
			{
				SerializeValueWeak(value, CreateWriter(stream, context, format), out unityObjects);
				return;
			}
			using (Cache<SerializationContext> cache = Cache<SerializationContext>.Claim())
			{
				SerializeValueWeak(value, CreateWriter(stream, cache, format), out unityObjects);
			}
		}

		public static void SerializeValue<T>(T value, Stream stream, DataFormat format, SerializationContext context = null)
		{
			if (context != null)
			{
				SerializeValue(value, CreateWriter(stream, context, format));
				return;
			}
			using (Cache<SerializationContext> cache = Cache<SerializationContext>.Claim())
			{
				SerializeValue(value, CreateWriter(stream, cache, format));
			}
		}

		public static void SerializeValue<T>(T value, Stream stream, DataFormat format, out List<UnityEngine.Object> unityObjects, SerializationContext context = null)
		{
			if (context != null)
			{
				SerializeValue(value, CreateWriter(stream, context, format), out unityObjects);
				return;
			}
			using (Cache<SerializationContext> cache = Cache<SerializationContext>.Claim())
			{
				SerializeValue(value, CreateWriter(stream, cache, format), out unityObjects);
			}
		}

		public static byte[] SerializeValueWeak(object value, DataFormat format)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SerializeValueWeak(value, memoryStream, format);
				return memoryStream.ToArray();
			}
		}

		public static byte[] SerializeValueWeak(object value, DataFormat format, out List<UnityEngine.Object> unityObjects)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SerializeValueWeak(value, memoryStream, format, out unityObjects);
				return memoryStream.ToArray();
			}
		}

		public static byte[] SerializeValue<T>(T value, DataFormat format, SerializationContext context = null)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SerializeValue(value, memoryStream, format, context);
				return memoryStream.ToArray();
			}
		}

		public static byte[] SerializeValue<T>(T value, DataFormat format, out List<UnityEngine.Object> unityObjects, SerializationContext context = null)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				SerializeValue(value, memoryStream, format, out unityObjects, context);
				return memoryStream.ToArray();
			}
		}

		public static object DeserializeValueWeak(IDataReader reader)
		{
			return Serializer.Get<object>().ReadValueWeak(reader);
		}

		public static object DeserializeValueWeak(IDataReader reader, List<UnityEngine.Object> referencedUnityObjects)
		{
			using (Cache<UnityReferenceResolver> cache = Cache<UnityReferenceResolver>.Claim())
			{
				cache.Value.SetReferencedUnityObjects(referencedUnityObjects);
				reader.Context.IndexReferenceResolver = cache.Value;
				return Serializer.Get<object>().ReadValueWeak(reader);
			}
		}

		public static T DeserializeValue<T>(IDataReader reader)
		{
			return Serializer.Get<T>().ReadValue(reader);
		}

		public static T DeserializeValue<T>(IDataReader reader, List<UnityEngine.Object> referencedUnityObjects)
		{
			using (Cache<UnityReferenceResolver> cache = Cache<UnityReferenceResolver>.Claim())
			{
				cache.Value.SetReferencedUnityObjects(referencedUnityObjects);
				reader.Context.IndexReferenceResolver = cache.Value;
				return Serializer.Get<T>().ReadValue(reader);
			}
		}

		public static object DeserializeValueWeak(Stream stream, DataFormat format, DeserializationContext context = null)
		{
			if (context != null)
			{
				return DeserializeValueWeak(CreateReader(stream, context, format));
			}
			using (Cache<DeserializationContext> cache = Cache<DeserializationContext>.Claim())
			{
				return DeserializeValueWeak(CreateReader(stream, cache, format));
			}
		}

		public static object DeserializeValueWeak(Stream stream, DataFormat format, List<UnityEngine.Object> referencedUnityObjects, DeserializationContext context = null)
		{
			if (context != null)
			{
				return DeserializeValueWeak(CreateReader(stream, context, format), referencedUnityObjects);
			}
			using (Cache<DeserializationContext> cache = Cache<DeserializationContext>.Claim())
			{
				return DeserializeValueWeak(CreateReader(stream, cache, format), referencedUnityObjects);
			}
		}

		public static T DeserializeValue<T>(Stream stream, DataFormat format, DeserializationContext context = null)
		{
			if (context != null)
			{
				return DeserializeValue<T>(CreateReader(stream, context, format));
			}
			using (Cache<DeserializationContext> cache = Cache<DeserializationContext>.Claim())
			{
				return DeserializeValue<T>(CreateReader(stream, cache, format));
			}
		}

		public static T DeserializeValue<T>(Stream stream, DataFormat format, List<UnityEngine.Object> referencedUnityObjects, DeserializationContext context = null)
		{
			if (context != null)
			{
				return DeserializeValue<T>(CreateReader(stream, context, format), referencedUnityObjects);
			}
			using (Cache<DeserializationContext> cache = Cache<DeserializationContext>.Claim())
			{
				return DeserializeValue<T>(CreateReader(stream, cache, format), referencedUnityObjects);
			}
		}

		public static object DeserializeValueWeak(byte[] bytes, DataFormat format)
		{
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return DeserializeValueWeak(stream, format);
			}
		}

		public static object DeserializeValueWeak(byte[] bytes, DataFormat format, List<UnityEngine.Object> referencedUnityObjects)
		{
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return DeserializeValueWeak(stream, format, referencedUnityObjects);
			}
		}

		public static T DeserializeValue<T>(byte[] bytes, DataFormat format, DeserializationContext context = null)
		{
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return DeserializeValue<T>(stream, format, context);
			}
		}

		public static T DeserializeValue<T>(byte[] bytes, DataFormat format, List<UnityEngine.Object> referencedUnityObjects, DeserializationContext context = null)
		{
			using (MemoryStream stream = new MemoryStream(bytes))
			{
				return DeserializeValue<T>(stream, format, referencedUnityObjects, context);
			}
		}

		public static object CreateCopy(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			if (obj is string)
			{
				return obj;
			}
			Type type = obj.GetType();
			if (type.IsValueType)
			{
				return obj;
			}
			if (type.InheritsFrom(typeof(UnityEngine.Object)))
			{
				return obj;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				List<UnityEngine.Object> unityObjects;
				SerializeValue(obj, memoryStream, DataFormat.Binary, out unityObjects);
				memoryStream.Position = 0L;
				return DeserializeValue<object>(memoryStream, DataFormat.Binary, unityObjects);
			}
		}
	}
}
