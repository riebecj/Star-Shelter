using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OscJack
{
	public class OscServer
	{
		private Thread _thread;

		private UdpClient _udpClient;

		private IPEndPoint _endPoint;

		private OscParser _osc;

		public bool IsRunning
		{
			get
			{
				return _thread != null && _thread.IsAlive;
			}
		}

		public int MessageCount
		{
			get
			{
				lock (_osc)
				{
					return _osc.MessageCount;
				}
			}
		}

		public OscServer(int listenPort)
		{
			_endPoint = new IPEndPoint(IPAddress.Any, listenPort);
			_udpClient = new UdpClient(_endPoint);
			_udpClient.Client.ReceiveTimeout = 1000;
			_osc = new OscParser();
		}

		public OscMessage PopMessage()
		{
			lock (_osc)
			{
				return _osc.PopMessage();
			}
		}

		public void Start()
		{
			if (_thread == null)
			{
				_thread = new Thread(ServerLoop);
				_thread.Start();
			}
		}

		public void Close()
		{
			_udpClient.Close();
		}

		private void ServerLoop()
		{
			try
			{
				while (true)
				{
					byte[] data = _udpClient.Receive(ref _endPoint);
					lock (_osc)
					{
						_osc.FeedData(data);
					}
				}
			}
			catch (SocketException)
			{
			}
		}
	}
}
