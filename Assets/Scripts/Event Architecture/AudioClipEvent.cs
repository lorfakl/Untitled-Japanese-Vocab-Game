using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Utilities.Events 
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipEvent : MonoBehaviour
    {
        [SerializeField]
        AudioClip clip;

        AudioSource _audioSource;

        public void PlayAudioClip()
        {
            _audioSource.clip = clip;   
            _audioSource.PlayOneShot(_audioSource.clip);
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
        }
    }
}


