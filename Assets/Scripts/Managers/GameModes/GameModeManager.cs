using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Incandescent.Managers.GameModes
{
    public class GameModeManager : MonoBehaviour
    {
        public static GameModeManager Instance { get; private set; }
        
        private IGameMode _current;
        private IGameMode _next;

        [SerializeField] private StoryModeManager _storyMode;
        [SerializeField] private MenuModeManager _menuMode;

        private bool _isChanging;
        public bool IsChanging => _isChanging;
        
        public bool IsEditor { get; private set; }

        private void Awake()
        {
            Debug.Log("GameModeManager: Awake()");

            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
#if UNITY_EDITOR
            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    _current = _menuMode;
                    break;
                default:
                    IsEditor = true;
                    _current = _storyMode;
                    break;
            }
            _current.EditorStart();
#else
            _current = _menuMode;
            _current.Enter();
#endif
        }

        public void EnterStoryMode()
        {
            StartCoroutine(ChangeGameMode(_storyMode));
        }
        
        public IEnumerator ChangeGameMode(IGameMode gameMode)
        {
            _isChanging = true;
            
            if (_current != null)
                yield return _current.Exit();
            
            _current = gameMode;
            yield return _current.Enter();
            
            _isChanging = false;
        }

        public void HandleQuitRequested()
        {
            StartCoroutine(QuitGame());
        }
        
        private IEnumerator QuitGame()
        {
            yield return _current.Exit();
            Application.Quit(0);
        }
    }
}