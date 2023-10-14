using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OysterUtils
{
  // The scene loading coroutine must be done on this monobehaviour
  // because it will persist across scenes.  If we were to start the scene load
  // coroutine from the trigger object, the coroutine would die right when the new scene loads
  // and the old scene unloads.
  public class SceneTransitionOverlay : MonoBehaviour
  {
    [SerializeField] private float transitionDuration = 0.5f;
    private Animator animator;

    private void Awake()
    {
      animator = GetComponent<Animator>();
      DontDestroyOnLoad(gameObject);
    }

    private async Task LoadSceneAsync(string name)
    {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(name);
      while (!asyncLoad.isDone)
      {
        await Awaitable.EndOfFrameAsync();
      }
    }

    public async Awaitable LoadScene(string toSceneName, Awaitable setupRoutine)
    {
      // Let the transition in animation play
      animator.SetBool("SceneVisible", false);
      await Awaitable.WaitForSecondsAsync(transitionDuration);

      // Load the new scene first, so the setup routine can operate on it properly
      await LoadSceneAsync(toSceneName);
      if (setupRoutine != null)
      {
        await setupRoutine;
      }

      // Let the transition out animation play
      animator.SetBool("SceneVisible", true);
      await Awaitable.WaitForSecondsAsync(transitionDuration);
      Destroy(gameObject);
    }
  }
}


