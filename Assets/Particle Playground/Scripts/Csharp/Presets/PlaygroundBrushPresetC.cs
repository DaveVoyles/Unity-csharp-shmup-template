using UnityEngine;
using System;
/*	Particle Playground - Brush Preset
*	Use this script to create your own presets to paint emission positions with.
*	Easiest way to create a new brush is to start by duplicating an existing brush prefab in the folder Resources/Brushes.
*/

public class PlaygroundBrushPresetC : MonoBehaviour {

	// Preset properties
	public string presetName = "Brush";					// The name of this brush preset

	// Brush properties
	public Texture2D texture;							// The texture to construct this Brush from
	public float scale = 1f;							// The scale of this Brush
	public BRUSHDETAILC detail;							// The detail level of this brush
	public float distance = 10000f;						// The distance the brush reaches

	// Paint properties
	public float spacing = .1f;							// The required space between the last and current paint position 
	public bool exceedMaxStopsPaint = false;			// Should painting stop when paintPositions is equal to maxPositions (if false paint positions will be removed from list when painting new ones)
}