using Genelib;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace VintageChihuahua
{
    public class CaninaGenetics : GeneInterpreter
    {
        public string Name => "Canina";

        void GeneInterpreter.Interpret(EntityBehaviorGenetics genetics)
        {
            Entity entity = genetics.entity;
            Genome genome = genetics.Genome;

            // Only apply to adult dogs
            if (!entity.Code.Path.Contains("adult") || !entity.Code.Path.Contains("familiaris"))
            {
                return;
            }

            float sizeMultiplier = CalculateSizeMultiplier(genome);

            if (sizeMultiplier < 0.5f)
            {
                // Pick puppy shape and texture
                entity.Properties.Client.LoadedShapeForEntity = entity.Properties.Client.LoadedAlternateShapes[0];
                entity.Properties.Client.ShapeForEntity = entity.Properties.Client.Shape.Alternates[0];
                entity.WatchedAttributes.SetInt("textureIndex", 1);
            }
            else
            {
                // Pick adult shape and texture
                entity.Properties.Client.LoadedShapeForEntity = entity.Properties.Client.LoadedShape;
                entity.Properties.Client.ShapeForEntity = entity.Properties.Client.Shape;
                entity.WatchedAttributes.SetInt("textureIndex", 0);
            }

            InterpretClientSide(entity, sizeMultiplier);

            // Server only section
            if (entity.Api.Side != EnumAppSide.Server)
            {
                return;
            }

            UpdateCollisionBox(entity, sizeMultiplier);
        }

        private void InterpretClientSide(Entity entity, float sizeMultiplier)
        {

            if (entity.Api.Side != EnumAppSide.Client)
            {
                return;
            }

            // Adjust entity size
            EntityClientProperties client = entity.World.GetEntityType(entity.Code).Client;
            if (sizeMultiplier < 0.5f)
            {
                // Scale back up because the puppy shape is already small
                sizeMultiplier = 0.7f;
            }
            entity.Properties.Client.Size = client.Size * sizeMultiplier;
        }

        private float CalculateSizeMultiplier(Genome genome)
        {
            // SS = 1.0 (standard size)
            // Ss = 0.65 (medium size)
            // ss = 0.3 (chihuahua size)
            if (genome.Homozygous("Size", "S"))
            {
                return 1.0f;
            }

            if (genome.Homozygous("Size", "s"))
            {
                return 0.3f;
            }

            return 0.65f;
        }

        private void UpdateCollisionBox(Entity entity, float sizeMultiplier)
        {
            // Get the base collision box size
            // Default for canis-familiaris is x: 1.0, y: 0.9
            float baseX = 1.0f;
            float baseY = 0.9f;

            // Scale the collision box
            entity.CollisionBox.X2 = baseX * sizeMultiplier;
            entity.CollisionBox.Y2 = baseY * sizeMultiplier;

            // Also update eye height proportionally
            float baseEyeHeight = 1.1f; // Default for canis-familiaris
            entity.Properties.EyeHeight = baseEyeHeight * sizeMultiplier;
        }

        void GeneInterpreter.MatchPhenotype(EntityBehaviorGenetics genetics)
        {
            Entity entity = genetics.entity;
            Genome genome = genetics.Genome;

            // Try to match existing size if present
            float existingSize = entity.Properties.Client.Size;

            // Map size back to genotype
            if (entity.WatchedAttributes.GetInt("textureIndex") == 1)
            {
                // Likely ss (chihuahua)
                genome.SetAutosomal("Size", 0, "s");
                genome.SetAutosomal("Size", 1, "s");
            }
            else if (existingSize < 0.85f)
            {
                // Likely Ss (medium)
                genome.SetAutosomal("Size", 0, "S");
                genome.SetAutosomal("Size", 1, "s");
            }
            else
            {
                // Likely SS (standard)
                genome.SetAutosomal("Size", 0, "S");
                genome.SetAutosomal("Size", 1, "S");
            }
        }
    }
}
