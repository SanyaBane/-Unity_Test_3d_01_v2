using Pathfinding;

namespace Assets.Scripts.Extensions
{
    public static class AIPathExtensions
    {
        public static float GetEndReachedDistance(this AIPath aiPath)
        {
            return aiPath.endReachedDistance;
        }
        
        public static void SetEndReachedDistance(this AIPath aiPath, float endReachedDistance)
        {
            aiPath.endReachedDistance = endReachedDistance;
        }
        
        public static float GetSlowdownDistance(this AIPath aiPath)
        {
            return aiPath.slowdownDistance;
        }
        
        public static void SetSlowdownDistance(this AIPath aiPath, float slowdownDistance)
        {
            aiPath.slowdownDistance = slowdownDistance;
        }
    }
}