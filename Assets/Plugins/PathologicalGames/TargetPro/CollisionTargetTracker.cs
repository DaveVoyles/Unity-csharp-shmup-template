/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    ///	This ineherits TargetTracker because of some limitations with RequireComponent().
    /// </summary>
    [AddComponentMenu("Path-o-logical/TargetPro/CollisionTargetTracker")]
    public class CollisionTargetTracker : TargetTracker
    {
        /// <summary>
        /// A list of sorted targets. The contents depend on numberOfTargets requested
        /// (-1 for all targets in the perimeter), and the sorting style userd.
        /// </summary>
        public override TargetList targets
        {
            get
            {
                // Start with an empty list each pass.
                this._targets.Clear();

                // If none are wanted, quit
                if (this.numberOfTargets == 0)
                    return this._targets;

                // Filter for any targets that are no longer active before returning them
                var validTargets = new List<Target>(this.allTargets);
                foreach (Target target in this.allTargets)
                    if (!target.gameObject.activeInHierarchy)
                        validTargets.Remove(target);

                // If no targets available, quit
                if (validTargets.Count == 0)
                    return this._targets;

                // None == Area-of-effect, so get everything in range, otherwise, 
                //   Get the first item(s). Since everything is sorted based on the
                //   sortingStyle, the first item(s) will always work
                if (this.numberOfTargets == -1)
                {
                    this._targets.AddRange(validTargets);
                }
                else
                {
                    // Grab the first item(s)
                    int num = Mathf.Clamp(this.numberOfTargets, 0, validTargets.Count);
                    for (int i = 0; i < num; i++)
                        this._targets.Add(validTargets[i]);
                }

                if (this.onPostSortDelegates != null) this.onPostSortDelegates(this._targets);

#if UNITY_EDITOR
                if (this.debugLevel > DEBUG_LEVELS.Normal)  // All higher than normal
                {
                    string msg = string.Format("returning targets: {0}",
                                               this._targets.ToString());
                    Debug.Log(string.Format("{0}: {1}", this, msg));
                }
#endif
                return _targets;
            }
        }
        private TargetList allTargets = new TargetList();


        public Collider coll;


        protected override void Awake()
        {
            this.xform = this.transform;

            this.coll = this.collider;
            if (this.coll == null)
                throw new Exception("No collider or compound (child) collider found;");

            if (this.coll.isTrigger)
                throw new Exception
                (
                    "CollisionTargetTrackers do not work with trigger colliders." +
                    "It is designed to work with Physics OnCollider events only."
                );
        }

        /// <summary>
        /// Add targets to the internal list when they enter the collider
        /// </summary>
        private void OnCollisionEnter(Collision collisionInfo)
        {
            if (!this.IsInLayerMask(collisionInfo.gameObject)) return;

            // Get a target struct which will also cache information, such as the Transfrom,
            //   GameObject and ITargetable component
            var target = new Target(collisionInfo.transform, this);

            // Do Add() only if this is ITargetable
            if (target.targetable == null) return;

            // Ignore if isTargetable is false
            if (!target.targetable.isTargetable) return;

            if (!this.allTargets.Contains(target))
                this.allTargets.Add(target);

#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = string.Format
                (
                    "OnCollisionEnter detected target: {0} | All Targets = [{1}]",
                    target.targetable.name,
                    this.allTargets.ToString()
                );
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif
        }


        /// <summary>
        /// Remove targets from the internal list when they exit the collider
        /// </summary>
        private void OnCollisionExit(Collision collisionInfo)
        {
            // Note: Iterating and comparing cached data should be the fastest way...
            var target = new Target();
            foreach (Target currentTarget in this.allTargets)
                if (currentTarget.gameObject == collisionInfo.gameObject)
                    target = currentTarget;

            if (target == Target.Null)
                return;

            this.StartCoroutine(this.DelayRemove(target));

#if UNITY_EDITOR
            if (this.debugLevel > DEBUG_LEVELS.Off)  // All higher than normal
            {
                string msg = string.Format
                (
                    "OnCollisionExit no longer tracking target: {0} | All Targets = [{1}]",
                    target.targetable.name,
                    this.allTargets.ToString()
                );
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }
#endif
        }

        private IEnumerator DelayRemove(Target target)
        {
            yield return null;

            if (this.allTargets.Contains(target))
                this.allTargets.Remove(target);
        }

        // Shadow base class members to prevent unneeded logic from running.
        protected override void OnEnable() { }
        protected override void OnDisable() { }

        #region Private Utilities
        /// <summary>
        /// Checks if a GameObject is in a LayerMask
        /// </summary>
        /// <param name="obj">GameObject to test</param>
        /// <param name="layerMask">LayerMask with all the layers to test against</param>
        /// <returns>True if in any of the layers in the LayerMask</returns>
        private bool IsInLayerMask(GameObject obj)
        {
            // Convert the object's layer to a bit mask for comparison
            LayerMask objMask = 1 << obj.layer;
            LayerMask targetMask = this.targetLayers;
            if ((targetMask.value & objMask.value) == 0) // Extra brackets required!
                return false;
            else
                return true;
        }

        #endregion Private Utilities

    }
}