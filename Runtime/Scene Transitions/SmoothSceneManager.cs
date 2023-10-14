using UnityEngine;

namespace OysterUtils
{
  public static class SmoothSceneManager
  {
    private static GameObject SceneTransitionOverlayPrefab;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
      SceneTransitionOverlayPrefab = Resources.Load<GameObject>("Prefabs/scene-transition-overlay");
    }

    // setupRoutine is executed after the new scene is loaded, but before the fade in transition
    public async static void LoadScene(string toSceneName, Awaitable setup = null)
    {
      GameObject transitionObject = GameObject.Instantiate(SceneTransitionOverlayPrefab, Vector3.zero, Quaternion.identity);
      await transitionObject.GetComponent<SceneTransitionOverlay>().LoadScene(toSceneName, setup);
    }
  }
}