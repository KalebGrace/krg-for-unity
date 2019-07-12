namespace KRG
{
    public class PlayerCharacter : Character
    {
        public static Character instance; //a temporary solution

        public const string TAG = "Player";

        public override CharacterType Type => CharacterType.PlayerCharacter;
    }
}
