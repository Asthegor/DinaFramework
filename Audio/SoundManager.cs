using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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
    public class SoundManager
    {
        private readonly ContentManager _content;
        private readonly Dictionary<IKey, Song> _songs = new();
        private readonly Dictionary<IKey, SoundEffect> _sounds = new();
        private readonly Dictionary<IKey, SoundEffectInstance> _soundInstances = new();
        private Song? _currentSong;

        /// <summary>
        /// Volume global de la musique (0.0 à 1.0).
        /// </summary>
        public static float MusicVolume
        {
            get => MediaPlayer.Volume;
            set => MediaPlayer.Volume = MathHelper.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Volume global des effets sonores (0.0 à 1.0).
        /// </summary>
        public float SoundVolume { get; set; } = 1f;

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
            _content = string.IsNullOrEmpty(contentDirectory) ? game.Content : new ContentManager(game.Services, contentDirectory);
            MediaPlayer.IsRepeating = false;
        }

        #region Chargement

        /// <summary>
        /// Charge une chanson depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé unique pour identifier la chanson.</param>
        /// <param name="assetName">Nom de l'asset dans le ContentManager.</param>
        public void LoadSong(Key<SongTag> key, string assetName)
        {
            if (!_songs.ContainsKey(key))
                _songs[key] = _content.Load<Song>(assetName);
        }

        /// <summary>
        /// Charge un effet sonore depuis le ContentManager.
        /// </summary>
        /// <param name="key">Clé unique pour identifier l'effet sonore.</param>
        /// <param name="assetName">Nom de l'asset dans le ContentManager.</param>
        public void LoadSound(Key<SoundTag> key, string assetName)
        {
            if (!_sounds.ContainsKey(key))
            {
                var sound = _content.Load<SoundEffect>(assetName);
                _sounds[key] = sound;
                var instance = sound.CreateInstance();
                instance.Volume = SoundVolume;
                _soundInstances[key] = instance;
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
                }
                MediaPlayer.IsRepeating = loop;
                MediaPlayer.Play(song);
            }
        }

        /// <summary>
        /// Arrête la musique en cours.
        /// </summary>
        public static void StopSong() => MediaPlayer.Stop();

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
            _content.Unload();
        }
    }
}