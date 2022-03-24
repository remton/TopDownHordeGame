//All credit to almirage on github
//https://gist.github.com/almirage/e9e4f447190371ee6ce9
//https://gist.github.com/Athomield/071b58697fa3d3daaf4557512d0971b7

using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour {

	public Sprite[] sprites;
	public int framesPerSprite = 6;
	public bool loop = true;
	public bool destroyOnEnd = false;
	public bool reverse = false;

	private int index = 0;
	private Image image;
	private int frame = 0;

	void Awake() {
		image = GetComponent<Image>();
	}

	/// Updates the animation
	void Update() {
		Debug.Log("SPRITES " + sprites.Length);


		// Prevents run if not looping at end
		if (CheckAnimationEnded()) { return; }

		frame++;    // Increment frame count
		if (frame < framesPerSprite) return; // Wait for enough frames to pass
		frame = 0;  // Reset frame count

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
