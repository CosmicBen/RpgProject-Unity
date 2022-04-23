using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rpg.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup myCanvasGroup = null;
        private Coroutine fadeCoroutine = null;

        private CanvasGroup MyCanvasGroup
        {
            get
            {
                if (myCanvasGroup == null) { myCanvasGroup = GetComponent<CanvasGroup>(); }
                return myCanvasGroup;
            }
        }

        public Coroutine FadeOut(float time) { return Fade(1.0f, time); }
        public Coroutine FadeIn(float time) { return Fade(0.0f, time); }

        public Coroutine Fade(float target, float time)
        {
            if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); }
            fadeCoroutine = StartCoroutine(FadeRoutine(target, time));
            return fadeCoroutine;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(MyCanvasGroup.alpha, target))
            {
                MyCanvasGroup.alpha = Mathf.MoveTowards(MyCanvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }

            MyCanvasGroup.alpha = target;
        }

        public void FadeOutImmediate()
        {
            MyCanvasGroup.alpha = 1.0f;
        }
    }
}