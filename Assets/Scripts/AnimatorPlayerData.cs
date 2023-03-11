namespace Assets.Scripts
{
    public struct AnimatorPlayerData
    {
        public bool IsRunningForward;
        public bool IsRunningBackward;

        public bool IsRunningStrafeLeft;
        public bool IsRunningStrafeRight;

        public bool IsRotatingLeft;
        public bool IsRotatingRight;

        public bool JumpStart;

        public void ResetAnimatorData()
        {
            IsRunningForward = false;
            IsRunningBackward = false;

            IsRunningStrafeLeft = false;
            IsRunningStrafeRight = false;

            IsRotatingLeft = false;
            IsRotatingRight = false;

            JumpStart = false;
        }
    }
}
