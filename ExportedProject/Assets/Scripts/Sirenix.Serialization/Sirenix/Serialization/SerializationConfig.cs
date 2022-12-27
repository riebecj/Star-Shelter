namespace Sirenix.Serialization
{
	public class SerializationConfig
	{
		private readonly object LOCK = new object();

		private volatile ISerializationPolicy serializationPolicy;

		private volatile DebugContext debugContext;

		public ISerializationPolicy SerializationPolicy
		{
			get
			{
				if (serializationPolicy == null)
				{
					lock (LOCK)
					{
						if (serializationPolicy == null)
						{
							serializationPolicy = SerializationPolicies.Unity;
						}
					}
				}
				return serializationPolicy;
			}
			set
			{
				lock (LOCK)
				{
					serializationPolicy = value;
				}
			}
		}

		public DebugContext DebugContext
		{
			get
			{
				if (debugContext == null)
				{
					lock (LOCK)
					{
						if (debugContext == null)
						{
							debugContext = new DebugContext();
						}
					}
				}
				return debugContext;
			}
			set
			{
				lock (LOCK)
				{
					debugContext = value;
				}
			}
		}

		public SerializationConfig()
		{
			ResetToDefault();
		}

		public void ResetToDefault()
		{
			lock (LOCK)
			{
				serializationPolicy = null;
				debugContext = null;
			}
		}
	}
}
