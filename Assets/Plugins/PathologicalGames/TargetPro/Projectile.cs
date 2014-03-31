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
    [AddComponentMenu("Path-o-logical/TargetPro/Projectile")]
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : TargetTracker
    {
        #region Parameters
        /// <summary>
        /// Holds the target passed by the FireController on launch
        /// </summary>
        public Target target { get; internal set; }

        /// <summary>
        /// This is used internally to provide an interface in the inspector and to store
        /// structs as serialized objects.
        /// </summary>
        public List<HitEffectGUIBacker> _effectsOnTarget = new List<HitEffectGUIBacker>();

        /// <summary>
        /// A list of HitEffect structs which hold one or more descriptions
        /// of how this Projectile can affect a Target.
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

        /// <summary>
        /// If true, more than just the primary target will be affected when this projectile
        /// detonates. Use the range options to determine the behavior.
        /// </summary>
        public bool areaHit = true;

        /// <summary>
        /// If the projectile has a rigidbody, this will detonate it if it falls asleep.
        /// See Unity's docs for more information on how this happens.
        /// </summary>
        public bool detonateOnRigidBodySleep = true;

        /// <summary>
        /// Determines what should cause this projectile to detonate.
        ///     TargetOnly
        ///         Only a direct hit will trigger detonation
        ///     HitLayers
        ///         Contact with any colliders in any of the layers in the HitLayers mask 
        ///         will trigger detonation
        /// </summary>
        public DETONATION_MODES detonationMode = DETONATION_MODES.HitLayers;
        public enum DETONATION_MODES { TargetOnly, HitLayers }

        /// <summary>
        /// An optional timer to detonate this projectile no matter what happens after
        /// this timer in seconds expires.
        /// </summary>
        public float timer = 0;

        /// <summary>
        /// Sets the target notification behavior. Telling targets they are hit is optional 
        /// for situations where a delayed response is required, such as launching a projectile, 
        /// or for custom handling
        /// 
        /// MODES:
        ///     Off
        ///         Do not notify anything. delegates can still be used for custom handling
        ///     Direct
        ///         On Detonation targets will be notified immediately
        ///     PassToDetonator
        ///         If the DetonationPrefab has an Explosion component, let it handle
        ///         notifying the targets
        /// </summary>
        public NOTIFY_TARGET_OPTIONS notifyTargets = NOTIFY_TARGET_OPTIONS.Direct;
        public enum NOTIFY_TARGET_OPTIONS { Off, Direct, PassToDetonator }

        /// <summary>
        /// An optional prefab to instance on detonation
        /// </summary>
        public Transform detonationPrefab;

        /// <summary>
        /// The FireController which launched this projectile
        /// </summary>
        public FireController fireController;
        #endregion Parameters


        #region Cache
        // Keeps the state of each individual foldout item during the editor session - tiny data
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        private float curTimer;
        private Rigidbody rbd;
        #endregion Cache



        protected override void Awake()
        {
            base.Awake();

            // This will be tested for null when used. This is optional.
            this.rbd = this.GetComponent<Rigidbody>();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            this.StartCoroutine(this.Launch());
        }


        private IEnumerator Launch()
        {
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = "Launching...";
                Debug.Log(string.Format("Projectile ({0}): {1}", this.name, msg));
            }

            // Reset
            this.curTimer = this.timer;

            // Wait for next frame to begin to be sure targets have been propegated
            // This also makes this loop easier to manage.
            yield return new WaitForFixedUpdate();  // Because of physics...matters?

            // START EVENT
            if (this.OnLaunchedDelegates != null) this.OnLaunchedDelegates();

            // The timer can exit the loop if used 
            while (true)
            {
                // UPDATE EVENT
                if (this.OnLaunchedUpdateDelegates != null) this.OnLaunchedUpdateDelegates();

                // Detonate if rigidbody falls asleep - optional.
                if (this.detonateOnRigidBodySleep && this.rbd != null && this.rbd.IsSleeping())
                    this.DetonateProjectile();

                // Only work with the timer if the user entered a value over 0
                if (this.timer > 0)
                {
                    if (this.curTimer <= 0) break;

                    this.curTimer -= Time.deltaTime;
                }


                yield return new WaitForFixedUpdate();  // Because of physics...matters?
            }

            // Will only run if the timer is used and it breaks the loop above
            this.DetonateProjectile();

        }

        /// <summary>
        /// Destroys the projectile on impact with target or if no target, a collider 
        /// in the layermask, or anything: depends on the chosen DETONATION_MODES
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            switch (this.detonationMode)
            {
                case DETONATION_MODES.HitLayers:
                    // LayerMask compare
                    if (((1 << other.gameObject.layer) & this.targetLayers) != 0)
                    {
                        // If this rojectile was set to hit layers but still not an area  
                        //   hit, try to hit the collider that detonated it
                        if (!this.areaHit)
                        {
                            var xform = other.transform;
                            var targetable = xform.GetComponent<Targetable>();

                            // If the collider's gameObject doesn't have a targetable
                            //   component, it can't be a valid target, so ignore it
                            if (targetable != null)
                                this.target = new Target(xform, this);
                        }

                        this.DetonateProjectile();
                    }

                    return;

                case DETONATION_MODES.TargetOnly:
                    if (this.target.isSpawned &&
                        this.target.gameObject == other.gameObject) // Target was hit
                    {
                        this.DetonateProjectile();
                    }

                    return;
            }

            // else keep flying...
        }

        /// <summary>
        /// Destroys the projectile on impact and finds objects in range to 
        ///	affect if they share the same tag as target.
        /// </summary>
        public void DetonateProjectile()
        {
            // Prevent being run more than once in a frame where it has already 
            //   been destroyed.
            if (!this.gameObject.activeInHierarchy) return;

            // Build a new list of targets depending on the options used
            var targetList = new TargetList();
            if (this.areaHit)
            {
                // This is turned back off OnDisable() (base class)
                this.perimeter.enabled = true;

                targetList.AddRange(this.targets); // Add all targets in range
            }
            else
            {
                if (this.target != Target.Null)
                    targetList.Add(this.target); // Add projectile target
            }

            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = string.Format("Detonating with targets: {0}", targetList);
                Debug.Log(string.Format("Projectile ({0}): {1}", this.name, msg));
            }

            // Create a new list of targets which have this target tracker reference.
            //   This is for output so targets which are handled at all by this Projectile
            //   are stamped with a reference.
            var targetCopies = new TargetList();
            Target target;
            foreach (Target inTarget in targetList)
            {
                if (inTarget == Target.Null)
                    continue;

                // Can't edit a struct in a foreach loop, so need to copy and store
                target = new Target(inTarget);
                target.projectile = this;  // Add reference. null before t
                targetCopies.Add(target);

                switch (this.notifyTargets)
                {
                    case NOTIFY_TARGET_OPTIONS.Direct:
                        target.targetable.OnHit(this.effectsOnTarget, target, this.collider);
                        break;
                }

                // Just for debug. Show a gizmo line when firing
                if (this.debugLevel > DEBUG_LEVELS.Off)
                    Debug.DrawLine
                    (
                        this.xform.position,
                        target.transform.position,
                        Color.red
                    );

            }

            switch (this.notifyTargets)
            {
                case NOTIFY_TARGET_OPTIONS.Direct:
                    this.SpawnDetonatorPrefab(false);
                    break;

                case NOTIFY_TARGET_OPTIONS.PassToDetonator:
                    this.SpawnDetonatorPrefab(true);
                    break;
            }


            // Trigger delegates
            if (this.OnDetonationDelegates != null) this.OnDetonationDelegates(targetCopies);

            // Clean-up in case this instance is used in a pooling system like PoolManager
            this.target = Target.Null;

            InstanceManager.Despawn(this.transform);
        }


        /// <summary>
        /// Spawns a prefab if this.detonationPrefab is not null.
        /// If the detonationPrefab has a detonator component, it is optionally
        /// passed this Projectiles effects on target and range.
        /// </summary>
        /// <param name="passEffects"></param>
        private void SpawnDetonatorPrefab(bool passEffects)
        {
            // This is optional. If no ammo prefab is set, quit quietly
            if (this.detonationPrefab == null) return;

            Transform inst = InstanceManager.Spawn
            (
                this.detonationPrefab.transform,
                this.xform.position,
                this.xform.rotation
            );

            // Nothing left to do, this is just a simple spawn trigger.
            if (!passEffects) return;

            // Detonator....
            var detonator = inst.GetComponent<Detonator>();
            if (detonator == null)   // Protection
                return;

            detonator.effectsOnTarget = this.effectsOnTarget;
            detonator.perimeterShape = this.perimeterShape;
            detonator.range = this.range;
        }



        #region OnLaunched Delegates
        /// <summary>
        /// Runs when this projectile is launched
        /// </summary>
        public delegate void OnLaunched();
        private OnLaunched OnLaunchedDelegates;

        public void AddOnLaunchedDelegate(OnLaunched del)
        {
            this.OnLaunchedDelegates += del;
        }

        public void SetOnLaunchedDelegate(OnLaunched del)
        {
            this.OnLaunchedDelegates = del;
        }

        public void RemoveOnLaunchedDelegate(OnLaunched del)
        {
            this.OnLaunchedDelegates -= del;
        }
        #endregion OnDetonation Delegates



        #region OnLaunchedUpdate Delegates
        /// <summary>
        /// Runs every frame
        /// </summary>
        public delegate void OnLaunchedUpdate();
        private OnLaunchedUpdate OnLaunchedUpdateDelegates;

        public void AddOnLaunchedUpdateDelegate(OnLaunchedUpdate del)
        {
            this.OnLaunchedUpdateDelegates += del;
        }

        public void SetOnLaunchedUpdateDelegate(OnLaunchedUpdate del)
        {
            this.OnLaunchedUpdateDelegates = del;
        }

        public void RemoveOnLaunchedUpdateDelegate(OnLaunchedUpdate del)
        {
            this.OnLaunchedUpdateDelegates -= del;
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