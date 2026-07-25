using System.Collections;
using UnityEngine;

namespace CrosswalkGame
{
    [System.Serializable]
    public class GameTimer
    {
        public GameTimer(float time)
        {
            MaxTime = time;
        }

        public float ElapsedTime { get; private set; } = 0f;
        public float MaxTime { get; private set; } = 0f;

        private Coroutine runCoroutine = null;
        public bool IsRunning => runCoroutine != null && ElapsedTime < MaxTime;

        public void Run()
        {
            if (runCoroutine != null)
                GameManager.instance.StopCoroutine(runCoroutine);

            runCoroutine = GameManager.instance.StartCoroutine(RunRoutine());
            IEnumerator RunRoutine()
            {
                while (ElapsedTime < MaxTime)
                {
                    ElapsedTime += Time.deltaTime;
                    yield return null;
                }

                runCoroutine = null;
            }
        }

        public void Rewind()
        {
            ElapsedTime = 0f;

            if (runCoroutine != null)
                GameManager.instance.StopCoroutine(runCoroutine);
            
            runCoroutine = null;
        }
    }
}
