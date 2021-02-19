namespace _0G.Legacy
{
    public enum CharacterID
    {
        None = 0,

        /* EXAMPLE:
        
        // PLAYER CHARACTERS
        Mario = 1,

        // BOSSES
        Bowser = 10,

        // ENEMIES
        KoopaTroopa = 100,

        // NON-PLAYER CHARACTERS
        Toad = 1000,
        */
    }

    public enum CharacterAssetLabel
    {
        // MUST MATCH ORDER IN CharacterTag & CharacterType

        Unlabelled = 0,
        Player = 1,
        Npc = 2,
        Enemy = 3,
        Boss = 4,
        // N/A
    }

    public enum CharacterTag
    {
        // MUST MATCH ORDER IN CharacterAssetLabel & CharacterType

        Untagged = 0,
        Player = 1,
        NPC = 2,
        Enemy = 3,
        Boss = 4,
        Animation = 5,
    }

    public enum CharacterType
    {
        // MUST MATCH ORDER IN CharacterAssetLabel & CharacterTag

        None = 0,
        PlayerCharacter = 1,
        NonPlayerCharacter = 2,
        Enemy = 3,
        Boss = 4,
        // N/A
    }

    public static class CharacterEnums
    {
        public static string ToAssetLabel(this CharacterType characterType)
        {
            // direct cast from CharacterType enum to CharacterAssetLabel enum
            CharacterAssetLabel characterAssetLabel = (CharacterAssetLabel) characterType;

            return characterAssetLabel.ToString();
        }

        public static string ToTag(this CharacterType characterType)
        {
            // direct cast from CharacterType enum to CharacterTag enum
            CharacterTag characterTag = (CharacterTag) characterType;

            return characterTag.ToString();
        }
    }
}