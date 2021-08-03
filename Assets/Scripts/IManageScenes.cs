using UnityEngine.SceneManagement;

namespace DarkKey
{
    public interface IManageScenes
    {
        public void SwitchScene(Scene scene);

        public void SwitchToOfflineScene();
    }
}