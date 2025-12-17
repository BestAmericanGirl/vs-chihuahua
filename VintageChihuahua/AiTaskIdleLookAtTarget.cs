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

            float velcroMultiplier = GetVelcroMultiplier();

            if (velcroMultiplier != 1 && durationUntilMs > 0)
            {
                // Base function calculates duration, which we'll modify based on velcro
                long currentDuration = durationUntilMs - entity.World.ElapsedMilliseconds;
                durationUntilMs = entity.World.ElapsedMilliseconds + (long)(currentDuration * velcroMultiplier);
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

			float velcroMultiplier = GetVelcroMultiplier();

			if (velcroMultiplier != 1)
			{
                float cooldownMultiplier = 2.0f - velcroMultiplier;
				long currentCooldown = cooldownUntilMs - entity.World.ElapsedMilliseconds;
				cooldownUntilMs = entity.World.ElapsedMilliseconds + (long)(currentCooldown * cooldownMultiplier);
			}

			lookTask = null;
            targetEntity = null;
        }

        // Harcoded lower priority if not full velcro (same as wander)
        public override float Priority => GetVelcroMultiplier() == 1.0 ? baseConfig.Priority : 1.0f;

        protected override bool CheckExecutionChance()
        {
            return Rand.NextDouble() <= baseConfig.ExecutionChance * GetVelcroMultiplier();
        }

        //protected override bool CheckCooldowns()
        //{
        //    return cooldownUntilMs * (2 - GetVelcroMultiplier()) <= entity.World.ElapsedMilliseconds && cooldownUntilTotalHours * GetVelcroMultiplier() <= entity.World.Calendar.TotalHours;
        //}

        private float GetVelcroMultiplier()
        {
            return entity.WatchedAttributes.GetFloat(CaninaGenetics.VelcroAttribute, 1.0f);
        }
    }
}
