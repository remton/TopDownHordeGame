//All credit to almirage on github
//https://gist.github.com/almirage/e9e4f447190371ee6ce9
//https://gist.github.com/Athomield/071b58697fa3d3daaf4557512d0971b7

using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour {

	public Sprite[] sprites;
	public float framesPerSec = 10;
	public bool loop = true;
	public bool destroyOnEnd = false;
	public bool reverse = false;
	public bool playOnAwake = true;

	////Event called when animation ends. 
	//public delegate void OnEnd();
	//public event OnEnd EventOnEnd;

	private int index = 0;
	private Image image;
	private bool isPlaying;
	void Awake() {
		if (playOnAwake)
			Play();
	}

	private float timeUntilNextFrame;
	private float frameTime;

	public void Play() {
		image = GetComponent<Image>();
		frameTime = 1f / framesPerSec;
		timeUntilNextFrame = frameTime;
		isPlaying = true;
    }

	/// Updates the animation
	void Update() {
		if (!isPlaying)
			return;
		
		// Prevents run if not looping at end
		if (CheckAnimationEnded()) {
            //if (EventOnEnd != null) { EventOnEnd.Invoke(); }
			return; 
		}
		
		//Wait for more time to pass
		if (!(timeUntilNextFrame <= 0)) {
			timeUntilNextFrame -= Time.deltaTime;
			return;
		}

		timeUntilNextFrame = frameTime; //Set time until next frame


		image.sprite = sprites[Mathf.Clamp(index, 0, sprites.Length - 1)];  // Apply current sprite

		if (reverse) { index--; } // +/- sprite index
		else { index++; }

		// Looping and Self-Destruction
		if (!reverse && index >= sprites.Length) {
			if (loop) index = 0;
			if (destroyOnEnd) Destroy(gameObject);
		}
		else if (reverse && index < 0) {
			if (loop) index = sprites.Length;
			if (destroyOnEnd) Destroy(gameObject);
		}
	}

	/// Check if the animation has ended.
	internal bool CheckAnimationEnded() {
		return (!loop &&
			((!reverse && index >= sprites.Length) ||
			(reverse && index <= -1)));
	}
}
