namespace WhiteArrow.SnapboxSDK
{
    public interface IStateNodeParent
    {
        StateGraphPhase GraphPhase { get; }
        string Context { get; }

        void AddChilde(StateNode node);
        void RemoveChilde(StateNode node);
    }
}