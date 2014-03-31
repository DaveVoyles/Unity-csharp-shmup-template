/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    ///	Add description of the class
    /// </summary>
    [AddComponentMenu("Path-o-logical/TargetPro/Targetable")]
    [RequireComponent(typeof(Rigidbody))]
    public class Targetable : MonoBehaviour
    {
        #region Public Parameters
        public bool isTargetable = true;

        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        public List<Perimeter> perimeters = new List<Perimeter>();

        // Delegate type declarations
        public delegate void OnDetectedDelegate(TargetTracker source);
        public delegate void OnNotDetectedDelegate(TargetTracker source);
        public delegate void OnHitDelegate(HitEffectList effects, Target target);
        public delegate void OnHitColliderDelegate(HitEffectList effects, Target target, Collider other);
        #endregion Public Parameters


        #region Private Parameters
        public Transform xform;
        public GameObject go;

        // Internal lists for each delegate type
        private OnDetectedDelegate onDetectedDelegates;
        private OnNotDetectedDelegate onNotDetectedDelegates;
        private OnHitDelegate onHitDelegates;
        private OnHitColliderDelegate onHitColliderDelegates;
        #endregion Private Parameters



        #region Events
        /// <summary>
        /// Cache
        /// </summary>
        private void Awake()
        {
            // Cache
            this.xform = this.transform;
            this.go = this.gameObject;
        }


        /// <summary>
        /// Handle clean up on disable and destroy
        /// </summary>
        /// <param name="source"></param>
        private void OnDisable() { this.CleanUp(); }
        private void OnDetroy() { this.CleanUp(); }
        private void CleanUp()
        {
            if (!Application.isPlaying) return; // Game was stopped.

            var perimetersCopy = new List<Perimeter>(this.perimeters);
            foreach (Perimeter perimeter in perimetersCopy)
            {
                // Protect against async destruction
                if (perimeter.Count == 0 || perimeter.targetTracker == null)
                    continue;

                perimeter.Remove(this);

#if UNITY_EDITOR
                if (this.debugLevel > DEBUG_LEVELS.Normal)
                {
                    string msg = string.Format
                    (
                        "Targetable ({0}): On Disabled or Destroyed - " +
                                            "Removed from {1}.",
                        this.name,
                        perimeter.targetTracker.name
                    );
                    Debug.Log(msg);
                }
#endif
            }
        }

        /// <summary>
        /// Triggered when a target is hit
        /// </summary>
        /// <param name="source">The HitEffectList to send</param>
        /// <param name="source">
        /// The target struct used to cache this target when sent
        /// </param>
        public void OnHit(HitEffectList effects, Target target)
        {
            this.OnHit(effects, target, null);
        }

        public void OnHit(HitEffectList effects, Target target, Collider other)
        {
#if UNITY_EDITOR
            // Normal level debug and above
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                Debug.Log
                (
                    string.Format
                    (
                        "Targetable ({0}): HitEffects[{1}]",
                        this.name,
                        effects.ToString()
                    )
                );
            }
#endif

            // Set the hitTime for all effects in the list.
            effects = effects.CopyWithHitTime();

            if (this.onHitDelegates != null)
                this.onHitDelegates(effects, target);

            // Same as above with addition of collider in signature
            if (this.onHitColliderDelegates != null)
                this.onHitColliderDelegates(effects, target, other);
        }

        /// <summary>
        /// Triggered when a target is first found by a perimeter
        /// </summary>
        /// <param name="source">The TargetTracker which triggered this event</param>
        internal void OnDetected(TargetTracker source)
        {
#if UNITY_EDITOR
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Normal)
            {
                string msg = "Detected by " + source.name;
                Debug.Log(string.Format("Targetable ({0}): {1}", this.name, msg));
            }
#endif

            if (this.onDetectedDelegates != null) this.onDetectedDelegates(source);
        }

        /// <summary>
        /// Triggered when a target is first found by a perimeter
        /// </summary>
        /// <param name="source">The TargetTracker which triggered this event</param>
        internal void OnNotDetected(TargetTracker source)
        {
#if UNITY_EDITOR
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Normal)
            {
                string msg = "No longer detected by " + source.name;
                Debug.Log(string.Format("Targetable ({0}): {1}", this.name, msg));
            }
