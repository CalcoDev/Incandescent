using System;
using Incandescent.GameObjects.Entities;
using UnityEngine;

namespace Incandescent.Managers.GameModes
{
    public class StoryModeContext : MonoBehaviour
    {
        public static StoryModeContext Instance { get; private set; }
        
        [SerializeField] private Player _player;
        
        private void Awake()
        {
            Debug.Log("StoryModeContext: Awake()");
            if (Instance != null)
                Debug.Log("StoryModeContext: Instance already exists! Overwriting...");
            Instance = this;
        }
    }
}