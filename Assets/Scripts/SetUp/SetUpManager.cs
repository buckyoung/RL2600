using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace RL2600.SetUp {
	public class SetUpManager : MonoBehaviour {
		public void changeScene(int scene) {
			SceneManager.LoadScene(scene);
		}
	}
}
