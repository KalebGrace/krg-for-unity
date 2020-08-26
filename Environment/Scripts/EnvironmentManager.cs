namespace KRG
{
    public class EnvironmentManager : Manager
    {
        public override float priority => 500;

        public int CurrentEnvironmentID => G.app.GameplaySceneId;

        public override void Awake() { }
    }
}