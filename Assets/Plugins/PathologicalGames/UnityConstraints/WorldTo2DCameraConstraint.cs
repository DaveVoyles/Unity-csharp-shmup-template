/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PathologicalGames
{
    /// <summary>
    ///	Makes this Transform, being rendered from an orthographic (2D) camera, appear to 
    ///	follow a target being rendered by another camera, in another space. 
    ///	
    /// This is perfect for use with most sprite-based GUI plugins to make a GUI element 
    /// follow a 3D object. For example, a life bar can be setup to stay above the head of 
    /// enemies.
    /// </summary>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint- World To 2D Camera")]
    public class WorldTo2DCameraConstraint : TransformConstraint
    {
        /// <summary>
        /// If false, the billboard will only rotate along the upAxis.
        /// </summary>
        public bool vertical = true;

        /// <summary>
        /// The camera used to render the target
        /// </summary>
        public Camera targetCamera;

        /// <summary>
        /// The camera the output object will be constrained to.
        /// </summary>
        public Camera orthoCamera;

        /// <summary>
        /// Offset the screen position. The z value is depth from the camera.
        /// </summary>
        public Vector3 offset;

        /// <summary>
        /// Determines when to apply the offset.
        /// WorldSpace - Applies the X and Y offset relative to the target object, in world-
        ///              space, before converting to the ortheCamera's viewport-space. The
        ///              offset values are used for world units
        /// ViewportSpace - Applies the X and Y offset in the ortheCamera's viewport-space. 
        ///              The offset values are in viewport space, which is normalized 0-1.
        /// </summary>
        public enum OFFSET_MODE { WorldSpace, ViewportSpace };

        /// <summary>
        /// Determines when to apply the offset. In world-space or view-port space
        /// </summary>
        public OFFSET_MODE offsetMode = OFFSET_MODE.WorldSpace;

        /// <summary>
        /// Determines what happens when the target moves offscreen
        /// Constraint - Continues to track the target the same as it does on-screen
        /// Limit - Stops at the edge of the frame but continues to track the target
        /// DoNothing - Stops calculating the constraint
        /// </summary>
        public enum OFFSCREEN_MODE { Constrain, Limit, DoNothing };

        /// <summary>
        /// Determines what happens when the target moves offscreen
        /// </summary>
        public OFFSCREEN_MODE offScreenMode = OFFSCREEN_MODE.Constrain;

        /// <summary>
        /// Offsets the behavior determined by offscreenMode. 
        /// e.g. 0.1 to 0.9 would stop 0.1 from the edge of the screen on both sides
        /// </summary>
        public Vector2 offscreenThreasholdW = new Vector2(0, 1);

        /// <summary>
        /// Offsets the behavior determined by offscreenMode. 
        /// e.g. 0.1 to 0.9 would stop 0.1 from the edge of the screen on both sides
        /// </summary>
        public Vector2 offscreenThreasholdH = new Vector2(0, 1);


        protected override void Awake()
        {
            base.Awake();

            // Defaults...
            // This is done in the inspector as well, but needs to be in both places
            //   so any application is covered.
            var cameras = FindObjectsOfType(typeof(Camera)) as Camera[];

            // Default to the first ortho camera that is set to render this object
            foreach (Camera cam in cameras)
            {
                if (!cam.orthographic)
                    continue;

                if ((cam.cullingMask & 1 << this.gameObject.layer) > 0)
                {
                    this.orthoCamera = cam;
                    break;
                }
            }

            // Default to the first camera that is set to render the target
            if (this.target != null)
            {
                foreach (Camera cam in cameras)
                {
                    if ((cam.cullingMask & 1 << this.target.gameObject.layer) > 0)
                    {
                        this.targetCamera = cam;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Ignore position work in the base class
            bool constrainPositionSetting = this.constrainPosition;
            this.constrainPosition = false;

            base.OnConstraintUpdate(); // Handles rotation and scale

            // Set back to the user setting
            this.constrainPosition = constrainPositionSetting;

            if (!this.constrainPosition)
                return;

            // Note: pos is reused and never destroyed
            this.pos = this.target.position;

            if (this.offsetMode == WorldTo2DCameraConstraint.OFFSET_MODE.WorldSpace)
            {
                this.pos.x += this.offset.x;
                this.pos.y += this.offset.y;
            }

            this.pos = this.targetCamera.WorldToViewportPoint(this.pos);

            if (this.offsetMode == WorldTo2DCameraConstraint.OFFSET_MODE.ViewportSpace)
            {
                this.pos.x += this.offset.x;
                this.pos.y += this.offset.y;
            }

            switch (this.offScreenMode)
            {
                case OFFSCREEN_MODE.DoNothing:
                    // The pos is normalized so if it is between 0 and 1 in x and y, it is on screen.
                    bool isOnScreen = (
                        this.pos.z > 0f &&
                        this.pos.x > this.offscreenThreasholdW.x &&
                        this.pos.x < this.offscreenThreasholdW.y &&
                        this.pos.y > this.offscreenThreasholdH.x &&
                        this.pos.y < this.offscreenThreasholdH.y
                    );

                    if (!isOnScreen)
                        return;  // Quit
                    else
                        break;  // It is on screen, so continue...

                case OFFSCREEN_MODE.Constrain:
                    break;  // Nothing modified. Continue...

                case OFFSCREEN_MODE.Limit:
                    // pos is normalized so if it is between 0 and 1 in x and y, it is on screen.
                    // Clamp will do nothing unless the target is offscreen
                    this.pos.x = Mathf.Clamp
                    (
                        this.pos.x,
                        this.offscreenThreasholdW.x,
                        this.offscreenThreasholdW.y
                    );

                    this.pos.y = Mathf.Clamp
                    (
                        this.pos.y,
                        this.offscreenThreasholdH.x,
                        this.offscreenThreasholdH.y
                    );
                    break;
            }

            this.pos = this.orthoCamera.ViewportToWorldPoint(this.pos);
            this.pos.z = this.offset.z;

            // Output only if wanted, otherwise use the transform's current value
            if (!this.outputPosX) this.pos.x = this.position.x;
            if (!this.outputPosY) this.pos.y = this.position.y;
            if (!this.outputPosZ) this.pos.z = this.position.z;

            this.xform.position = pos;
        }

    }
}