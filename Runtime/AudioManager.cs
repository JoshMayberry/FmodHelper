using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

using jmayberry.SceneTransitions;

namespace jmayberry.FmodHelper {
	public class AudioManager : MonoBehaviour {
		[Header("Preload")]
        [SerializeField] [FMODUnity.BankRef] protected List<string> banksToPreLoad;
		public bool waitForAsyncBanks = true;
		public bool waitForSampleData = true;
		public bool autoLoadRootScene = true;
        [SerializeField] internal protected bool persistOnLoad = true;

        public static AudioManager instance { get; protected set; }

		protected virtual void Awake() {
			if (instance != null) {
				Debug.LogError("Found more than one AudioManager in the scene.");
				Destroy(this.gameObject);
				return;
			}

            instance = this;
			if (persistOnLoad) {
                DontDestroyOnLoad(gameObject);
            }

			if (banksToPreLoad.Count > 0) {
				SceneTransitionManager.instance.preloadOperations.Add(new PreloadFmodBanksOperation(banksToPreLoad) {
					waitForAsyncBanks = this.waitForAsyncBanks,
					waitForSampleData = this.waitForSampleData,
				});
			}

			if (autoLoadRootScene) {
				SceneTransitionManager.instance.LoadRoot();
			}
		}
	}
}