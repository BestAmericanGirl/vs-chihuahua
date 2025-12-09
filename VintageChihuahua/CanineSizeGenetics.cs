using System.Linq;
using Genelib;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;

namespace VintageChihuahua
{
    public class TestBehavior : EntityBehavior
    {
		public static readonly string Code = "chihuahua";

		public TestBehavior(Entity entity) : base(entity)
		{
		}

        public override string PropertyName()
        {
			return Code;
		}

		public override void OnEntityLoaded()
		{
			UpdateShape();
		}

		public override void OnEntitySpawn()
		{
			UpdateShape();
		}

		public override void AfterInitialized(bool _)
		{
			UpdateShape();
		}

		private void UpdateShape()
		{
			if (entity.Properties.Client.Renderer is not EntityShapeRenderer renderer || entity.Properties.Client.LoadedAlternateShapes == null)
			{
				return;
			}

			if (entity.Properties.Client.Size > 0.5f)
			{
				entity.WatchedAttributes.SetInt("textureIndex", 0);
				renderer.OverrideEntityShape = entity.Properties.Client.LoadedAlternateShapes[0];
				//entity.MarkShapeModified();
				//return;
			}

			entity.WatchedAttributes.SetInt("textureIndex", 1);
			//renderer.OverrideEntityShape = entity.Properties.Client.LoadedAlternateShapes[1];
			//entity.MarkShapeModified();

		}
	}

    public class CanineSizeGenetics : GeneInterpreter
	{
		public string Name => "CanineSize";

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

			if (!entity.WatchedAttributes.HasAttribute("hasClonedProperties"))
			{
				// Clone the properties so this entity has its own copy
				entity.Properties.Client = (EntityClientProperties)entity.Properties.Client.Clone();
				entity.WatchedAttributes.SetBool("hasClonedProperties", true);
			}

			if (sizeMultiplier < 0.5f)
			{


				//entity.Properties.Client.LoadedShape = entity.Properties.Client.LoadedAlternateShapes[0].Clone();
				entity.Properties.Client.LoadedShapeForEntity = entity.Properties.Client.LoadedAlternateShapes[0];
				entity.Properties.Client.ShapeForEntity = entity.Properties.Client.Shape.Alternates[0];
				//entity.Properties.Client.LoadedAlternateShapes = null;
				//entity.Properties.Client.DetermineLoadedShape(entity.EntityId);
				entity.WatchedAttributes.SetInt("textureIndex", 1);
				entity.requirePosesOnServer = true;
				entity.MarkShapeModified();
				// entity.Properties.Client.Animations = entity.World.GetEntityType(new AssetLocation("caninae:creature-caninae-canina-baby-female-canis-familiaris")).Client.Animations;
				//entity.World.AssetManager
				//var pup = new AssetLocation("caninae:caninae-canina-baby-female-canis-familiaris");
				//var puppy = entity.World.ClassRegistry.CreateEntity(entity.World.GetEntityType(new AssetLocation("caninae:caninae-canina-baby-female-canis-familiaris")));
				//entity.World.SpawnEntity(puppy);
				//entity.Properties.Client = (EntityClientProperties)puppy.Properties.Client.Clone();
				//EntityShapeRenderer
				//var hi = entity.World.GetEntityType(new AssetLocation("caninae:caninae-canina-baby-female-canis-familiaris"));
				//var puppy = entity.World.ClassRegistry.CreateEntity(entity.World.GetEntityType(new AssetLocation("caninae:caninae-canina-baby-female-canis-familiaris")));
				//puppy.Initialize(hi, entity.World.Api, 0);
				//var bla = entity.World.AssetManager.Exists(new AssetLocation("caninae:caninae-canina-baby-female-canis-familiaris"));
				entity.Properties.Client.Init(new AssetLocation("caninae:caninae-canina-baby-female-canis-familiaris"), entity.World);
				if (entity.Api.Side != EnumAppSide.Server){
					//var renderer = new EntityShapeRenderer(entity, (ICoreClientAPI)entity.World.Api);
					//entity.Properties.Client.Renderer = renderer;
					//renderer.TesselateShape();
					
				}
				else
				{
					((ServerAnimator)entity.AnimManager.Animator).ReloadAttachmentPoints();
				}
				entity.AnimManager.AnimationsDirty = true;
				entity.WatchedAttributes.MarkAllDirty();
			}
			else
			{
				//entity.Properties.Client.LoadedAlternateShapes = null;
				//entity.Properties.Client.DetermineLoadedShape(entity.EntityId);
				entity.Properties.Client.LoadedShapeForEntity = entity.Properties.Client.LoadedShape;
				entity.Properties.Client.ShapeForEntity = entity.Properties.Client.Shape;
				entity.WatchedAttributes.SetInt("textureIndex", 0);
				entity.requirePosesOnServer = true;
				entity.MarkShapeModified();
				//entity.AnimManager.Animator = entity.World.Api.Side == EnumAppSide.Client ?
				//	new ClientAnimator(() => 1, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, shape.JointsById) :
				//	new ServerAnimator(() => 1, cacheObj.RootPoses, cacheObj.Animations, cacheObj.RootElems, shape.JointsById)
				//;

				if (entity.Api.Side != EnumAppSide.Server){
					//var renderer = new EntityShapeRenderer(entity, (ICoreClientAPI)entity.World.Api);
					//entity.Properties.Client.Renderer = renderer;
					//renderer.TesselateShape();
				}
				else
				{
					
				}
				entity.AnimManager.AnimationsDirty = true;
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