#endif

            if (this.onNotDetectedDelegates != null) this.onNotDetectedDelegates(source);
        }
        #endregion Events



        #region Target Tracker Members
        public float strength { get; set; }

        /// <summary>
        /// Waypoints is just a list of positions used to determine the distance to
        /// the final destination. See distToDest.
        /// </summary>
        [HideInInspector]
        public List<Vector3> waypoints = new List<Vector3>();

        /// <summary>
        /// Get the distance from this GameObject to the nearest waypoint and then
        /// through all remaining waypoints.
        /// Set wayPoints (List of Vector3) to use this feature.
        /// The distance is kept as a sqrMagnitude for faster performance and
        /// comparison.
        /// </summary>
        /// <returns>The distance as sqrMagnitude</returns>
        public float distToDest
        {
            get
            {
                if (this.waypoints.Count == 0) return 0;  // if no points, return

                // First get the distance to the first point from the current position
                float dist = this.GetDistToPos(waypoints[0]);

                // Add the distance to each point from the one before.
                for (int i = 0; i < this.waypoints.Count - 2; i++)  // -2 keeps this in bounds
                    dist += (waypoints[i] - waypoints[i + 1]).sqrMagnitude;

                return dist;
            }
        }


        /// <summary>
        /// Get the distance from this GameObject to another position in space.
        /// The distance is kept as a sqrMagnitude for faster performance and
        /// comparison
        /// </summary>
        /// <param name="other">The position to find the distance to</param>
        /// <returns>The distance as sqrMagnitude</returns>
        public float GetDistToPos(Vector3 other)
        {
            return (this.xform.position - other).sqrMagnitude;
        }



        #region Delegate Add/Set/Remove Functions
        #region OnDetectedDelegates
        /// <summary>
        /// Add a new delegate to be triggered when a target is first found by a perimeter.
        /// The delegate signature is:  delegate(TargetTracker source)
        /// See TargetTracker documentation for usage of the provided 'source'
        /// </summary>
        /// <param name="del"></param>
        public void AddOnDetectedDelegate(OnDetectedDelegate del)
        {
            this.onDetectedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnDetectedDelegate(OnDetectedDelegate del)
        {
            this.onDetectedDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate 
        /// See docs for AddOnDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnDetectedDelegate(OnDetectedDelegate del)
        {
            this.onDetectedDelegates -= del;
        }
        #endregion OnDetectedDelegates


        #region OnNotDetectedDelegate
        /// <summary>
        /// Add a new delegate to be triggered when a target is dropped by a perimieter for
        /// any reason; leaves or is removed.
        /// The delegate signature is:  delegate(TargetTracker source)
        /// See TargetTracker documentation for usage of the provided 'source'
        /// </summary>
        /// <param name="del"></param>
        public void AddOnNotDetectedDelegate(OnNotDetectedDelegate del)
        {
            this.onNotDetectedDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnNotDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnNotDetectedDelegate(OnNotDetectedDelegate del)
        {
            this.onNotDetectedDelegates = del;
        }

        /// <summary>
        /// Removes a OnNotDetectedDelegate 
        /// See docs for AddOnNotDetectedDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnNotDetectedDelegate(OnNotDetectedDelegate del)
        {
            this.onNotDetectedDelegates -= del;
        }
        #endregion OnNotDetectedDelegate


        #region OnHitDelegate
        /// <summary>
        /// Add a new delegate to be triggered when the target is hit.
        /// The delegate signature is:  delegate(HitEffectList effects, Target target)
        /// See IHitEffect documentation for usage of the provided 'effect'
        /// </summary>
        /// <param name="del"></param>
        public void AddOnHitDelegate(OnHitDelegate del)
        {
            this.onHitDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnHitDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnHitDelegate(OnHitDelegate del)
        {
            this.onHitDelegates = del;
        }

        /// <summary>
        /// Removes a OnHitDelegate 
        /// See docs for AddOnHitDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnHitDelegate(OnHitDelegate del)
        {
            this.onHitDelegates -= del;
        }
        #endregion OnHitDelegate


        #region OnHitColliderDelegate
        /// <summary>
        /// Add a new delegate to be triggered when the target is hit. This is the same as 
        /// the OnHitDelegate but also provides a collider. The collider will be null if 
        /// a projectile detonates for other reasons.
        /// The delegate signature is:  
        ///     delegate(HitEffectList effects, Target target, Collider other)
        /// See IHitEffect documentation for usage of the provided 'effect'
        /// </summary>
        /// <param name="del"></param>
        public void AddOnHitColliderDelegate(OnHitColliderDelegate del)
        {
            this.onHitColliderDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnHitColliderDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void SetOnHitColliderDelegate(OnHitColliderDelegate del)
        {
            this.onHitColliderDelegates = del;
        }

        /// <summary>
        /// Removes a OnHitColliderDelegate 
        /// See docs for AddOnHitColliderDelegate()
        /// </summary>
        /// <param name="del"></param>
        public void RemoveOnHitColliderDelegate(OnHitColliderDelegate del)
        {
            this.onHitColliderDelegates -= del;
        }
        #endregion OnHitColliderDelegate

        #endregion Delegate Add/Set/Remove Functions

        #endregion Target Tracker Members

    }
}