namespace KRG
{
    public static class CharacterTypeEM
    {
        public static bool IsPlayerCharacter(this CharacterType characterType)
        {
            return characterType == CharacterType.PlayerCharacter;
        }

        public static bool IsBoss(this CharacterType characterType)
        {
            return characterType == CharacterType.Boss;
        }
        
        public static bool IsEnemy(this CharacterType characterType)
        {
            return characterType == CharacterType.Enemy;
        }

        public static bool IsEnemyOrBoss(this CharacterType characterType)
        {
            switch (characterType)
            {
                case CharacterType.Enemy:
                case CharacterType.Boss:
                    return true;
            }
            return false;
        }

        public static bool IsNPC(this CharacterType characterType)
        {
            return characterType == CharacterType.NonPlayerCharacter;
        }

        public static string ToTag(this CharacterType characterType)
        {
            // direct cast from CharacterType enum to CharacterTag enum
            CharacterTag characterTag = (CharacterTag)characterType;

            return characterTag.ToString();
        }
    }
}
