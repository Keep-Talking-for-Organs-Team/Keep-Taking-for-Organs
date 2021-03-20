using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

using DoubleHeat;

namespace KeepTalkingForOrgansGame {

    public class AudioSettingsHandler : MonoBehaviour {

        [Header("REFS")]
        public Slider masterVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider musicVolumeSlider;

        void Awake () {
            UpdateSliders();
        }

        void OnEnable () {
            UpdateSliders();
        }

        public void OnValuesChanged (int index) {

            AudioSettings settings = GlobalManager.current.audioSettings;

            if (index == 0)
                settings.masterVolume = masterVolumeSlider.value;
            else if (index == 1)
                settings.sfxVolume    = sfxVolumeSlider.value;
            else if (index == 2)
                settings.musicVolume  = musicVolumeSlider.value;

            GlobalManager.AssignAudioSettings(index);
        }


        void UpdateSliders () {
            AudioSettings settings = GlobalManager.current.audioSettings;

            masterVolumeSlider.value = settings.masterVolume;
            sfxVolumeSlider.value    = settings.sfxVolume;
            musicVolumeSlider.value  = settings.musicVolume;
        }

    }
}
