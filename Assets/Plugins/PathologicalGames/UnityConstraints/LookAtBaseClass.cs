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


namespace PathologicalGames
{

    /// <summary>
    ///	This is the base class for all look-at based constraints including billboarding.
    /// </summary>
    [AddComponentMenu("")] // Hides from Unity4 Inspector menu
    public class LookAtBaseClass : ConstraintBaseClass
    {
        /// <summary>
        /// The axis used to point at the target. 
        /// This is public for user input only, should not be used by derrived classes
        /// </summary>
        public Vector3 pointAxis = -Vector3.back;

        /// <summary>
        /// The axis to point up in world space.  
        /// This is public for user input only, should not be used by derrived classes
        /// </summary>
        public Vector3 upAxis = Vector3.up;

        /// <summary>
        /// Runs when the noTarget mode is set to ReturnToDefault
        /// </summary>
        protected override void NoTargetDefault()
        {
            this.xform.rotation = Quaternion.identity;
        }


        protected override Transform internalTarget
        {
            get
            {
                // Note: This backing field is in the base class
                if (this._internalTarget != null)
                    return this._internalTarget;

                Transform target = base.internalTarget; // Will init internalTarget GO

                // Set the internal target to 1 unit away in the direction of the pointAxis
                // this.target will be the internalTarget due to SetByScript
                target.position = (this.xform.rotation * this.pointAxis) + this.xform.position;

                return this._internalTarget;
            }
        }


        /// <summary>
        /// Processes the user's 'pointAxis' and 'upAxis' to look at the target.
        /// </summary>
        /// <param name="lookVect">The direction in which to look</param>
        /// <param name="upVect">
        /// The secondary axis will point along a plane in this direction
        /// </param>
        protected Quaternion GetUserLookRotation(Quaternion lookRot)
        {
            // Get the look at rotation and a rotation representing the user input
            var userAxisRot = Quaternion.LookRotation(this.pointAxis, this.upAxis);

            // offset the look-at by the user input to get the final result
            return lookRot * Quaternion.Inverse(userAxisRot);
        }

    }
}