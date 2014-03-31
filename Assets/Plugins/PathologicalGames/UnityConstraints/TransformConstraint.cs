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

    /// <description>
    ///	Constrain this transform to a target's scale, rotation and/or translation.
    /// </description>
    [AddComponentMenu("Path-o-logical/UnityConstraints/Constraint - Transform (Postion, Rotation, Scale)")]
    public class TransformConstraint : ConstraintBaseClass
    {
        /// <summary>
        /// Option to match the target's position
        /// </summary>
        public bool constrainPosition = true;

        /// <summary>
        /// If false, the rotation in this axis will not be affected
        /// </summary>
        public bool outputPosX = true;

        /// <summary>
        /// If false, the rotation in this axis will not be affected
        /// </summary>
        public bool outputPosY = true;

        /// <summary>
        /// If false, the rotation in this axis will not be affected
        /// </summary>
        public bool outputPosZ = true;

        /// <summary>
        /// Option to match the target's rotation
        /// </summary>
        public bool constrainRotation = false;

        /// <summary>
        /// Used to alter the way the rotations are set
        /// </summary>
        public UnityConstraints.OUTPUT_ROT_OPTIONS output =
                                        UnityConstraints.OUTPUT_ROT_OPTIONS.WorldAll;

        /// <summary>
        /// Option to match the target's scale. This is a little more expensive performance
        /// wise and not needed very often so the default is false.
        /// </summary>
        public bool constrainScale = false;


        // Cache...
        internal Transform parXform;


        /// <summary>
        /// Cache as much as possible before starting the co-routine
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            this.parXform = this.xform.parent;
        }


        /// <summary>
        /// Runs each frame while the constraint is active
        /// </summary>
        protected override void OnConstraintUpdate()
        {
            // Note: Do not run base.OnConstraintUpdate. It is not implimented

            if (this.constrainScale)
                this.SetWorldScale(target);

            if (this.constrainRotation)
            {
                this.xform.rotation = this.target.rotation;
                UnityConstraints.MaskOutputRotations(this.xform, this.output);
            }

            if (this.constrainPosition)
            {
                this.pos = this.xform.position;

                // Output only if wanted
                if (this.outputPosX) this.pos.x = this.target.position.x;
                if (this.outputPosY) this.pos.y = this.target.position.y;
                if (this.outputPosZ) this.pos.z = this.target.position.z;

                this.xform.position = pos;
            }
        }



        /// <summary>
        /// Runs when the noTarget mode is set to ReturnToDefault
        /// </summary>
        protected override void NoTargetDefault()
        {
            if (this.constrainScale)
                this.xform.localScale = Vector3.one;

            if (this.constrainRotation)
                this.xform.rotation = Quaternion.identity;

            if (this.constrainPosition)
                this.xform.position = Vector3.zero;
        }

        /// <summary>
        /// A transform used for all Perimeter Gizmos to calculate the final 
        /// position and rotation of the drawn gizmo
        /// </summary>
        internal static Transform scaleCalculator;

        /// <summary>
        /// Sets this transform's scale to equal the target in world space.
        /// </summary>
        /// <param name="sourceXform"></param>
        public virtual void SetWorldScale(Transform sourceXform)
        {
            // Set the scale now that both Transforms are in the same space
            this.xform.localScale = this.GetTargetLocalScale(sourceXform);
        }

        /// <summary>
        /// Sets this transform's scale to equal the target in world space.
        /// </summary>
        /// <param name="sourceXform"></param>
        internal Vector3 GetTargetLocalScale(Transform sourceXform)
        {
            // Singleton: Create a hidden empty gameobject to use for scale calculations
            //   All instances will this. A single empty game object is so small it will
            //   never be an issue. Ever.
            if (TransformConstraint.scaleCalculator == null)
            {
                string name = "TransformConstraint_spaceCalculator";

                // When the game starts and stops the reference can be lost but the transform
                //   still exists. Re-establish the reference if it is found.
                var found = GameObject.Find(name);
                if (found != null)
                {
                    TransformConstraint.scaleCalculator = found.transform;
                }
                else
                {
                    var scaleCalc = new GameObject(name);
                    scaleCalc.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    TransformConstraint.scaleCalculator = scaleCalc.transform;
                }
            }

            // Store the source's lossyScale, which is Unity's estimate of "world scale", 
            //   to this seperate Transform which doesn't have a parent (it is in world space) 
            Transform refXform = TransformConstraint.scaleCalculator;

            // Cast the reference transform in to the space of the source Xform using
            //   Parenting, then cast it back to set.
            refXform.parent = sourceXform;
            refXform.localRotation = Quaternion.identity;  // Stablizes this solution
            refXform.localScale = Vector3.one;

            // Parent the reference transform to this object so they are in the same
            //   space, now we have a local scale to use.
            refXform.parent = this.parXform;

            return refXform.localScale;
        }

    }


}