using Genelib;
using Vintagestory.API.Common;

namespace VintageChihuahua
{
    public class VintageChihuahuaSystem : ModSystem
    {
        public static VintageChihuahuaSystem Instance { get; private set; } = null!;

        public VintageChihuahuaSystem()
        {
            Instance = this;
        }
        public override void Start(ICoreAPI api)
        {
            base.Start(api);

            // Register the custom gene interpreter
            GenomeType.RegisterInterpreter(new CaninaGenetics());
        }

        public override double ExecuteOrder()
        {
            // Run after genelib loads
            return 1.0;
        }
    }
}
