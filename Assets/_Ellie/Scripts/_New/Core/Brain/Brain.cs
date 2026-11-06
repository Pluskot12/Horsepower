using System;

namespace Ellie.Core
{
    [Serializable]
    public abstract class Brain<TContext>
    {
        public abstract void Setup(TContext context);
        public abstract void Tick(TContext context, float dt);
    }
}
