using System.Collections;
using System.Collections.Generic;

namespace OscJack
{
	public class OscDirectory : IEnumerable<KeyValuePair<string, object[]>>, IEnumerable
	{
		private Dictionary<string, object[]> _dataStore;

		private OscServer[] _servers;

		private int _totalMessageCount;

		public int TotalMessageCount
		{
			get
			{
				UpdateState();
				return _totalMessageCount;
			}
		}

		public OscDirectory(int port)
			: this(new int[1] { port })
		{
		}

		public OscDirectory(int[] portList)
		{
			_dataStore = new Dictionary<string, object[]>();
			_servers = new OscServer[portList.Length];
			for (int i = 0; i < portList.Length; i++)
			{
				_servers[i] = new OscServer(portList[i]);
				_servers[i].Start();
			}
		}

		public bool HasData(string address)
		{
			return _dataStore.ContainsKey(address);
		}

		public object[] GetData(string address)
		{
			UpdateState();
			object[] value;
			_dataStore.TryGetValue(address, out value);
			return value;
		}

		public IEnumerator<KeyValuePair<string, object[]>> GetEnumerator()
		{
			return _dataStore.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dataStore.GetEnumerator();
		}

		private void UpdateState()
		{
			OscServer[] servers = _servers;
			foreach (OscServer oscServer in servers)
			{
				while (oscServer.MessageCount > 0)
				{
					OscMessage oscMessage = oscServer.PopMessage();
					_dataStore[oscMessage.address] = oscMessage.data;
					_totalMessageCount++;
				}
			}
		}
	}
}
