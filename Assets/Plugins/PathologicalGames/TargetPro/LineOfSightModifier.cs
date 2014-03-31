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
    ///	Adds Line-of-Sight (LOS) filtering to TargetPRO components. Line of sight means 
    ///	events are based on whether or not a target can be "seen". This visibility test 
    ///	is done by ray casting against a given layer. If the ray is broken before hitting 
    ///	the target, the target is not in LOS.
    ///	
    /// If added to the same GameObject as a TargetTracker it can filter out any targets 
    /// which are not currently in LOS.
    /// 
    /// If added to the same GameObject as a FireController it can prevent firing on any 
    /// targets which are not currently in LOS.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TargetPro/Modifier - Line of Sight")]
    [RequireComponent(typeof(TargetTracker))]
    public class LineOfSightModifier : MonoBehaviour
    {
        #region Parameters
        public LayerMask targetTrackerLayerMask;
        public LayerMask fireControllerLayerMask;

        public enum TEST_MODE { SinglePoint, SixPoint }
        public TEST_MODE testMode = TEST_MODE.SinglePoint;
        public float radius = 1.0f;

        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;
        #endregion Parameters


        #region Cache
        // Public for reference and Inspector logic
        [HideInInspector]
        public TargetTracker tracker;
        [HideInInspector]
        public FireController fireCtrl;
        #endregion Cache


        private void Awake()
        {
            this.tracker = this.GetComponent<TargetTracker>();
            this.fireCtrl = this.GetComponent<FireController>();

            this.tracker.AddOnPostSortDelegate(this.FilterTrackerTargetList);

            if (this.fireCtrl != null)
                this.fireCtrl.AddOnPreFireDelegate(this.FilterFireTargetList);
        }


        private void FilterTrackerTargetList(TargetList targets)
        {
            // Quit if the mask is set to nothing == OFF
            if (this.targetTrackerLayerMask.value == 0)
                return;

            Vector3 fromPos = this.tracker.perimeter.transform.position;
            LayerMask mask = this.targetTrackerLayerMask;
            this.FilterTargetList(targets, mask, fromPos, Color.red);
        }


        private void FilterFireTargetList(TargetList targets)
        {
            // Quit if the mask is set to nothing == OFF
            if (this.fireControllerLayerMask.value == 0)
                return;

            Vector3 fromPos;
            if (this.fireCtrl.emitter != null)
                fromPos = this.fireCtrl.emitter.position;
            else
                fromPos = this.fireCtrl.transform.position;

            LayerMask mask = this.fireControllerLayerMask;
            this.FilterTargetList(targets, mask, fromPos, Color.yellow);

        }


        private void FilterTargetList(TargetList targets, LayerMask mask, Vector3 fromPos,
                                      Color debugLineColor)
        {
#if UNITY_EDITOR
            var debugRemoveNames = new List<string>();
#endif

            Vector3 toPos;
            bool isNotLOS;
            var iterTargets = new List<Target>(targets);

            foreach (Target target in iterTargets)
            {
                isNotLOS = false;

                toPos = target.targetable.xform.position;

                if (this.testMode == TEST_MODE.SixPoint)
                {
                    var sweep = new List<Vector3>();
                    sweep.Add(new Vector3(toPos.x + this.radius, toPos.y, toPos.z));
                    sweep.Add(new Vector3(toPos.x, toPos.y + this.radius, toPos.z));
                    sweep.Add(new Vector3(toPos.x, toPos.y, toPos.z + this.radius));

                    sweep.Add(new Vector3(toPos.x - this.radius, toPos.y, toPos.z));
                    sweep.Add(new Vector3(toPos.x, toPos.y - this.radius, toPos.z));
                    sweep.Add(new Vector3(toPos.x, toPos.y, toPos.z - this.radius));

                    foreach (Vector3 pos in sweep)
                    {
                        isNotLOS = Physics.Linecast(fromPos, pos, mask);
#if UNITY_EDITOR
                        if (this.debugLevel > DEBUG_LEVELS.Off)
                            Debug.DrawLine(fromPos, pos, debugLineColor, 0.01f);
#endif
                        // Quit loop at first positive test
                        if (isNotLOS)
                            continue;
                        else
                            break;
                    }
                }
                else
                {
                    isNotLOS = Physics.Linecast(fromPos, toPos, mask);
#if UNITY_EDITOR
                    if (this.debugLevel > DEBUG_LEVELS.Off)
                        Debug.DrawLine(fromPos, toPos, debugLineColor, 0.01f);
#endif
                }

                if (isNotLOS)
                {
                    targets.Remove(target);

#if UNITY_EDITOR
                    debugRemoveNames.Add(target.targetable.name);
#endif
                }
            }

#if UNITY_EDITOR
            if (this.debugLevel == DEBUG_LEVELS.High && debugRemoveNames.Count > 0)
                Debug.Log("Holding fire for LOS: " +
                          string.Join(",", debugRemoveNames.ToArray()));
#endif

        }

    }
}