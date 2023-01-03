using System.Collections;

namespace Incandescent.Managers.GameModes
{
    public interface IGameMode
    {
        IEnumerator Exit();
        IEnumerator Enter();

        void EditorStart();
        
        GameModeState State { get; }
    }
}