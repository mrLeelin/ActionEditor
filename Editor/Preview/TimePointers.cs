
namespace NBC.ActionEditor
{
    public interface IDirectableTimePointer
    {
        PreviewBase target { get; }
        float time { get; }
        void TriggerForward(float currentTime, float previousTime);
        void TriggerBackward(float currentTime, float previousTime);
        void Update(float currentTime, float previousTime);
    }

    public struct StartTimePointer : IDirectableTimePointer
    {
        private bool triggered;
        private float lastTargetStartTime;
        public PreviewBase target { get; private set; }
        float IDirectableTimePointer.time => target.Directable.StartTime;

        public StartTimePointer(PreviewBase target)
        {
            this.target = target;
            triggered = false;
            lastTargetStartTime = target.Directable.StartTime;
        }

        void IDirectableTimePointer.TriggerForward(float currentTime, float previousTime)
        {
            if (!target.Directable.IsActive) return;
            if (currentTime >= target.Directable.StartTime)
            {
                if (!triggered)
                {
                    triggered = true;
                    target.Enter(false);
                    target.Update(target.Directable.ToLocalTime(currentTime), 0);
                }
            }
        }

        void IDirectableTimePointer.Update(float currentTime, float previousTime)
        {
            if (!target.Directable.IsActive) return;
            if (currentTime >= target.Directable.StartTime && currentTime < target.Directable.EndTime &&
                currentTime > 0)
            {
                var deltaMoveClip = target.Directable.StartTime - lastTargetStartTime;
                var localCurrentTime = target.Directable.ToLocalTime(currentTime);
                var localPreviousTime = target.Directable.ToLocalTime(previousTime + deltaMoveClip);

                target.Update(localCurrentTime, localPreviousTime);
                lastTargetStartTime = target.Directable.StartTime;
            }
        }

        void IDirectableTimePointer.TriggerBackward(float currentTime, float previousTime)
        {
            if (!target.Directable.IsActive) return;
            if (currentTime < target.Directable.StartTime || currentTime <= 0)
            {
                if (triggered)
                {
                    triggered = false;
                    target.Update(0, target.Directable.ToLocalTime(previousTime));
                    target.Exit(true);
                }
            }
        }
    }

    public struct EndTimePointer : IDirectableTimePointer
    {
        private bool triggered;
        public PreviewBase target { get; private set; }
        float IDirectableTimePointer.time => target.Directable.EndTime;

        public EndTimePointer(PreviewBase target)
        {
            this.target = target;
            triggered = false;
        }

        void IDirectableTimePointer.TriggerForward(float currentTime, float previousTime)
        {
            if (!target.Directable.IsActive) return;
            if (currentTime >= target.Directable.EndTime)
            {
                if (!triggered)
                {
                    triggered = true;
                    target.Update(target.Directable.GetLength(), target.Directable.ToLocalTime(previousTime));
                    target.Exit(false);
                }
            }
        }


        void IDirectableTimePointer.Update(float currentTime, float previousTime)
        {
            
        }


        void IDirectableTimePointer.TriggerBackward(float currentTime, float previousTime)
        {
            if (!target.Directable.IsActive) return;
            if (currentTime < target.Directable.EndTime || currentTime <= 0)
            {
                if (triggered)
                {
                    triggered = false;
                    target.Enter(true);
                    target.Update(target.Directable.ToLocalTime(currentTime), target.Directable.GetLength());
                }
            }
        }
    }
}