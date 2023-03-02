namespace Assets.Scripts.StateMachineScripts
{
    public interface IState
    {
        void TickState();
        void OnEnterState(IState previousState);
        void OnExitState();

        bool IsAllowedToMove();
    }
}