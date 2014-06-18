#pragma strict

/*	Particle Playground - Brush Preset
*	Use this script to create your own presets to paint emission positions with.
*	Easiest way to create a new brush is to start by duplicating an existing brush prefab in the folder Resources/Brushes.
*/

// Preset properties
var presetName : String = "Brush";				// The name of this brush preset

// Brush properties
var texture : Texture2D;						// The texture to construct this Brush from
var scale : float = 1.0;						// The scale of this Brush
var detail : BRUSHDETAIL;						// The detail level of this brush
var distance : float = 10000;					// The distance the brush reaches

// Paint properties
var spacing : float = .1;						// The required space between the last and current paint position 
var exceedMaxStopsPaint : boolean = false;		// Should painting stop when paintPositions is equal to maxPositions (if false paint positions will be removed from list when painting new ones)
