namespace DinaFramework.Scenes
{
    public abstract class LoadingScene : Scene
    {
        private float _progress;
        public float Progress { get => _progress; set => _progress = value; }
        protected LoadingScene(SceneManager sceneManager) : base(sceneManager)
        {
        }
    }
}
