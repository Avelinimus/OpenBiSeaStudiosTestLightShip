using Game.UI;
using Injection;
using MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Core.Managers
{
    public abstract class BaseAudioManager : IDisposable
    {
        protected const int kMusicUIChannels = 3;
        protected const int kMusicChannels = 1;
        protected const int kUIChannels = 32;
        private const string kSoundsPath = "Sounds/";

        [Inject] protected GameView _gameView;

        private readonly List<AudioSource> _audioSources;
        private bool _muteMusic;
        private bool _muteSounds;
        private float _volumeMusic;
        private float _volumeSounds;

        public virtual bool IsMutted
        {
            set
            {
                foreach (var audioSource in _audioSources)
                {
                    audioSource.mute = value;
                }
            }
        }

        public virtual bool MuteMusic
        {
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source.loop)
                    {
                        source.mute = value;
                    }
                }

                _muteMusic = value;
            }
            get => _muteMusic;
        }

        public virtual float VolumeMusic
        {
            set
            {
                foreach (var source in _audioSources)
                {
                    if (source.loop)
                    {
                        source.volume = value;
                    }
                }

                _volumeMusic = value;
            }
            get => _volumeMusic;
        }

        public virtual bool MuteSounds
        {
            set
            {
                foreach (var source in _audioSources)
                {
                    if (!source.loop)
                    {
                        source.mute = value;
                    }
                }

                _muteSounds = value;
            }
            get => _muteSounds;
        }

        public virtual float VolumeSounds
        {
            set
            {
                foreach (var source in _audioSources)
                {
                    if (!source.loop)
                    {
                        source.volume = value;
                    }
                }

                _volumeSounds = value;
            }
            get => _volumeSounds;
        }

        protected BaseAudioManager()
        {
            _audioSources = new List<AudioSource>();
        }

        protected void Initialize(AudioSource[] components)
        {
            _audioSources.AddRange(components);

            for (int i = 0; i < kMusicUIChannels; i++)
            {
                _audioSources[i].loop = true;
            }

            for (int i = 0; i < kMusicUIChannels; i++)
            {
                var audioSource = _audioSources[i + kMusicUIChannels];
                audioSource.loop = true;
                audioSource.spatialBlend = 1;
                audioSource.rolloffMode = AudioRolloffMode.Custom;
                audioSource.maxDistance = 25;
            }
        }

        public virtual void Dispose()
        {
            _audioSources.Clear();
        }

        public void PlaySound(SoundType type)
        {
            var channel = GetFreeUISoundChannel();

            if (null == channel)
                return;

            channel.clip = Preload(type);
            Log.Info(Channel.Audio, type);
            channel.Play();
        }

        public void PlayMusic(SoundType type)
        {
            var channel = GetFreeMusicUIChannel();

            if (null == channel)
            {
                channel = _audioSources.FirstOrDefault(temp => temp.loop);
            }

            channel.clip = Preload(type);
            Log.Info(Channel.Audio, type);
            channel.Play();
        }

        public void PlayMusic(SoundType type, Vector3 position)
        {
            var channel = GetFreeMusicChannel();

            if (null == channel)
                return;

            channel.transform.position = position;
            channel.clip = Preload(type);
            channel.Play();
        }


        public bool IsPlay(SoundType type)
        {
            var isPlay = false;
            var soundName = type.ToString();

            foreach (var audioSource in _audioSources)
            {
                if (audioSource.isPlaying && audioSource.clip.name.Equals(soundName))
                {
                    isPlay = true;
                }
            }

            return isPlay;
        }

        public void Stop(SoundType type)
        {
            var soundName = type.ToString();

            foreach (var audioSource in _audioSources)
            {
                if (null != audioSource.clip && audioSource.clip.name.Equals(soundName))
                {
                    audioSource.Stop();
                    audioSource.transform.position = Vector3.zero;
                }
            }
        }

        public void StopAll()
        {
            foreach (var audioSource in _audioSources)
            {
                audioSource.Stop();
            }
        }

        public AudioClip Preload(SoundType type)
        {
            return Resources.Load<AudioClip>(kSoundsPath + type);
        }

        private AudioSource GetFreeUISoundChannel()
        {
            return _audioSources.FirstOrDefault(temp =>
                !temp.loop && temp.spatialBlend <= 0 && (temp.clip == null || !temp.isPlaying));
        }

        private AudioSource GetFreeMusicChannel()
        {
            return _audioSources.FirstOrDefault(temp =>
                temp.loop && temp.spatialBlend >= 1 && (temp.clip == null || !temp.isPlaying));
        }

        private AudioSource GetFreeMusicUIChannel()
        {
            return _audioSources.FirstOrDefault(temp => temp.loop && (temp.clip == null || !temp.isPlaying));
        }
    }

    public enum SoundType
    {
        None,
    }

    public sealed class AudioManager : BaseAudioManager
    {
        private const string kName = "AudioListener";

        private readonly List<GameObject> _goAudious;
        private List<AudioSource> _additionalSoundSources;
        private List<AudioSource> _additionalMusicSources;
        private AudioListener _audioListener;

        public override bool MuteSounds
        {
            set
            {
                base.MuteSounds = value;

                foreach (var source in _additionalSoundSources)
                {
                    source.mute = value;
                }
            }
            get => base.MuteSounds;
        }

        public override float VolumeSounds
        {
            set
            {
                base.VolumeSounds = value;

                foreach (var source in _additionalSoundSources)
                {
                    source.volume = value;
                }
            }
            get => base.VolumeSounds;
        }

        public override bool MuteMusic
        {
            set
            {
                base.MuteMusic = value;

                foreach (var source in _additionalMusicSources)
                {
                    source.mute = value;
                }
            }
            get => base.MuteMusic;
        }

        public override float VolumeMusic
        {
            set
            {
                base.VolumeMusic = value;

                foreach (var source in _additionalMusicSources)
                {
                    source.volume = value;
                }
            }
            get => base.VolumeMusic;
        }

        public AudioManager()
        {
            _goAudious = new List<GameObject>();
            _additionalMusicSources = new List<AudioSource>();
            _additionalSoundSources = new List<AudioSource>();
        }

        public void Initialize()
        {
            var sources = new AudioSource[kMusicChannels + kUIChannels];

            for (int i = 0; i < kMusicChannels + kUIChannels; i++)
            {
                var goAudio = new GameObject(GetType().Name + (i + 1));
                goAudio.transform.SetParent(_gameView.transform);
                _goAudious.Add(goAudio);
                sources[i] = goAudio.AddComponent<AudioSource>();
            }

            var audioListener = new GameObject(kName);
            _audioListener = audioListener.AddComponent<AudioListener>();
            SetAudioListenerParentDefault();
            Initialize(sources);
        }

        public void SetAudioListenerParent(Transform parent)
        {
            _audioListener.transform.SetParent(parent);
        }

        public void SetAudioListenerParentDefault()
        {
            _audioListener.transform.SetParent(_gameView.transform);
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (var goAudio in _goAudious)
            {
                GameObject.Destroy(goAudio);
            }

            _goAudious.Clear();
            GameObject.Destroy(_audioListener);
        }
    }
}