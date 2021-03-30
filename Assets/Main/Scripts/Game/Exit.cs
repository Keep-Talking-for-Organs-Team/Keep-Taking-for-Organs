using UnityEngine;

namespace KeepTalkingForOrgansGame {

    public class Exit : MonoBehaviour {

        public Sprite reachedSprite;

        [Header("REFS")]
        public SpriteRenderer sr;

        Sprite _defaultSprite;

        void Awake () {
            _defaultSprite = sr.sprite;
        }


        public void OnPlayerEnter () {
            sr.sprite = reachedSprite;
        }

        public void OnPlayerExit () {
            sr.sprite = _defaultSprite;
        }

    }
}
