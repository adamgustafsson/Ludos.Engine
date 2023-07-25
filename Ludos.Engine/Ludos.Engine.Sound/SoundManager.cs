namespace Ludos.Engine.Sound
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;

    public class SoundManager
    {
        private List<SoundEffect> _soundEffects;
        private List<Song> _soundTracks;
        private int _activeTrack;

        public SoundManager(List<SoundEffect> soundEffects, List<Song> soundTracks)
        {
            _soundEffects = soundEffects;
            _soundTracks = soundTracks;
        }

        public SoundManager(ContentManager content, SoundInfo soundInfo)
        {
            LoadContent(content, soundInfo);
        }

        public bool SoundEnabled { get; set; } = true;
        public bool MusicEnabled { get; set; } = true;

        public void LoadContent(ContentManager content, SoundInfo soundInfo)
        {
            if (string.IsNullOrWhiteSpace(soundInfo.SoundEffectsPath) || string.IsNullOrWhiteSpace(soundInfo.SoundEffectsPath))
            {
                throw new Exception("Invalid path.");
            }

            _soundEffects = new List<SoundEffect>();
            _soundTracks = new List<Song>();

            foreach (var title in soundInfo.SoundEffectTitles)
            {
                _soundEffects.Add(content.Load<SoundEffect>(string.Format("{0}/{1}", soundInfo.SoundEffectsPath, title)));
            }

            foreach (var title in soundInfo.SoundTracksTitles)
            {
                _soundTracks.Add(content.Load<Song>(string.Format("{0}/{1}", soundInfo.SoundTracksPath, title)));
            }
        }

        public void PlaySound(int soundEffectIndex, float volumne = 0.4f)
        {
            if (!SoundEnabled)
            {
                return;
            }

            _soundEffects[soundEffectIndex].Play(volumne, 0f, 0f);
        }

        public void PlaySound(string soundEffectName, float volumne = 0.4f)
        {
            if (!SoundEnabled)
            {
                return;
            }

            _soundEffects.Where(x => x.Name.Equals(soundEffectName)).FirstOrDefault().Play(volumne, 0f, 0f);
        }

        public void PlaySoundTrack(int soundTrackIndex, float volumne = 0.4f)
        {
            if (!MusicEnabled)
            {
                return;
            }

            if (!IsPlayingSong(soundTrackIndex))
            {
                PlaySoundTrack(_soundTracks[soundTrackIndex], volumne);
            }
        }

        public void StopSoundTrack()
        {
            if (MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Stop();
            }
        }

        public void PlaySoundTrack(string soundTrackName, float volumne = 0.4f)
        {
            if (!MusicEnabled)
            {
                return;
            }

            var soundTrack = _soundTracks.Where(x => x.Name.Equals(soundTrackName)).FirstOrDefault();

            if (!IsPlayingSong(_soundTracks.IndexOf(soundTrack)))
            {
                PlaySoundTrack(soundTrack, volumne);
            }
        }

        private void PlaySoundTrack(Song soundTrack, float volumne)
        {
            MediaPlayer.Stop();
            MediaPlayer.Volume = volumne;
            MediaPlayer.Play(soundTrack);
            _activeTrack = _soundTracks.IndexOf(soundTrack);
        }

        private bool IsPlayingSong(int soundTrackIndex)
        {
            return MediaPlayer.State == MediaState.Playing && _activeTrack == soundTrackIndex;
        }
    }
}