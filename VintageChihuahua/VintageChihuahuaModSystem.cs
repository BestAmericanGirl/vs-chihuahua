using Vintagestory.API.Common;

using Genelib;

namespace VintageChihuahua
{	
	public class VintageChihuahuaSystem : ModSystem
	{
		public static VintageChihuahuaSystem Instance;
		public override void Start(ICoreAPI api)
		{
			base.Start(api);
			api.RegisterEntityBehaviorClass(TestBehavior.Code, typeof(TestBehavior));

			// Register the custom gene interpreter
			GenomeType.RegisterInterpreter(new CanineSizeGenetics());



			Instance = this;
		}

		public override double ExecuteOrder()
		{
			// Run after genelib loads
			return 1.0;
		}
	}
}