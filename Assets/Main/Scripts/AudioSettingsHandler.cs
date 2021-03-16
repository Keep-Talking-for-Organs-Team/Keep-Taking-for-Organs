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


        void OnEnable () {
            UpdateSliders();
        }

        public void OnValuesChanged () {
            AudioSettings settings = GlobalManager.current.audioSettings;

            settings.masterVolume = masterVolumeSlider.value;
            settings.sfxVolume    = sfxVolumeSlider.value;
            settings.musicVolume  = musicVolumeSlider.value;

            GlobalManager.AssignAudioSettings();
        }


        void UpdateSliders () {
            AudioSettings settings = GlobalManager.current.audioSettings;

            masterVolumeSlider.value = settings.masterVolume;
            sfxVolumeSlider.value    = settings.sfxVolume;
            musicVolumeSlider.value  = settings.musicVolume;
        }

    }
}
