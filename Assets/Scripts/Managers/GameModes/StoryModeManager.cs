using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Incandescent.Managers.GameModes
{
    public class StoryModeManager : MonoBehaviour, IGameMode
    {
        [SerializeField] private GameModeState _state;
        public GameModeState State => _state;

        private string _activeScene;

        public IEnumerator Enter()
        {
            Debug.Log("StoryModeManager: Enter()");
            
            if (_state != GameModeState.Exited)
                yield break;
            
            _state = GameModeState.Entering;
            
            // INFO(calco): Aarthificial does this because he has a save system which stores the last scene the player was in.
            // I don't have that, therefore I just load the first scene in the build settings.
            // If you have a save system, you can just load the last scene the player was in.
            
            // if (!SceneManager.GetSceneByPath(_activeScene).isLoaded)
            //     yield return SceneManager.LoadSceneAsync(_activeScene, LoadSceneMode.Additive);
            _activeScene = SceneUtility.GetScenePathByBuildIndex(1);
            
            yield return SceneManager.LoadSceneAsync(_activeScene, LoadSceneMode.Single);
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(_activeScene));

            _state = GameModeState.Entered;
        }
        
        public IEnumerator Exit()
        {
            Debug.Log("StoryModeManager: Exit()");
            
            if (_state != GameModeState.Entered)
                yield break;
            
            _state = GameModeState.Exiting;
            // Do some stuff lmao
            _state = GameModeState.Exited;
        }
        
        public void EditorStart()
        {
            Debug.Log("StoryModeManager: EditorStart()");
            _activeScene = SceneManager.GetActiveScene().path;
            _state = GameModeState.Entered;
        }
    }
}