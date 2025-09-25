using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System;
using System.Collections.Generic;

namespace DinaFramework.Audio
{
    /// <summary>
    /// Tag pour identifier une chanson.
    /// </summary>
    public sealed class SongTag { }

    /// <summary>
    /// Tag pour identifier un effet sonore.
    /// </summary>
    public sealed class SoundTag { }

    /// <summary>
    /// Gestionnaire de sons et musiques du jeu.
    /// </summary>
    public sealed class SoundManager : IDisposable
    {
        private readonly ContentManager _content;
        private readonly Dictionary<IKey, Song> _songs = [];
        private readonly Dictionary<IKey, SoundEffect> _sounds = [];
        private readonly Dictionary<IKey, SoundEffectInstance> _soundInstances = [];
        private readonly Dictionary<IKey, float> _soundVolumes = [];

        private Song? _currentSong;
        private readonly bool _ownsContent;
        private bool _dispose;

        /// <summary>
        /// La chanson actuellement en cours de lecture.
        /// </summary>
        public Song? CurrentSong => _currentSong;

        /// <summary>
        /// Initialise un nouveau gestionnaire de sons et musiques.
        /// </summary>
        /// <param name="game">Instance du jeu pour accéder au ContentManager.</param>
        /// <param name="contentDirectory">Répertoire de contenu optionnel.</param>
        public SoundManager(Game game, string? contentDirectory = null)
        {
            ArgumentNullException.ThrowIfNull(game, nameof(game));
            if (string.IsNullOrEmpty(contentDirectory))
            {
                _content = game.Content;
                _ownsContent = false;
            }
            else
            {
                _content = new ContentManager(game.Services, contentDirectory);
                _ownsContent = true;
            }
            MediaPlayer.IsRepeating = false;
            ServiceLocator.Register(ServiceKeys.SoundManager, this);
        }

        #region Chargement

        /// <summary>
        /// Charge une chanson depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé unique pour identifier la chanson.</param>
        public void LoadSong(Key<SongTag> key)
        {
            if (!_songs.ContainsKey(key))
                _songs[key] = _content.Load<Song>(key.Value);
        }

        /// <summary>
        /// Charge un effet sonore depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé unique pour identifier l'effet sonore.</param>
        public void LoadSound(Key<SoundTag> key)
        {
            if (!_sounds.ContainsKey(key))
            {
                var sound = _content.Load<SoundEffect>(key.Value);
                _sounds[key] = sound;
                var instance = sound.CreateInstance();
                _soundInstances[key] = instance;
                _soundVolumes[key] = 1f;
                instance.Volume = _soundVolumes[key] * GlobalSoundVolume * MasterVolume;
            }
        }
        #endregion

        #region Play/Stop
        /// <summary>
        /// Joue une chanson. Arrête la chanson précédente si différente.
        /// </summary>
        /// <param name="key">Clé de la chanson à jouer.</param>
        /// <param name="loop">Indique si la chanson doit boucler.</param>
        public void PlaySong(Key<SongTag> key, bool loop = true)
        {
            if (_songs.TryGetValue(key, out var song))
            {
                if (_currentSong != song)
                {
                    StopSong();
                    _currentSong = song;
                    MediaPlayer.IsRepeating = loop;
                    MediaPlayer.Play(song);
                }
            }
        }

        /// <summary>
        /// Arrête la musique en cours.
        /// </summary>
        public void StopSong()
        {
            if (_currentSong != null)
            {
                MediaPlayer.Stop();
                _currentSong = null;
            }
        }

        /// <summary>
        /// Joue un effet sonore.
        /// </summary>
        /// <param name="key">Clé de l'effet sonore à jouer.</param>
        /// <param name="loop">Indique si l'effet doit boucler.</param>
        public void PlaySound(Key<SoundTag> key, bool loop = false)
        {
            if (_soundInstances.TryGetValue(key, out var instance))
            {
                instance.IsLooped = loop;
                instance.Play();
            }
        }

        /// <summary>
        /// Arrête un effet sonore spécifique.
        /// </summary>
        /// <param name="key">Clé de l'effet sonore à arrêter.</param>
        public void StopSound(Key<SoundTag> key)
        {
            if (_soundInstances.TryGetValue(key, out var instance))
                instance.Stop();
        }

        #endregion

        #region Volumes
        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _soundVolume = 1f;

        /// <summary>
        /// Volume maître (0.0 à 1.0) appliqué à la musique et aux effets sonores.
        /// </summary>
        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                _masterVolume = MathHelper.Clamp(value, 0f, 1f);
                UpdateAllVolumes();
            }
        }
        /// <summary>
        /// Volume global de la musique (0.0 à 1.0).
        /// </summary>
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = MathHelper.Clamp(value, 0f, 1f);
                UpdateMusicVolume();
            }
        }
        /// <summary>
        /// Volume global des effets sonores (0.0 à 1.0).
        /// </summary>
        public float GlobalSoundVolume
        {
            get => _soundVolume;
            set
            {
                _soundVolume = MathHelper.Clamp(value, 0f, 1f);
                UpdateSoundVolume();
            }
        }

        /// <summary>
        /// Permet de récupérer le volume d'un son donné.
        /// </summary>
        public float GetSoundVolume(Key<SoundTag> key) => _soundVolumes.ContainsKey(key) ? _soundVolumes[key] : 0;

        /// <summary>
        /// Définit le volume individuel d’un son (0 à 1)
        /// </summary>
        public void SetSoundVolume(Key<SoundTag> key, float volume)
        {
            if (_soundInstances.ContainsKey(key))
            {
                _soundVolumes[key] = MathHelper.Clamp(volume, 0f, 1f);
                _soundInstances[key].Volume = _soundVolumes[key] * _soundVolume * _masterVolume;
            }
        }

        private void UpdateAllVolumes()
        {
            UpdateMusicVolume();
            UpdateSoundVolume();
        }
        private void UpdateMusicVolume()
        {
            MediaPlayer.Volume = _masterVolume * _musicVolume;
        }
        private void UpdateSoundVolume()
        {
            foreach (var key in _soundInstances.Keys)
                _soundInstances[key].Volume = _soundVolumes[key] * _soundVolume * _masterVolume;
        }

        #endregion

        /// <summary>
        /// Décharge tous les sons et chansons et libère le ContentManager associé.
        /// </summary>
        public void Unload()
        {
            StopSong();
            foreach (var instance in _soundInstances.Values)
                instance.Dispose();

            _songs.Clear();
            _sounds.Clear();
            _soundInstances.Clear();
            _soundVolumes.Clear();

            _content.Unload();
        }
        /// <summary>
        /// Libère toutes les ressources utilisées par le SoundManager.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_dispose)
                return;

            if (disposing)
            {
                Unload();
                if (_ownsContent)
                    _content.Dispose();
            }

            _dispose = true;
        }
    }
}