using UnityEngine;

namespace Incandescent.Managers.GameModes
{
    public class MenuModeContext : MonoBehaviour
    {
        public static MenuModeContext Instance { get; private set; }
        
        private void Awake()
        {
            Debug.Log("MenuModeContext: Awake()");
            if (Instance != null)
                Debug.Log("MenuModeContext: Instance already exists! Overwriting...");
            Instance = this;
        }

        public void StartGame()
        {
            GameModeManager.Instance.EnterStoryMode();
        }
    }
}