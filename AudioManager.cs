using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    private AudioSource EffectsAudioSource;

    [SerializeField]
    private AudioClip PinDropSound;

    [SerializeField]
    private AudioClip NextTurnSound;

    [SerializeField]
    private AudioClip LoseSound;

    [SerializeField]
    private AudioClip WinSound;

    private static AudioManager inst;

    private AudioManager() {

    }

    protected void Awake() {
        inst = this;
    }

    protected void Start () {
		
	}
	
	protected void Update () {
		
	}

    public void PlayPinDropSound() {
        EffectsAudioSource.PlayOneShot(PinDropSound);
    }

    public void PlayNextTurnSound() {
        EffectsAudioSource.PlayOneShot(NextTurnSound);
    }

    public void PlayWinSound() {
        EffectsAudioSource.PlayOneShot(WinSound);
    }

    public void PlayLoseSound() {
        EffectsAudioSource.PlayOneShot(LoseSound);
    }

    public static AudioManager GetAudioManager() {
        return inst;
    }
}
