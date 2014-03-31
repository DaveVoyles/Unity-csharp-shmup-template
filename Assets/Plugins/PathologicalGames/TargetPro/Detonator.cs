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
    ///	Controls the basic functionality and information decismination for a projectile
    ///	Use another script with the exposes delegates to control the effects and movement.
    /// </summary>
    [AddComponentMenu("Path-o-logical/TargetPro/Detonator")]
    public class Detonator : TargetTracker
    {
        #region Parameters
        /// <summary>
        /// The amount of time it takes to grow from 0 to the max range size.
        /// </summary>
        public float durration = 2;

        /// <summary>
        /// The range this detonator will grow to. The same as the starting range as setup 
        /// in the inspector
        /// </summary>
        public Vector3 maxRange;

        /// <summary>
        /// This is used internally to provide an interface in the inspector and to store
        /// structs as serialized objects.
        /// </summary>
        public List<HitEffectGUIBacker> _effectsOnTarget = new List<HitEffectGUIBacker>();

        /// <summary>
        /// A list of HitEffect structs which hold one or more descriptions
        /// of how this Detonator can affect a Target.
        /// This is replaced by a projectile's effects if spawned by a projectile.
        /// </summary>
        // Encodes / Decodes HitEffects to and from HitEffectGUIBackers
        public HitEffectList effectsOnTarget
        {
            // Convert the stored _effectsOnTarget backing field to a list of HitEffects
            get
            {
                var returnHitEffectsList = new HitEffectList();
                foreach (var effectBacker in this._effectsOnTarget)
                {
                    // Create and add a struct-form of the backing-field instance
                    returnHitEffectsList.Add
                    (
                        new HitEffect
                        {
                            name = effectBacker.name,
                            value = effectBacker.value,
                            duration = effectBacker.duration,
                        }
                    );
                }

                return returnHitEffectsList;
            }

            // Convert and store the bassed list of HitEffects as HitEffectGUIBackers
            set
            {
                // Clear and set the backing-field list also used by the GUI
                this._effectsOnTarget.Clear();

                HitEffectGUIBacker effectBacker;
                foreach (var effect in value)
                {
                    effectBacker = new HitEffectGUIBacker(effect);
                    this._effectsOnTarget.Add(effectBacker);
                }
            }
        }

        #endregion Parameters


        #region Cache
        // Keeps the state of each individual foldout item during the editor session - tiny data
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();
        #endregion Cache



        protected override void Awake()
        {
            base.Awake();

            // Cache
            this.maxRange = this.range;

            // Start at zero. Also done in Detonate for use with PoolManager respawning.
            this.range = Vector3.zero;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            this.StartCoroutine(this.Detonate());
        }

        private IEnumerator Detonate()
        {
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = "Detonating...";
                Debug.Log(string.Format("Detonator ({0}): {1}", this.name, msg));
            }

            // Wait for next frame to begin to be sure targets have been propegated
            // This also makes this loop easier to manage.
            yield return new WaitForFixedUpdate();  // Because of physics...matters?

            // START EVENT
            if (this.OnDetonatingDelegates != null) this.OnDetonatingDelegates();

            // Keep track of targets which have already been processed so they 
            //      aren't hit twice
            var processedTargetList = new TargetList();

            this.range = Vector3.zero;

            float timer = 0;
            float progress = 0;  // Normalized mount processed 0-1

            // The timer can exit the loop if used 
            while (true)
            {
                // UPDATE EVENT
                if (this.OnDetonatingUpdateDelegates != null)
                    this.OnDetonatingUpdateDelegates(progress);

                // Exit?
                if (timer >= this.durration) break;
                timer += Time.deltaTime;

                progress = timer / this.durration;

                this.range = this.maxRange * progress;

                // Build a list of targets in range which have NOT been processed yet.
                var newTargets = new TargetList();
                foreach (Target target in this.targets)
                    if (!processedTargetList.Contains(target))
                        newTargets.Add(target);

                if (newTargets.Count > 0)
                {
                    if (this.debugLevel > DEBUG_LEVELS.Off)
                    {
                        string msg = string.Format("Detonation hitting targets: {0}", this.targets);
                        Debug.Log(string.Format("Detonator ({0}): {1}", this.name, msg));
                    }

                    foreach (Target target in newTargets)
                    {
                        target.targetable.OnHit
                        (
                            this.effectsOnTarget,
                            target,
                            this.perimeter.collider
                        );

                        processedTargetList.Add(target);
                    }
                }

                yield return new WaitForFixedUpdate();  // Because of physics...matters?
            }

            // Prevent being run more than once in a frame where it has already 
            //   been destroyed.
            if (!this.gameObject.activeInHierarchy)
                yield break;  // Same as return

            PathologicalGames.InstanceManager.Despawn(this.transform);
        }


        #region OnDetonating Delegates
        /// <summary>
        /// Runs when this projectile is launched
        /// </summary>
        public delegate void OnDetonating();
        private OnDetonating OnDetonatingDelegates;

        public void AddOnDetonatingDelegate(OnDetonating del)
        {
            this.OnDetonatingDelegates += del;
        }

        public void SetOnDetonatingDelegate(OnDetonating del)
        {
            this.OnDetonatingDelegates = del;
        }

        public void RemoveOnDetonatingDelegate(OnDetonating del)
        {
            this.OnDetonatingDelegates -= del;
        }
        #endregion OnDetonation Delegates



        #region OnDetonatingUpdate Delegates
        /// <summary>
        /// Runs every frame
        /// </summary>
        public delegate void OnDetonatingUpdate(float progress);
        private OnDetonatingUpdate OnDetonatingUpdateDelegates;

        public void AddOnDetonatingUpdateDelegate(OnDetonatingUpdate del)
        {
            this.OnDetonatingUpdateDelegates += del;
        }

        public void SetOnDetonatingUpdateDelegate(OnDetonatingUpdate del)
        {
            this.OnDetonatingUpdateDelegates = del;
        }

        public void RemoveOnDetonatingUpdateDelegate(OnDetonatingUpdate del)
        {
            this.OnDetonatingUpdateDelegates -= del;
        }
        #endregion OnDetonation Delegates



        #region OnDetonation Delegates
        /// <summary>
        /// Runs when this projectile detonates.
        /// </summary>
        /// <param name="targets"></param>
        public delegate void OnDetonation(TargetList targets);
        private OnDetonation OnDetonationDelegates;

        public void AddOnDetonationDelegate(OnDetonation del)
        {
            this.OnDetonationDelegates += del;
        }

        public void SetOnDetonationDelegate(OnDetonation del)
        {
            this.OnDetonationDelegates = del;
        }

        public void RemoveOnDetonationDelegate(OnDetonation del)
        {
            this.OnDetonationDelegates -= del;
        }
        #endregion OnDetonation Delegates



    }
}