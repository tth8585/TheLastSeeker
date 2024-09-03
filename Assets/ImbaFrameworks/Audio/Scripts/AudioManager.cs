/*
Kaka @ 2020
Audio manager: manage all sound in game
TODO: Pooling audio source
*/


using Imba.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Imba.Audio
{
    public class AudioManager : ManualSingletonMono<AudioManager>
	{
        //driven SFX engine
        public event Action OnChangeSFX;
        #region VARIABLES

        private const float TIME_TO_CHECK_IDLE_AUDIO_SOURCE = 5f;

		//TODO: Move this to ScriptableObject
		public List<AudioData> _database;

		private float _timeToReset;

		private float _timeToCheckIdleAudioSource;

		private bool _timerIsSet;

		private AudioName _tmpName;

		private float _tmpVol;

		private bool _isLowered;

		private bool _fadeOut;

		private bool _fadeIn;

		private string _fadeInUsedString;

		private string _fadeOutUsedString;

		private bool _isMuteMusic;

		private bool _isMuteSfx;

		// Vibration
		public bool IsVibrationOff;
		#endregion

		#region UNITY METHOD


		void Reset()
		{
			_database = new List<AudioData>
			{
				new AudioData()
			};
		}

		
		// Use this for initialization
		public override void Awake()
		{
			base.Awake();
			//if(PlayerPrefs.HasKey("MuteMusic"))
			//	_isMuteMusic = PlayerPrefs.GetInt("MuteMusic") == 1;
			if (PlayerPrefs.HasKey("MuteMusic"))
				_isMuteMusic = PlayerPrefs.GetInt("MuteMusic") == 1 ? true : false;
			//  if (PlayerPrefs.HasKey("MuteSFX"))
			//_isMuteSfx = PlayerPrefs.GetInt("MuteSFX") == 1;
			if (PlayerPrefs.HasKey("MuteSFX"))
				_isMuteSfx = PlayerPrefs.GetInt("MuteSFX") == 1 ? true : false;

			// Vibration, de tam o day, voi cac chuc nang co dung vibration co the goi vo day check neu false thi ko cho rung, true thi cho rung
			if (PlayerPrefs.HasKey("VibrationOff"))
				IsVibrationOff = PlayerPrefs.GetInt("VibrationOff") == 1 ? true : false;


            foreach (var s in _database)
			{

				if (s.PlayOnAwake)
				{
					s.Source = CreateAudioSource(s);
					if (IsMuteAudio(s.Type))
					{
						s.Source.mute = true;
					}

					s.Source.Play();
				}
			}
		}
		
		void OnEnable()
		{
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
       
		}

		void OnDisable()
		{
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}

		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			AudioManager.Instance.FadeIn(GetMusicBGName(scene.name), 1f);
		}

		void OnSceneUnloaded(Scene scene)
		{
			AudioManager.Instance.StopMusic(GetMusicBGName(scene.name));
		}
		
		
		//TODO: move to game logic
		AudioName GetMusicBGName(string currentScene)
		{
			switch (currentScene)
			{
			
				case "Login":
					return AudioName.Track_1;
				case "SceneHome":
					return AudioName.Track_1;
				case "Hometown":
					return AudioName.Track_1;
				case "Academy":
					return AudioName.Track_1;
				default:
					return AudioName.NoSound;
			}

			//Debug.LogError("Need Setup Music for " + currentScene);

			return AudioName.NoSound;
		}



		#endregion

		#region CLASS METHODS

		private AudioSource CreateAudioSource(AudioData a)
		{
			AudioSource s = Instance.gameObject.AddComponent<AudioSource>();
			s.clip = a.AudioClip;
			s.volume = a.Volume;
			s.playOnAwake = a.PlayOnAwake;
			s.priority = a.Priority;
			s.loop = a.IsLooping;
			return s;
		}

		private bool IsMuteAudio(AudioType type)
		{
			if (type == AudioType.BGM && _isMuteMusic) return true;
			if (type == AudioType.SFX && _isMuteSfx) return true;

			return false;
		}

		private AudioData GetAudioData(AudioName audioName)
		{
			AudioData s = _database.Find(a => a.AudioName == audioName);

			return s;
		}

		public void PlaySFX(AudioName audioName)
		{
			if (_isMuteSfx || audioName == AudioName.NoSound) return;

			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
			}
			else
			{
				if (s.Source == null)
				{
					s.Source = CreateAudioSource(s);
				}

				s.Source.PlayOneShot(s.AudioClip, s.Volume);
			}
		}

		public void PlaySoundFromSource(AudioName audioName, AudioSource audioSource, bool isChangeSound = false)
		{
			if (_isMuteSfx || audioName == AudioName.NoSound) return;

			if (audioSource == null) return;

			if (audioSource.clip == null || isChangeSound)
			{
				AudioData s = GetAudioData(audioName);
				if (s == null)
				{
					Debug.LogError("Sound name" + audioName + "not found!");
					return;
				}

				if (audioSource.clip == null)
				{
					audioSource.clip = s.AudioClip;
					audioSource.volume = s.Volume;
					audioSource.priority = s.Priority;
					audioSource.playOnAwake = s.PlayOnAwake;
					audioSource.spatialBlend = s.SpatialBlend;
					audioSource.rolloffMode = AudioRolloffMode.Linear;
					audioSource.minDistance = 1;
					audioSource.maxDistance = 50;
				}
			}

			audioSource.PlayOneShot(audioSource.clip, audioSource.volume);

		}


		public void PlayMusic(AudioName audioName)
		{
			if (_isMuteMusic || audioName == AudioName.NoSound) return;

			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
				return;
			}

			if (s.Source == null)
			{
				s.Source = CreateAudioSource(s);
			}

			if (!s.Source.isPlaying)
			{
				s.Source.Play();
			}
		}

		public void MuteMusic()
		{
			if (PlayerPrefs.HasKey("MuteMusic"))
			{
				_isMuteMusic = PlayerPrefs.GetInt("MuteMusic") == 1 ? true : false;
				foreach (var s in _database)
				{
					if (s.AudioClip == null)
						continue;
					if (s.Type == AudioType.BGM)
					{
						if (s.Source == null)
						{
							continue;
						}

                        //s.Source.volume = (_isMuteMusic) ? 0f : 1f;
                        s.Source.mute = _isMuteMusic;
                    }
                }
			}
		}

		public void MuteSfx()
		{
			if (PlayerPrefs.HasKey("MuteSFX"))
			{
                OnChangeSFX?.Invoke();
                _isMuteSfx = PlayerPrefs.GetInt("MuteSFX") == 1 ? true : false;
				foreach (var s in _database)
				{
					if (s.AudioClip == null)
						continue;
					if (s.Type == AudioType.SFX)
					{
						if (s.Source == null)
						{
							continue;
						}

						s.Source.mute = _isMuteSfx;
                    }
				}
			}
		}

		public void StopMusic(AudioName audioName)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");

			}
			else
			{
				if (s.Type == AudioType.BGM && s.Source != null)
				{
					s.Source.Stop();
				}
			}
		}

		public void PauseMusic(AudioName audioName)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
			}
			else
			{
				if (s.Type == AudioType.BGM && s.Source != null)
				{
					s.Source.Pause();
				}
			}
		}

		public void UnPauseMusic(AudioName audioName)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
			}
			else
			{
				if (s.Type == AudioType.BGM && s.Source != null)
				{
					s.Source.UnPause();
				}
			}
		}

		public void LowerVolume(AudioName audioName, float duration)
		{
			if (Instance._isLowered == false)
			{
				AudioData s = GetAudioData(audioName);
				if (s == null)
				{
					Debug.LogError("Sound name" + audioName + "not found!");
					return;
				}
				else
				{
					Instance._tmpName = audioName;
					Instance._tmpVol = s.Volume;
					Instance._timeToReset = Time.time + duration;
					Instance._timerIsSet = true;
					s.Source.volume = s.Source.volume / 3;
				}

				Instance._isLowered = true;
			}
		}

		public void FadeOut(AudioName audioName, float duration)
		{
			Instance.StartCoroutine(Instance.IFadeOut(audioName, duration));
		}

		public void FadeIn(AudioName audioName, float duration)
		{
			Instance.StartCoroutine(Instance.IFadeIn(audioName, duration));
		}



		//not for use
		private IEnumerator IFadeOut(AudioName audioName, float duration)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + name + "not found!");
				yield return null;
			}
			else
			{
				if (_fadeOut == false)
				{
					_fadeOut = true;
					
					if (s.Source == null)
					{
						yield return null;
					}
					
					float startVol = s.Source.volume;
					_fadeOutUsedString = name;
					while (s.Source != null && s.Source.volume > 0)
					{
						s.Source.volume -= startVol * Time.deltaTime / duration;
						yield return null;
					}

					s.Source.Stop();
					yield return new WaitForSeconds(duration);
					_fadeOut = false;
				}
				else
				{
					//Debug.Log("Could not handle two fade outs at once : " + name + " , " + _fadeOutUsedString +
					  //        "! Stopped the music " + name);
					//StopMusic(audioName);//dont stop, cause stop same music
				}
			}
		}

		private IEnumerator IFadeIn(AudioName audioName, float duration)
		{
			AudioData s = GetAudioData(audioName);
			if (s == null)
			{
				Debug.LogError("Sound name" + audioName + "not found!");
				yield return null;
			}
			else
			{
				if (s.Source == null)
				{
					s.Source = CreateAudioSource(s);
				}

				//mute => volum = 0
				//if (_isMuteMusic)
				//    s.Source.volume = 0f;
				if (s.Source.isPlaying)
					yield return null;
				
				{
					if (_fadeIn == false)
					{
						_fadeIn = true;
						Instance._fadeInUsedString = name;
						s.Source.Play();

						if (_isMuteMusic) //check mute music
						{
                            s.Source.mute = true;
							_fadeIn = false;
							yield break;
						}

                        s.Source.volume = 0f;
                        float targetVolume = s.Volume;
						while (s.Source.volume < targetVolume)
						{
							s.Source.volume += Time.deltaTime / duration;
							yield return null;
						}

						yield return new WaitForSeconds(duration);
						_fadeIn = false;
					}
					else
					{
						Debug.Log("Could not handle two fade ins at once: " + name + " , " + _fadeInUsedString +
						          "! Played the music " + name);
						StopMusic(audioName);
						PlayMusic(audioName);
					}
				}
			}
		}

		void ResetVol()
		{
			AudioData s = GetAudioData(_tmpName);
			s.Source.volume = _tmpVol;
			_isLowered = false;
		}

		private void Update()
		{
			if (Time.time >= _timeToReset && _timerIsSet)
			{
				ResetVol();
				_timerIsSet = false;
			}

			_timeToCheckIdleAudioSource += Time.deltaTime;
			if (_timeToCheckIdleAudioSource > TIME_TO_CHECK_IDLE_AUDIO_SOURCE)
			{
				var audios = GetComponents<AudioSource>();
				foreach (var a in audios)
				{
					if (!a.isPlaying)
						Destroy(a);
				}

				_timeToCheckIdleAudioSource = 0;
			}
		}

		#endregion
	}
}