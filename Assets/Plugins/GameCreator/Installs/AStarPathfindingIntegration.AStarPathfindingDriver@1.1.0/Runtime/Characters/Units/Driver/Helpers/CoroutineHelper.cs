using System.Collections;
using UnityEngine;

namespace Arawn.Runtime.AStarPathfinding
{
    public class CoroutineHelper : MonoBehaviour
    {
        private static CoroutineHelper _coroutineRunner;

        private static CoroutineHelper CoroutineRunner
        {
            get
            {
                if (_coroutineRunner == null)
                {
                    var runnerObject = new GameObject("CoroutineHelperRunner");
                    UnityEngine.Object.DontDestroyOnLoad(runnerObject);
                    _coroutineRunner = runnerObject.AddComponent<CoroutineHelper>();
                }

                return _coroutineRunner;
            }
        }

        public static Coroutine Start(IEnumerator coroutine)
        {
            return CoroutineRunner.StartCoroutine(coroutine);
        }

        public static void Stop(Coroutine coroutine)
        {
            CoroutineRunner.StopCoroutine(coroutine);
        }
    }
}