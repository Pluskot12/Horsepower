using Demo.Perception;
using UnityEngine;

namespace Horsepower.NPC
{

    public abstract class EnemyComponent : MonoBehaviour
    {
        public virtual void Init(EnemyContext context) { }
        public virtual void Tick(EnemyContext context, float dt) { }
        public virtual void Dispose(EnemyContext context) { }
    }
}
