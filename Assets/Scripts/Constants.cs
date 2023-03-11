using UnityEngine;

namespace Assets.Scripts
{
    public static class Constants
    {
        public const string MESSAGE_RESOURCE_NOT_FOUNDED = "Resource not founded.";
        public const string MESSAGE_CANT_CAST_WHILE_MOVING = "Can not cast while moving.";
        public const string MESSAGE_CANT_CAST_WHILE_TURNING = "Can not cast while turning.";
        public const string MESSAGE_CANT_CAST_WHILE_CASTING_SOMETHING_ELSE = "Can't cast while casting another ability.";
        public const string MESSAGE_CANT_CAST_WHEN_DEAD = "Can't cast when dead.";
        public const string MESSAGE_CANT_CAST_GLOBAL_COOLDOWN = "Ability is on a global cooldown.";
        public const string MESSAGE_CANT_CAST_COOLDOWN = "Ability is on a cooldown.";
        public const string MESSAGE_CANT_CAST_NEED_TARGET = "Ability needs a target.";
        public const string MESSAGE_CANT_CAST_NOT_AVAILABLE = "Ability is not available.";
        public const string MESSAGE_CANT_CAST_NOT_ENOUGH_MANA = "Not enough mana.";
        public const string MESSAGE_CANT_CAST_ON_SELF = "Can't cast this ability on self.";
        public const string MESSAGE_CANT_CAST_ON_ALLY = "Can't cast this ability on friendly unit.";
        public const string MESSAGE_CANT_CAST_ON_ENEMY = "Can't cast this ability on enemy unit.";
        public const string MESSAGE_CANT_CAST_TARGET_IS_NOT_SEEN = "Target is not seen.";
        public const string MESSAGE_CANT_CAST_TARGET_TOO_FAR_AWAY = "Target is too far away.";

        public const float TIME_COMBO_ACTION_AVAILABLE = 9f; //10f

        public const string PLAYER_PREFS_KEY_LIST_ACTION_BARS = "ListActionBars";

        public const float GLOBAL_COOLDOWN_DEFAULT = 2.5f;

        public const float BUFF_TICK_TIME = 3.0f; //each 3 seconds

        public static Color COLOR_DAMAGE = new Color(1, 0.6f, 0, 1);
        public static Color COLOR_HEAL = new Color(0, 0.9f, 0, 1);
        
        public static Color COLOR_AOE_GROUND_INDICATOR = new Color(1, 0.666f, 0, 0.2f);
        
        public const float BLM_PREFERRED_ATTACK_DISTANCE = 6.0f;

        #region "Outline"-properties
        public static Color NEUTRAL_HOVERED_TARGET_OUTLINE_COLOR = new Color(1, 0.95f, 0, 0.2f);
        public static Color NEUTRAL_SELECTED_TARGET_OUTLINE_COLOR = new Color(1, 0.95f, 0, 1);

        public static Color FRIENDLY_HOVERED_TARGET_OUTLINE_COLOR = new Color(0.5f, 1, 1, 0.2f);
        public static Color FRIENDLY_SELECTED_TARGET_OUTLINE_COLOR = new Color(0.5f, 1, 1, 1);

        //public static Color ENEMY_HOVERED_TARGET_OUTLINE_COLOR = new Color(1, 0.95f, 0, 0.2f);
        //public static Color ENEMY_SELECTED_TARGET_OUTLINE_COLOR = new Color(1, 0.95f, 0, 1);

        public static float TARGET_OUTLINE_WIDTH = 0.3f;
        #endregion
    }
}
