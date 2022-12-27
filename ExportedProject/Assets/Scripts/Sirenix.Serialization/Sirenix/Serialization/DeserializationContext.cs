using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sirenix.Utilities;

namespace Sirenix.Serialization
{
	public sealed class DeserializationContext : ICacheNotificationReceiver
	{
		private SerializationConfig config;

		private Dictionary<int, object> internalIdReferenceMap = new Dictionary<int, object>(128);

		private StreamingContext streamingContext;

		private IFormatterConverter formatterConverter;

		public IExternalStringReferenceResolver StringReferenceResolver { get; set; }

		public IExternalGuidReferenceResolver GuidReferenceResolver { get; set; }

		public IExternalIndexReferenceResolver IndexReferenceResolver { get; set; }

		public StreamingContext StreamingContext
		{
			get
			{
				return streamingContext;
			}
		}

		public IFormatterConverter FormatterConverter
		{
			get
			{
				return formatterConverter;
			}
		}

		public SerializationConfig Config
		{
			get
			{
				if (config == null)
				{
					config = new SerializationConfig();
				}
				return config;
			}
			set
			{
				config = value;
			}
		}

		public DeserializationContext()
			: this(default(StreamingContext), new FormatterConverter())
		{
		}

		public DeserializationContext(StreamingContext context)
			: this(context, new FormatterConverter())
		{
		}

		public DeserializationContext(FormatterConverter formatterConverter)
			: this(default(StreamingContext), formatterConverter)
		{
		}

		public DeserializationContext(StreamingContext context, FormatterConverter formatterConverter)
		{
			if (formatterConverter == null)
			{
				throw new ArgumentNullException("formatterConverter");
			}
			streamingContext = context;
			this.formatterConverter = formatterConverter;
			Reset();
		}

		public void RegisterInternalReference(int id, object reference)
		{
			internalIdReferenceMap[id] = reference;
		}

		public object GetInternalReference(int id)
		{
			object value;
			internalIdReferenceMap.TryGetValue(id, out value);
			return value;
		}

		public object GetExternalObject(int index)
		{
			if (IndexReferenceResolver == null)
			{
				Config.DebugContext.LogWarning("Tried to resolve external reference by index (" + index + "), but no index reference resolver is assigned to the deserialization context. External reference has been lost.");
				return null;
			}
			object value;
			if (IndexReferenceResolver.TryResolveReference(index, out value))
			{
				return value;
			}
			Config.DebugContext.LogWarning("Failed to resolve external reference by index (" + index + "); the index resolver could not resolve the index. Reference lost.");
			return null;
		}

		public object GetExternalObject(Guid guid)
		{
			if (GuidReferenceResolver == null)
			{
				Config.DebugContext.LogWarning(string.Concat("Tried to resolve external reference by guid (", guid, "), but no guid reference resolver is assigned to the deserialization context. External reference has been lost."));
				return null;
			}
			for (IExternalGuidReferenceResolver externalGuidReferenceResolver = GuidReferenceResolver; externalGuidReferenceResolver != null; externalGuidReferenceResolver = externalGuidReferenceResolver.NextResolver)
			{
				object value;
				if (externalGuidReferenceResolver.TryResolveReference(guid, out value))
				{
					return value;
				}
			}
			Config.DebugContext.LogWarning(string.Concat("Failed to resolve external reference by guid (", guid, "); no guid resolver could resolve the guid. Reference lost."));
			return null;
		}

		public object GetExternalObject(string id)
		{
			if (StringReferenceResolver == null)
			{
				Config.DebugContext.LogWarning("Tried to resolve external reference by string (" + id + "), but no string reference resolver is assigned to the deserialization context. External reference has been lost.");
				return null;
			}
			for (IExternalStringReferenceResolver externalStringReferenceResolver = StringReferenceResolver; externalStringReferenceResolver != null; externalStringReferenceResolver = externalStringReferenceResolver.NextResolver)
			{
				object value;
				if (externalStringReferenceResolver.TryResolveReference(id, out value))
				{
					return value;
				}
			}
			Config.DebugContext.LogWarning("Failed to resolve external reference by string (" + id + "); no string resolver could resolve the string. Reference lost.");
			return null;
		}

		public void Reset()
		{
			config = null;
			internalIdReferenceMap.Clear();
			IndexReferenceResolver = null;
			GuidReferenceResolver = null;
			StringReferenceResolver = null;
		}

		void ICacheNotificationReceiver.OnFreed()
		{
			Reset();
		}

		void ICacheNotificationReceiver.OnClaimed()
		{
		}
	}
}
