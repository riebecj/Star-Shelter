using System;

namespace Phonon
{
	public class PhononManagerContainer
	{
		private ComputeDevice computeDevice = new ComputeDevice();

		private Scene scene = new Scene();

		private Environment environment = new Environment();

		private EnvironmentalRenderer environmentRenderer = new EnvironmentalRenderer();

		private BinauralRenderer binauralRenderer = new BinauralRenderer();

		private ProbeManager probeManager = new ProbeManager();

		private int refCounter;

		public void Initialize(bool initializeRenderer, PhononManager phononManager)
		{
			if (refCounter++ != 0)
			{
				return;
			}
			int numComputeUnits;
			bool useOpenCL;
			ComputeDeviceType deviceType = phononManager.ComputeDeviceSettings(out numComputeUnits, out useOpenCL);
			SimulationSettings simulationSettings = phononManager.SimulationSettings();
			GlobalContext globalContext = phononManager.GlobalContext();
			RenderingSettings renderingSettings = phononManager.RenderingSettings();
			try
			{
				computeDevice.Create(useOpenCL, deviceType, numComputeUnits);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			probeManager.Create();
			if (scene.Create(computeDevice, simulationSettings, globalContext) == Error.None)
			{
				try
				{
					environment.Create(computeDevice, simulationSettings, scene, probeManager, globalContext);
				}
				catch (Exception ex2)
				{
					throw ex2;
				}
				if (initializeRenderer)
				{
					environmentRenderer.Create(environment, renderingSettings, simulationSettings, globalContext);
				}
			}
			if (initializeRenderer)
			{
				binauralRenderer.Create(environment, renderingSettings, globalContext);
			}
		}

		public void Destroy()
		{
			refCounter--;
			if (refCounter == 0)
			{
				environment.Destroy();
				scene.Destroy();
				computeDevice.Destroy();
				probeManager.Destroy();
				binauralRenderer.Destroy();
				environmentRenderer.Destroy();
			}
		}

		public Scene Scene()
		{
			return scene;
		}

		public ProbeManager ProbeManager()
		{
			return probeManager;
		}

		public Environment Environment()
		{
			return environment;
		}

		public EnvironmentalRenderer EnvironmentalRenderer()
		{
			return environmentRenderer;
		}

		public BinauralRenderer BinauralRenderer()
		{
			return binauralRenderer;
		}
	}
}
