using UnityEngine;

namespace KeepTalkingForOrgansGame {

    [System.Serializable]
    public class AudioSettings {

        public float masterVolume;
        public float sfxVolume;
        public float musicVolume;

        public AudioSettings () {
            masterVolume = 0.8f;
            sfxVolume = 0.8f;
            musicVolume = 0.8f;
        }

    }
}
