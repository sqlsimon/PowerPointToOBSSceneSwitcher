using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using OBSWebsocketDotNet.Types.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PowerPointToOBSSceneSwitcher
{

	public class ObsLocal : IDisposable
	{
		private bool _DisposedValue;
		private OBSWebsocket _OBS;
		private List<string> validScenes;
		private string defaultScene;

		public ObsLocal() { }

		public Task Connect(string Password,string PortNumber)
		{

			_OBS = new OBSWebsocket();
			string _IPV4Address = GetIPV4Addess();
			System.Console.WriteLine("IPV4 Address: {0}", _IPV4Address);
			string _wsConnectionString = "ws://" + _IPV4Address + ":" + PortNumber;
			System.Console.WriteLine("Connecting to OBS WebSocket Address: {0}", _wsConnectionString);

            try
            {
                _OBS.ConnectAsync(_wsConnectionString, Password);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Connection to OBS failed: {0}", ex.Message.ToString());

            }

            System.Console.Write("Conecting...");
            while (!_OBS.IsConnected)
            {
				System.Threading.Thread.Sleep(5000);
                System.Console.Write(".");
            }

            return Task.CompletedTask;
		}

		private string GetIPV4Addess()
		{
            var networkAddressLIst = Dns.GetHostEntry(Dns.GetHostName());
            string IPV4Adress = networkAddressLIst.AddressList[1].ToString();

            return IPV4Adress;
		}

		public string DefaultScene
        {
            get { return defaultScene; }
			set
			{
				if (validScenes.Contains(value))
				{
					defaultScene = value;
				}
                else
                {
                    Console.WriteLine($"Scene named {value} does not exist and cannot be set as default");
                }
			}
        }

		public bool ChangeScene(string scene)
        {
			if (!validScenes.Contains(scene))
			{
                Console.WriteLine($"Scene named {scene} does not exist");
				if (String.IsNullOrEmpty(defaultScene))
				{
                    Console.WriteLine("No default scene has been set!");
					return false;
				}
			
				scene = defaultScene;
			}

			//_OBS.Api.SetCurrentScene(scene);
			_OBS.SetCurrentProgramScene(scene);
			return true;
        }

		public void GetScenes()
        {
			var allScene = _OBS.ListScenes();
			var list = allScene.Select(s => s.Name).ToList();
            Console.WriteLine("Valid Scenes:");
			foreach (var l in list)
			{
				Console.WriteLine(l);
			}
			validScenes = list;
		}

		public bool StartRecording()
		{
			try { _OBS.StartRecord(); }
			catch {  /* Recording already started */ }
			return true;
		}

		public bool StopRecording()
		{
			try { _OBS.StopRecord(); }
			catch {  /* Recording already stopped */ }
			return true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_DisposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects)
				}

				_OBS.Disconnect();
				_OBS = null;
				_DisposedValue = true;
			}
		}

		~ObsLocal()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}