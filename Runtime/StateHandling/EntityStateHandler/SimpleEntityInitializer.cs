namespace WhiteArrow.SnapboxSDK
{
    public class SimpleEntityInitializer : EntityStateHandler<ISimpleInitializable>
    {
        protected override void InitializeCore()
        {
            Target.Init();
        }
    }
}