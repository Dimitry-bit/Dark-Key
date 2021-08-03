using UnityEngine.SceneManagement;

namespace DarkKey.Core
{
    public interface IManageScenes
    {
        public void SwitchScene(Scene scene);

        public void SwitchToOfflineScene();

        public void SwitchToOnlineScene();
    }
}