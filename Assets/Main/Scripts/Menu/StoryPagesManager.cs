using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;
using DoubleHeat.Utilities;

namespace KeepTalkingForOrgansGame {

    public class StoryPagesManager : MonoBehaviour {

        public static bool hasPlayed = false;

        [Header("Properties")]
        public float durationPerPage = 1f;

        [Header("REFS")]
        public Image pageImg;


        Sprite[]  _pageSprites;
        Coroutine _currentPlayingStoryLine;
        bool      _isEnded = false;

        void Awake () {
            _pageSprites = Resources.LoadAll<Sprite>("Sprites/Story");
        }

        void OnEnable () {
            Play();
        }

        void OnDisable () {
            if (_currentPlayingStoryLine != null)
                StopCoroutine(_currentPlayingStoryLine);

            _currentPlayingStoryLine = null;
        }

        void Update () {
            if (_isEnded)
                return;

            if (Input.anyKeyDown) {

                bool isMouseButton = false;

                for (int i = 0 ; i <= 2; i++) {
                    if (Input.GetMouseButtonDown(i)) {
                        isMouseButton = true;
                    }
                }

                if (!isMouseButton)
                    Skip();
            }
        }


        public void Play () {
            _isEnded = false;

            if (_currentPlayingStoryLine != null)
                StopCoroutine(_currentPlayingStoryLine);

            _currentPlayingStoryLine = StartCoroutine(StoryLine());

            hasPlayed = true;
        }

        public void Skip () {
            if (_currentPlayingStoryLine != null)
                StopCoroutine(_currentPlayingStoryLine);

            GlobalManager.current.PostAudioEvent("Play_Tab");

            OnEnded();
        }


        IEnumerator StoryLine () {

            int   currentPageIndex = 0;
            float pageStartRealtime = Time.realtimeSinceStartup;

            if (_pageSprites.Length > 0) {
                pageImg.sprite = _pageSprites[0];
            }

            while (true) {

                yield return null;

                if (Time.realtimeSinceStartup - pageStartRealtime > durationPerPage) {
                    currentPageIndex++;

                    if (currentPageIndex < _pageSprites.Length) {

                        pageImg.sprite = _pageSprites[currentPageIndex];
                        pageStartRealtime = Time.realtimeSinceStartup;
                    }
                    else {
                        break;
                    }
                }
            }

            OnEnded();
        }

        void OnEnded () {
            _isEnded = true;

            _currentPlayingStoryLine = null;

            if (MenuSceneManager.current != null)
                MenuSceneManager.current.OnStoryPagesEnded();
        }

    }
}
