namespace ch.sycoforge.Util.Collections
{
	public abstract class SynchronizedCollection
	{
		internal readonly object _synclock = new object();
	}
}
