using UnityEngine;

namespace Project.Game.Enemies.Sounds
{

    [RequireComponent(typeof(AudioSource))]
    public class SlimeAudio : MonoBehaviour
    {
        private AudioSource _audioSource;

        public AudioClip[] ChasingStepClips { get; set; }
        public AudioClip DashStartClip { get; set; }
        public AudioClip DashEndClip { get; set; }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }

        /// <summary>
        /// Reproduce un sonido de paso aleatorio.
        /// Ideal para el estado de Chasing.
        /// </summary>
        public void PlayStepSound()
        {
            if (ChasingStepClips == null || ChasingStepClips.Length == 0) return;

            var clip = ChasingStepClips[Random.Range(0, ChasingStepClips.Length)];
        
            _audioSource.PlayOneShot(clip);
        }

        /// <summary>
        /// Reproduce el sonido de inicio del dash.
        /// </summary>
        public void PlayDashStart()
        {
            if (DashStartClip == null) return;
            _audioSource.PlayOneShot(DashStartClip);
        }

        /// <summary>
        /// Reproduce el sonido de fin del dash (aterrizaje).
        /// </summary>
        public void PlayDashEnd()
        {
            if (DashEndClip == null) return;
            _audioSource.PlayOneShot(DashEndClip);
        }
    }
}