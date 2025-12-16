using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace VintageChihuahua
{
	public class AiTaskIdleLookAtTarget : AiTaskIdleR
	{
		private AiTaskLookAtEntityConversable? lookTask;

		public AiTaskIdleLookAtTarget(EntityAgent entity, JsonObject taskConfig, JsonObject aiConfig)
			: base(entity, taskConfig, aiConfig)
		{
		}

		public override void StartExecute()
		{
			base.StartExecute();

			// Search for a target player using the parent class's targeting system
			if (SearchForTarget() && targetEntity != null)
			{
				// Create and start the look-at task
				lookTask = new AiTaskLookAtEntityConversable(entity, targetEntity);
				lookTask.StartExecute();
			}
		}

		public override bool ContinueExecute(float dt)
		{
			// Check base idle conditions first
			if (!base.ContinueExecute(dt))
			{
				return false;
			}

			// Continue the look task if we have one
			if (lookTask != null && targetEntity != null && targetEntity.Alive)
			{
				lookTask.ContinueExecute(dt);
			}

			return true;
		}

		public override void FinishExecute(bool cancelled)
		{
			base.FinishExecute(cancelled);

			lookTask = null;
			targetEntity = null;
		}
	}
}
