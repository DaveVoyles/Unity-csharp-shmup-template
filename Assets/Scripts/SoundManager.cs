/* Copyright (c) 2012, STANISLAW ADASZEWSKI
All rights reserved.
 http://algoholic.eu/sound-manager-for-unity3d/
Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of STANISLAW ADASZEWSKI nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL STANISLAW ADASZEWSKI BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
 USAGE:
     private SoundManager sm;
     sm = SoundMananger.GetSingleton();
     sm.PlayClip(SFX_shoot, false);
 */

using UnityEngine;
using System.Collections;
using System.IO;

public class SoundManager : MonoBehaviour
{
	/// <summary>
	/// Returns (creating if necessary) the SoundManager singleton.
	/// </summary>
	/// <returns>
	/// The SoundManager singleton.
	/// </returns>
	public static SoundManager GetSingleton() {
		if (sGameObj == null) {
			sGameObj = new GameObject();
		    sGameObj.name = "SoundManager";
			return (SoundManager) sGameObj.AddComponent(typeof(SoundManager));
		}
		return (SoundManager) sGameObj.GetComponent(typeof(SoundManager));
	}
	
	/// <summary>
	/// Immediately sets volume of the specified channel.
	/// </summary>
	/// <param name="i">Channel number</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	public void SetVolume(int i, float newVol) {
		oldVolume[i] = newVol;
		newVolume[i] = newVol;
		sources[i].volume = newVol;
	}
	
	/// <summary>
	/// Linearly interpolates volume of the specified channel
	/// from current value to the new value during the specified time.
	/// </summary>
	/// <param name="i">Channel number</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	/// <param name="time">Time in seconds</param>
	public void SetVolume(int i, float newVol, float time) {
		oldVolume[i] = sources[i].volume;
		newVolume[i] = newVol;
		transitionStart[i] = Time.time;
		transitionTime[i] = time;
	}
	

	/// <summary>
	/// Immediately sets volume of the specified clip. The difference
	/// between this method and SetVolume() taking channel number as
	/// parameter is that this method will effect the setting for all
	/// channels playing the given clip.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	public void SetVolume(AudioClip c, float newVol) {
		for (int i = 0; i < sources.Length; i++) {
			AudioSource s = sources[i];
			if (s.clip == c) {
				oldVolume[i] = newVol;
				newVolume[i] = newVol;
				s.volume = newVol;
			}
		}
	}
	
	/// <summary>
	/// Linearly interpolates volume of the specified clip
	/// from current value to the new value during the specified time.
	/// The difference between this method and SetVolume() taking channel
	/// number as parameter is that this method will effect the setting for all
	/// channels playing the given clip.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="newVol">New volume setting, 0.0f to 1.0f</param>
	/// <param name="time">Time in seconds</param>
	public void SetVolume(AudioClip c, float newVol, float time) {
		for (int i = 0; i < sources.Length; i++) {
			AudioSource s = sources[i];
			if (s.clip == c) {
				oldVolume[i] = s.volume;
				newVolume[i] = newVol;
				transitionStart[i] = Time.time;
				transitionTime[i] = time;
			}
		}
	}
	

	/// <summary>
	/// Plays given audio clip on any free channel.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="loop">Loop setting</param>
	/// <returns>Number of the assigned channel</returns>
	public int PlayClip(AudioClip c, bool loop) {
		for (int i = 0; i < sources.Length; i++) {
			AudioSource s = sources[i];
			if (!s.isPlaying) {
				s.clip = c;
				s.loop = loop;
				s.Play();
				SetVolume(i, 1.0f);
				return i;
			}
		}
		return -1;
	}
	
	/// <summary>
	/// Plays given audio clip on any free channel included in the mask.
	/// </summary>
	/// <param name="c">Audio clip</param>
	/// <param name="mask">Channel mask, e.g. to specify 0th, 3rd and 11th channel, use 0x0809</param>
	/// <param name="loop">Loop setting</param>
	/// <returns>Number of the assigned channel</returns>
	public int PlayClip(AudioClip c, int mask, bool loop) {
		for (int i = 0; i < sources.Length; i++) {
			if ((mask & (1 << i)) > 0 && !sources[i].isPlaying) {
				sources[i].clip = c;
				sources[i].loop = loop;
				sources[i].Play();
				SetVolume(i, 1.0f);
				return i;
			}
		}
		return -1;
	}
	
	/// <summary>
	/// Stops all channels playing the given clip.
	/// </summary>
	/// <param name="c">Audio clip</param>
	public void StopClip(AudioClip c) {
		foreach (AudioSource s in sources) {
			if (s.clip == c && s.isPlaying) {
				s.Stop();
			}
		}
	}
	
	/// <summary>
	/// Stops the given channel.
	/// </summary>
	/// <param name="i">Channel number</param>
	public void StopChannel(int i) {
		sources[i].Stop();
	}
	

	/// <summary>
	/// Utility function for changing music between levels.
	/// Has to be called using StartCoroutine(). Requires System.IO, therefore
	/// for maximum portability you might want to remove it along with the
	/// System.IO dependency.
	///
	/// The function lists all .ogg files in the music/ subdirectory
	/// picks one on random and starts playing it, fading out any previous music.
	/// Calling it once at the beginning of a level should achieve what people
	/// usually want.
	///
	/// Example:
	/// SoundManager sm = SoundManager.GetSingleton();
	/// StartCoroutine(sm.ShuffleMusic());
	/// </summary>
	public IEnumerator ShuffleMusic() {
		// Music
		DirectoryInfo di = new DirectoryInfo("music");
		FileInfo[] fi = di.GetFiles("*.ogg");
		WWW www = new WWW("file://" + fi[musicChoice++ % fi.Length].FullName);
		
		yield return www;
		
		int oldChan = musicChan;
		if (oldChan >= 0) {
			SetVolume(oldChan, 0, 5);
		}
		musicChan = PlayClip(www.GetAudioClip(false, true), true);
		if (oldChan >= 0) {
			SetVolume(musicChan, 0);
			SetVolume(musicChan, 0.5f, 5);
		} else {
			SetVolume (musicChan, 0.5f);
		}
	}

	// --------------
	// PRIVATE
	// --------------

	static GameObject sGameObj;
	int musicChan = -1;
	int musicChoice = (int)(Random.value * int.MaxValue);
	
	AudioSource[] sources;
	float[] oldVolume;
	float[] newVolume;
	float[] transitionStart;
	float[] transitionTime;
	Transform cam;
	
	SoundManager() {
	}
	
	void OnLevelWasLoaded(int level) {
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		for (int i = 0; i < sources.Length; i++) {
			if (!sources[i].loop) {
				sources[i].Stop();
			}
		}
	}
	
	void Awake() {
		DontDestroyOnLoad(sGameObj);
		
		sources = new AudioSource[16];
		
		for (int i = 0; i < sources.Length; i++) {
			sources[i] = (AudioSource) sGameObj.AddComponent(typeof(AudioSource));
		}
		
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		transform.position = cam.position;
		
		oldVolume = new float[sources.Length];
		newVolume = new float[sources.Length];
		transitionStart = new float[sources.Length];
		transitionTime = new float[sources.Length];
		
		for (int i = 0; i < sources.Length; i++) {
			oldVolume[i] = 1.0f;
			newVolume[i] = 1.0f;
			transitionStart[i] = 0.0f;
			transitionTime[i] = 0.00001f;
		}
	}
	
	void Update() {
		transform.position = cam.position;
		for (int i = 0; i < sources.Length; i++) {
			sources[i].volume = Mathf.Lerp(oldVolume[i], newVolume[i], Mathf.Min(1.0f, (Time.time - transitionStart[i]) / transitionTime[i]));
			if (newVolume[i] <= 0 && sources[i].volume <= 0 && sources[i].isPlaying) {
				sources[i].Stop();
			}
		}
	}
}


