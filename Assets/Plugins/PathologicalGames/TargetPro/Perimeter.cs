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
    ///	This class is is a custom implimentation of the IList interface so it is a custom
    ///	list that can take advantage of Unity's collider OnTrigger events to add and remove
    ///	Targets (and much more). This is tightly coupled with the TargetTracker instance
    ///	which creates it and cannot be used standalone.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))] // Needed to make collider private (parent ignores)
    public class Perimeter : MonoBehaviour, IList<Target>
    {
        /// <summary>
        /// The targetTracker which created this perimeter
        /// </summary>
        internal TargetTracker targetTracker;

        /// <summary>
        /// Setting this to false signals UpdateSort() that the list was changed and it
        /// needs to run immediatly.
        /// </summary>
        internal bool dirty = true;

        /// <summary>
        /// The primary data List for the perimeter.
        /// </summary>
        private TargetList targets = new TargetList();

        // Cache
        private Transform xform;


        #region Events
        /// <summary>
        /// Cache
        /// </summary>
        private void Awake()
        {
            this.xform = this.transform;
            this.rigidbody.isKinematic = true;
        }


        /// <summary>
        /// Runs a sort when the list is dirty (has been changed) or when the timer expires.
        /// This co-routine will stop if there are no targets. and must be restarted when a
        /// target enters.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateSort()
        {
            // Init 
            float startTime = Time.fixedTime;
            float intervalCounter = this.targetTracker.sortInterval;
            bool ignoreIntervalCounter = false;
            while (this.targets.Count > 0)   // Quit if there are no targets
            {
                if (this.targetTracker.sortInterval == 0)
                    ignoreIntervalCounter = true;
                else
                    ignoreIntervalCounter = false;

                // Sort if the list was changed or if the counter is greater than 0.
                //   Changing the list (setting dirty to true) will make this run immediatly
                //   AND reset the counter so the sort isn't run again right away.
                if (!this.dirty && (intervalCounter > 0 || ignoreIntervalCounter))
                {
                    intervalCounter -= Time.fixedTime - startTime;
                }
                else  // Time to sort!
                {
#if UNITY_EDITOR
                    if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                        Debug.Log("SORTING: " + this.ToString());
#endif

                    this.dirty = false;                   // reset to clean
                    intervalCounter = this.targetTracker.sortInterval;  // Reset
                    startTime = Time.fixedTime;           // New start time

                    // SORT!
                    var comparer = new TargetTracker.TargetComparer
                    (
                        this.targetTracker.sortingStyle,
                        this.xform
                    );

                    this.targets.Sort(comparer);
                }

                yield return new WaitForSeconds(this.targetTracker.sortInterval);
            }
        }
        #endregion Events




        #region Events
        /// <summary>
        /// Adds units to the list of targets as they enter range if they are in the 
        /// correct layer.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (!this.IsInLayerMask(other.gameObject)) return;

            // Get a target struct which will also cache information, such as the Transfrom,
            //   GameObject and ITargetable component
            var target = new Target(other.transform, this.targetTracker);

            // Do Add() only if this is ITargetable
            if (target.targetable == null) return;

            // Ignore if isTargetable is false
            if (!target.targetable.isTargetable) return;

            this.Add(target);
        }


        /// <summary>
        /// Drop units from the list of targets as they leave range.
        /// </summary>
        private void OnTriggerExit(Collider other)
        {
            // Will only work if there is something to remove. Debug logging inside too.
            this.Remove(other.transform);
        }
        #endregion Events



        #region List Interface
        public void Add(Target target)
        {
            // We are about to add our first target. Track for later.
            bool isFirst = false;
            if (this.targets.Count == 0)
                isFirst = true;

            this.targets.Add(target);  // ADD

            // Add this perimeter to the targetable's list for event communication
            target.targetable.perimeters.Add(this);

            // Trigger sorting.
            this.dirty = true;

#if UNITY_EDITOR
            if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                Debug.Log(string.Format("{0}  : Target ADDED - {1}",
                                        this.xform.parent.name,
                                        target.transform.name));
#endif

            // Trigger the delegate execution for this event
            target.targetable.OnDetected(this.targetTracker);


            // START SORTING?
            // Only If this is the first target and the sorting option is not none.
            if (isFirst &&
                this.targetTracker.sortingStyle != TargetTracker.SORTING_STYLES.None)
            {
#if UNITY_EDITOR
                if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                    Debug.Log(string.Format("{0} : Starting sorting co-routine",
                                            this.xform.parent.name,
                                            target.transform.name));
#endif

                this.StartCoroutine(this.UpdateSort());
            }
        }

        /// <summary>
        /// Remove an object from the list explicitly. 
        /// This works even if the object is still in range, effectivley hiding it from the 
        /// perimiter.
        /// </summary>
        /// <param name="xform">The transform component of the target to remove</param>
        /// <returns>True if somethign was removed</returns>
        public bool Remove(Transform xform)
        {
            return this.Remove(new Target(xform, this.targetTracker));
        }

        /// <summary>
        /// Remove a Targetable
        /// </summary>
        /// <param name="xform">The transform component of the target to remove</param>
        /// <returns>True if somethign was removed</returns>
        public bool Remove(Targetable targetable)
        {
            // Fillout the struct directly to avoid internal GetComponent calls.
            var target = new Target();
            target.gameObject = targetable.gameObject;
            target.transform = targetable.transform;
            target.targetable = targetable;

            return this.Remove(target);
        }

        /// <summary>
        /// Remove an object from the list explicitly. 
        /// This works even if the object is still in range, effectivley hiding it from the 
        /// perimiter.
        /// </summary>
        /// <param name="target">The Target to remove</param>
        /// <returns>True if somethign was removed</returns>
        public bool Remove(Target target)
        {
            // Quit if nothing was removed
            if (!this.targets.Remove(target)) return false;

            // Remove this perimeter from targetable's list to keep in sync
            target.targetable.perimeters.Remove(this);

            // Trigger sorting.
            this.dirty = true;

            // Silence errors on game exit / unload clean-up
            if (target.transform == null || this.xform == null || this.xform.parent == null)
                return false;

#if UNITY_EDITOR
            if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                Debug.Log(string.Format("{0}  : Target Removed - {1}",
                                        this.xform.parent.name,
                                        target.transform.name));
#endif

            // Trigger the delegate execution for this event
            target.targetable.OnNotDetected(this.targetTracker);

            return true;
        }


        /// <summary>
        /// Read-only index access
        /// </summary>
        /// <param name="index">int address of the item to get</param>
        /// <returns></returns>
        public Target this[int index]
        {
            get { return this.targets[index]; }
            set { throw new System.NotImplementedException("Read-only."); }
        }


        /// <summary>
        /// Clears the entire list explicitly
        /// This works even if the object is still in range, effectivley hiding it from the 
        /// perimiter.
        /// </summary>
        public void Clear()
        {
            // Trigger the delegate execution for this event
            foreach (Target target in this.targets)
                target.targetable.OnNotDetected(this.targetTracker);

            this.targets.Clear();

#if UNITY_EDITOR
            if (this.targetTracker.debugLevel > DEBUG_LEVELS.Off) // If on at all
                Debug.Log(string.Format("{0}  : All Targets Cleared!",
                                        this.xform.parent.name));
#endif

            // Trigger sorting.
            this.dirty = true;
        }


        /// <summary>
        /// Tests to see if an item is in the list
        /// </summary>
        /// <param name="item">The transform component of the target to test</param>
        /// <returns>True of the item is in the list, otherwise false</returns>
        public bool Contains(Transform transform)
        {
            return this.targets.Contains(new Target(transform, this.targetTracker));
        }


        /// <summary>
        /// Tests to see if an item is in the list
        /// </summary>
        /// <param name="target">The target object to test</param>
        /// <returns>True of the item is in the list, otherwise false</returns>
        public bool Contains(Target target)
        {
            return this.targets.Contains(target);
        }


        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Target> GetEnumerator()
        {
            foreach (Target target in this.targets)
                yield return target;
        }

        // Non-generic version? Not sure why this is used by the interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (Target target in this.targets)
                yield return target;
        }


        /// <summary>
        /// Used by OTHERList.AddRange()
        /// This adds this list to the passed list
        /// </summary>
        /// <param name="array">The list AddRange is being called on</param>
        /// <param name="arrayIndex">
        /// The starting index for the copy operation. AddRange seems to pass the last index.
        /// </param>
        public void CopyTo(Target[] array, int arrayIndex)
        {
            this.targets.CopyTo(array, arrayIndex);
        }


        // Not implimented from iList
        public int IndexOf(Target item) { throw new System.NotImplementedException(); }
        public void Insert(int index, Target item) { throw new System.NotImplementedException(); }
        public void RemoveAt(int index) { throw new System.NotImplementedException(); }
        public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
        #endregion List Interface



        #region List Utilities
        /// <summary>
        /// Returns the number of items in this (the collection). Readonly.
        /// </summary>
        public int Count { get { return this.targets.Count; } }


        /// <summary>
        ///	Returns a string representation of this (the collection)
        /// </summary>
        public override string ToString()
        {
            // Build an array of formatted strings then join when done
            string[] stringItems = new string[this.targets.Count];
            string stringItem;
            int i = 0;   // Index counter
            foreach (Target target in this.targets)
            {
                // Protection against async destruction.
                if (target.transform == null)
                {
                    stringItems[i] = "null";
                    i++;
                    continue;
                }

                stringItem = string.Format("{0}:Layer={1}",
                                            target.transform.name,
                                            LayerMask.LayerToName(target.gameObject.layer));

                // Finish the string for this target based on the target style
                switch (this.targetTracker.sortingStyle)
                {
                    case TargetTracker.SORTING_STYLES.None:
                        // Do nothing
                        break;

                    case TargetTracker.SORTING_STYLES.Nearest:
                    case TargetTracker.SORTING_STYLES.Farthest:
                        float d;
                        d = target.targetable.GetDistToPos(this.xform.position);
                        stringItem += string.Format(",Dist={0}", d);
                        break;

                    case TargetTracker.SORTING_STYLES.NearestToDestination:
                    case TargetTracker.SORTING_STYLES.FarthestFromDestination:
                        stringItem += string.Format(",DistToDest={0}",
                                                    target.targetable.distToDest);
                        break;
                }

                stringItems[i] = stringItem;
                i++;
            }

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return string.Format("[{0}]", System.String.Join(", ", stringItems));
        }
        #endregion List Utilities



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
            LayerMask targetMask = this.targetTracker.targetLayers;
            if ((targetMask.value & objMask.value) == 0) // Extra brackets required!
                return false;
            else
                return true;
        }

        #endregion Private Utilities
    }

}