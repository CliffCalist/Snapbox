using UnityEngine;

namespace WhiteArrow.SnapboxSDK
{
    [RequireComponent(typeof(ISimpleInitializable))]
    public class SimpleEntityInitializer : EntityStateHandler<ISimpleInitializable>
    {
        protected override void InitializeCore()
        {
            Target.Init();
        }
    }
}