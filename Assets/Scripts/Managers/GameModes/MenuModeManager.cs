using System.Collections;
using UnityEngine;

namespace Incandescent.Managers.GameModes
{
    public class MenuModeManager : MonoBehaviour, IGameMode
    {
        [SerializeField] private GameModeState _state;
        public GameModeState State => _state;

        public IEnumerator Enter()
        {
            if (_state != GameModeState.Exited)
                yield break;
            
            Debug.Log("MenuModeManager: Enter()");
            _state = GameModeState.Entering;
            _state = GameModeState.Entered;
        }

        public IEnumerator Exit()
        {
            if (_state != GameModeState.Entered)
                yield break;
            
            Debug.Log("MenuModeManager: Exit()");
            _state = GameModeState.Exiting;
            _state = GameModeState.Exited;
        }

        public void EditorStart()
        {
            Debug.Log("MenuModeManager: EditorStart()");
            _state = GameModeState.Entered;
        }
    }
}