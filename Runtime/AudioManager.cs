using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using jmayberry.SceneTransitions;

namespace jmayberry.FmodHelper {
	public class AudioManager : MonoBehaviour {
		[Header("Preload")]
		[FMODUnity.BankRef] private List<string> banksToLoad;
		public bool waitForAsyncBanks = true;
		public bool waitForSampleData = true;

		public static AudioManager instance { get; private set; }

		private void Awake() {
			if (instance != null) {
				Debug.LogError("Found more than one AudioManager in the scene.");
				Destroy(this.gameObject);
				return;
			}

			DontDestroyOnLoad(gameObject);
			instance = this;

			SceneTransitionManager.instance.preloadOperations.Add(new PreloadFmodBanksOperation(banksToLoad) {
				waitForAsyncBanks = this.waitForAsyncBanks,
				waitForSampleData = this.waitForSampleData,
			});
		}
	}

	// See: https://fmod.com/docs/2.02/unity/examples-async-loading.html
	public class PreloadFmodBanksOperation : LoadOperation {
		[FMODUnity.BankRef] private List<string> banksToLoad;
		public bool waitForAsyncBanks = true;
		public bool waitForSampleData = true;

		public PreloadFmodBanksOperation(List<string> banksToLoad) {
			this.description = "Preloading FMOD Banks";
			this.banksToLoad = banksToLoad ?? new List<string>();
		}

		public override IEnumerator Run(Action callWhenFinished) {
			float totalSteps = (banksToLoad.Count + (waitForAsyncBanks ? 1 : 0) + (waitForSampleData ? 1 : 0));
			if (totalSteps == 0) {
				callWhenFinished();
				yield break;
			}

			// Iterate all the Studio Banks and start them loading in the background including the audio sample data
			float progressStep = 1f / totalSteps;
			if (banksToLoad.Count == 0) {
				foreach (var bankName in banksToLoad) {
					FMODUnity.RuntimeManager.LoadBank(bankName, true);
					progress += progressStep;
					yield return null;
				}
			}

			if (waitForAsyncBanks) {
				// Keep yielding the co-routine until all the bank loading is done (for platforms with asynchronous bank loading)
				while (!FMODUnity.RuntimeManager.HaveAllBanksLoaded) {
					yield return null;
				}
				progress += progressStep;
			}

			if (waitForSampleData) {
				// Keep yielding the co-routine until all the sample data loading is done
				while (FMODUnity.RuntimeManager.AnySampleDataLoading()) {
					yield return null;
				}
				progress += progressStep;
			}

			callWhenFinished();
		}
	}
}