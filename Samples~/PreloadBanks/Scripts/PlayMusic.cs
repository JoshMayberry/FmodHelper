using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jmayberry.FmodHelper.Sample.PreloadBanks {
    public class PlayMusic : MonoBehaviour {
        [field: SerializeField] public EventReference musicEventReference { get; private set; }
        private EventInstance musicEventInstance;

        void Start() {
            InitializeMusic();
        }

        private void InitializeMusic() {
            this.musicEventInstance = RuntimeManager.CreateInstance(musicEventReference);
            this.musicEventInstance.start();
        }
    }
}