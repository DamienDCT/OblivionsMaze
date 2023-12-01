using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    // Tableau de sons à jouer (qui ne sont pas en boucle)
    [SerializeField]
    private Sound[] sounds;
    // Mixer de son pour la musique
    [SerializeField]
    private AudioMixerGroup musicMixerGroup;
    // Mixer de son pour les effets spéciaux
    [SerializeField]
    private AudioMixerGroup soundEffectMixerGroup;
    // Mixer de son pour le son général
    [SerializeField]
    private AudioMixerGroup generalMixerGroup;

    // Singleton
    public static AudioManager instance;

    private void Awake()
    {
        if(instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        // Pour chaque son, on ajoute un composant AudioSource, que l'on modifie selon les paramètres de chaque son
        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.clip = s.clip;
            s.source.loop = s.isLoop;
            s.source.playOnAwake = s.playOnAwake;
            s.isPlaying = false;

            // Choix du mixer sur lequel le son doit se jouer
            switch(s.audioType){
                case Sound.AudioTypes.soundEffect:
                    s.source.outputAudioMixerGroup = soundEffectMixerGroup;
                    break;
                case Sound.AudioTypes.music:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    break;

            }

            // Si on doit jouer le son directement, on le lance
            if(s.playOnAwake){
                s.source.Play();
            }
        }
    }


    // Pour jouer un son, on le recherche par son nom et on le lance s'il existe
    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) return;
        s.source.Play();
    }

    // Pour jouer un son en boucle, on le recherche par son nom et on le lance s'il existe et qu'il est pas déjà lancé
    public void PlayLoop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) return;
        if(s.isPlaying){
            return;
        }
        s.isPlaying = true;
        s.source.Play();
    }


    // Pour arrêter un son, on le recherche par son nom et on le stop s'il existe et qu'il est déjà lancé
    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) return;
        if(s.isPlaying)
            s.isPlaying = false;
        s.source.Stop();
    }

    // Méthode pour modifier les mixeurs audios en fonction des paramètres choisi par le joueur
    public void UpdateMixerValues(float generalValue, float musicValue, float soundEffectValue){
        generalMixerGroup.audioMixer.SetFloat("GeneralVolume", Mathf.Log10(generalValue)*20);
        musicMixerGroup.audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicValue)*20);
        soundEffectMixerGroup.audioMixer.SetFloat("EffectVolume",  Mathf.Log10(soundEffectValue)*20);
    }
}
