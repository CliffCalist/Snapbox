namespace WhiteArrow.Snapbox
{
    public class SimpleEntityInitializer : EntityStateHandler<ISimpleInitializable>
    {
        protected override void InitializeCore()
        {
            Target.Init();
        }
    }
}