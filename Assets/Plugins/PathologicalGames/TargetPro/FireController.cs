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
using PathologicalGames;

namespace PathologicalGames
{
    /// <summary>
    ///	Handles target notification when the given parameters are met.
    /// </summary>
    [RequireComponent(typeof(TargetTracker))]
    [AddComponentMenu("Path-o-logical/TargetPro/Fire Controller")]
    public class FireController : MonoBehaviour
    {
        #region Public Parameters
        /// <summary>
        /// The interval in seconds between firing.
        /// </summary>
        public float interval;

        /// <summary>
        /// When true this controller will fire immediately when it first finds a target, then
        /// continue to count the interval normally.
        /// </summary>
        public bool initIntervalCountdownAtZero = true;

        /// <summary>
        /// Sets the target notification behavior. Telling targets they are hit is optional 
        /// for situations where a delayed response is required, such as launching a projectile, 
        /// or for custom handling
        /// 
        /// MODES:
        ///     Off
        ///         Do not notify anything. delegates can still be used for custom handling
        ///     Direct
        ///         OnFire targets will be notified of this controllers effects
        ///     PassToProjectile
        ///         OnFire a projectile will be launched and passed this controllers effects.
        ///         The projectile must then handle notification. 
        ///         Use this when you want to set the effects on the launcher
        ///     UseProjectileEffects
        ///         OnFire a projectile will be launched but this controllers effects will NOT
        ///         be passed. The projectile's effects will be used instead.
        ///         The projectile must then handle notification.
        ///         Use this when you want to set the effects on the projectile, for a generic
        ///         launcher
        /// </summary>
        public NOTIFY_TARGET_OPTIONS notifyTargets = NOTIFY_TARGET_OPTIONS.Direct;
        public enum NOTIFY_TARGET_OPTIONS { Off, Direct, PassToProjectile, UseProjectileEffects }

        /// <summary>
        /// An optional prefab to instance, for each target, each OnFire event
        /// </summary>
        public Transform ammoPrefab;

        /// <summary>
        /// This is used internally to provide an interface in the inspector and to store
        /// structs as serialized objects.
        /// </summary>
        public List<HitEffectGUIBacker> _effectsOnTarget = new List<HitEffectGUIBacker>();

        /// <summary>
        /// A list of HitEffect structs which hold one or more descriptions
        /// of how this FireController can affect a Target.
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
        /// If true, wait for the target to be in front of the emitter before firing
        /// </summary>
        public bool waitForAlignment = false;

        /// <summary>
        /// If false the true angles will be compared for alignment (More precise. Emitter 
        /// must point at target.) If true, only the direction matters. (Good when turning 
        /// in a direction but perfect alignment isn't needed.)
        /// </summary>
        public bool flatAngleCompare = false;

        /// <summary>
        /// If waitForAlignment is true: The transform used as the point to fire from. 
        /// Optional. Default = this.transform
        /// </summary>
        public Transform emitter;

        /// <summary>
        /// If waitForAlignment is true: If the emitter is pointing towards the target 
        /// within this angle, in degrees, the target can be fired upon. 
        /// </summary>
        public float lockOnAngleTolerance = 5;

        /// <summary>
        /// Turn this on to print a stream of messages to help you see what this
        /// FireController is doing
        /// </summary>
        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        /// <summary>
        /// The current counter used for firing. Gets reset at the interval when 
        /// successfully fired, otherwise will continue counting down in to negative
        /// numbers
        /// </summary>
        public float fireIntervalCounter = 99999;


        // Delegate type declarations
        public delegate void OnStartDelegate();
        public delegate void OnUpdateDelegate();
        public delegate void OnTargetUpdateDelegate(TargetList targets);
        public delegate void OnIdleUpdateDelegate();
        public delegate void OnStopDelegate();
        public delegate void OnPreFireDelegate(TargetList targets);
        public delegate void OnFireDelegate(TargetList targets);

        // Keeps the state of each individual foldout item during the editor session
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        #endregion Public Parameters



        #region Private Parameters
        private TargetTracker targetTracker;
        private TargetList targets = new TargetList();

        // Emtpy delegate used for collection of user added/removed delegates
        private OnStartDelegate onStartDelegates;
        private OnUpdateDelegate onUpdateDelegates;
        private OnTargetUpdateDelegate onTargetUpdateDelegates;
        private OnIdleUpdateDelegate onIdleUpdateDelegates;
        private OnStopDelegate onStopDelegates;
        private OnPreFireDelegate onPreFireDelegates;
        private OnFireDelegate onFireDelegates;
        #endregion Private Parameters



        #region Events
        /// <summary>
        /// Cache
        /// </summary>
        private void Awake()
        {
            // Emitter is optional
            if (this.emitter == null) this.emitter = this.transform;

            this.targetTracker = this.GetComponent<TargetTracker>(); // Required Component
        }

        /// <summary>
        /// Turn on the firing system when this component is enabled, which includes 
        /// creation
        /// </summary>
        private void OnEnable()
        {
            this.StartCoroutine(this.FiringSystem());   // Start event is inside this
        }

        /// <summary>
        /// Turn off the firing system if this component is disabled or destroyed
        /// </summary>
        private void OnDisable()
        {
            // This has to be here because if it is in the TargetingSystem coroutine
            //   when the coroutine is stopped, it will get skipped, not ran last.
            this.OnStop();    // EVENT TRIGGER

            // Clean up...
            // Clear the list so we don't keep garbage around for no reason.
            this.targets.Clear();
        }

        /// <summary>
        /// Runs once when when the targeting system starts up. This happens OnEnable(),
        /// which includes destroying this component
        /// </summary>
        private void OnStart()
        {
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = "Starting Firing System...";
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }

            if (this.onStartDelegates != null) this.onStartDelegates();
        }

        /// <summary>
        /// Runs each frame while the targeting system is active, no matter what.
        /// </summary>
        private void OnUpdate()
        {
            if (this.onUpdateDelegates != null) this.onUpdateDelegates();
        }

        /// <summary>
        /// Runs each frame while tracking a target. This.targets is not empty!
        /// </summary>
        private void OnTargetUpdate(TargetList targets)
        {
            if (this.onTargetUpdateDelegates != null) this.onTargetUpdateDelegates(targets);
        }

        /// <summary>
        /// Runs each frame while tower is idle (no targets)
        /// </summary>
        private void OnIdleUpdate()
        {
            if (this.onIdleUpdateDelegates != null) this.onIdleUpdateDelegates();
        }

        /// <summary>
        /// Runs once when when the targeting system is stopped. This happens OnDisable(),
        /// which includes destroying this component
        /// </summary>
        private void OnStop()
        {
            // Higest level debug
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = "stopping Firing System...";
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }

            if (this.onStopDelegates != null) this.onStopDelegates();
        }

        /// <summary>
        /// Fire on the targets
        /// </summary>
        private void OnFire()
        {
            // Log a message to show what is being fired on
            if (this.debugLevel > DEBUG_LEVELS.Off)
            {
                string msg = string.Format("Firing on: {0}\nHitEffects{1}",
                                      this.targetsString, this.effectsOnTarget.ToString());
                Debug.Log(string.Format("{0}: {1}", this, msg));
            }

            // Create a new list of targets which have this target tracker reference.
            var targetCopies = new TargetList();
            Target target;
            foreach (Target inTarget in this.targets)
            {
                // Can't edit a struct in a foreach loop, so need to copy and store
                target = new Target(inTarget);
                target.fireController = this;  // Add reference. null before t
                targetCopies.Add(target);

                switch (this.notifyTargets)
                {
                    case NOTIFY_TARGET_OPTIONS.Direct:
                        target.targetable.OnHit(this.effectsOnTarget, target);
                        this.SpawnAmmunition(target, false, false);
                        break;

                    case NOTIFY_TARGET_OPTIONS.PassToProjectile:
                        this.SpawnAmmunition(target, true, true);
                        break;

                    case NOTIFY_TARGET_OPTIONS.UseProjectileEffects:
                        this.SpawnAmmunition(target, true, false);
                        break;
                }

                if (this.notifyTargets > NOTIFY_TARGET_OPTIONS.Off)
                {
                    // Just for debug. Show a gizmo line when firing
                    if (this.debugLevel > DEBUG_LEVELS.Off)
                        Debug.DrawLine(this.emitter.position,
                                       target.transform.position,
                                       Color.red);
                }
            }

            // Write the result over the old target list. This is for output so targets
            //   which are handled at all by this target tracker are stamped with a 
            //   reference.
            this.targets = targetCopies;

            // Trigger the delegates
            if (this.onFireDelegates != null) this.onFireDelegates(this.targets);
        }

        private void SpawnAmmunition(Target target, bool passTarget, bool passEffects)
        {
            // This is optional. If no ammo prefab is set, quit quietly
            if (this.ammoPrefab == null) return;

            Transform inst = PathologicalGames.InstanceManager.Spawn
            (
                this.ammoPrefab.transform,
                this.emitter.position,
                this.emitter.rotation
            );

            // Nothing left to do, this is probably a direct-damage effect, like a laser
            if (!passTarget) return;

            // Projectile....
            var projectile = inst.GetComponent<Projectile>();
            if (projectile == null)   // Protection
            {
                var msg = string.Format("Ammo '{0}' must have an Projectile component", inst.name);
                Debug.Log(string.Format("{0}: {1}", this, msg));

                return;
            }

            // Pass informaiton
            projectile.fireController = this;
            projectile.target = target;

            if (passEffects) projectile.effectsOnTarget = this.effectsOnTarget;
        }

        #region Delegate Add/Set/Remove methods

        #region OnStartDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when the firing system first starts up. 
        /// This happens on OnEnable (which is also run after Awake when first instanced)
        /// The delegate signature is:  delegate()
        /// See TargetTracker documentation for usage of the provided '...'
        /// </summary>
        /// <param name="del">An OnStartDelegate</param>
        public void AddOnStartDelegate(OnStartDelegate del)
        {
            this.onStartDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnStartDelegate()
        /// </summary>
        /// <param name="del">An OnStartDelegate</param>
        public void SetOnStartDelegate(OnStartDelegate del)
        {
            this.onStartDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for AddOnStartDelegate()
        /// </summary>
        /// <param name="del">An OnStartDelegate</param>
        public void RemoveOnStartDelegate(OnStartDelegate del)
        {
            this.onStartDelegates -= del;
        }
        #endregion OnDetectedDelegates Add/Set/Remove



        #region OnUpdateDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered everyframe while active, no matter what.
        /// There are two events which are more specific to the two states of the system:
        ///   1. When Idle (No Target)  - See the docs for OnIdleUpdateDelegate()
        ///   2. When There IS a target - See the docs for OnTargetUpdateDelegate()
        /// The delegate signature is:  delegate()
        /// </summary>
        /// <param name="del">An OnUpdateDelegate</param>
        public void AddOnUpdateDelegate(OnUpdateDelegate del)
        {
            this.onUpdateDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnUpdateDelegate</param>
        public void SetOnUpdateDelegate(OnUpdateDelegate del)
        {
            this.onUpdateDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnUpdateDelegate</param>
        public void RemoveOnUpdateDelegate(OnUpdateDelegate del)
        {
            this.onUpdateDelegates -= del;
        }
        #endregion OnUpdateDelegates Add/Set/Remove



        #region OnTargetUpdateDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered each frame when a target is being tracked.
        /// For other 'Update' events, see the docs for OnUpdateDelegates()
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided 'Target' in this list.
        /// </summary>
        /// <param name="del">An OnTargetUpdateDelegate</param>
        public void AddOnTargetUpdateDelegate(OnTargetUpdateDelegate del)
        {
            this.onTargetUpdateDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnTargetUpdateDelegate</param>
        public void SetOnTargetUpdateDelegate(OnTargetUpdateDelegate del)
        {
            this.onTargetUpdateDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnTargetUpdateDelegate</param>
        public void RemoveOnTargetUpdateDelegate(OnTargetUpdateDelegate del)
        {
            this.onTargetUpdateDelegates -= del;
        }
        #endregion OnTargetUpdateDelegates Add/Set/Remove



        #region OnIdleUpdateDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered every frame when there is no target to track.
        /// The delegate signature is:  delegate()
        /// </summary>
        /// <param name="del">An OnIdleUpdateDelegate</param>
        public void AddOnIdleUpdateDelegate(OnIdleUpdateDelegate del)
        {
            this.onIdleUpdateDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnIdleUpdateDelegate</param>
        public void SetOnIdleUpdateDelegate(OnIdleUpdateDelegate del)
        {
            onIdleUpdateDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnIdleUpdateDelegate</param>
        public void RemoveOnIdleUpdateDelegate(OnIdleUpdateDelegate del)
        {
            onIdleUpdateDelegates -= del;
        }
        #endregion OnIdleUpdateDelegates Add/Set/Remove



        #region OnStopDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when the firing system is stopped. 
        /// This is caused by destroying the object which has this component or if
        /// the object or component are disabled (The system will restart when  
        /// enabled again)
        /// The delegate signature is:  delegate()
        /// </summary>
        /// <param name="del">An OnStopDelegate</param>
        public void AddOnStopDelegate(OnStopDelegate del)
        {
            this.onStopDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnStopDelegate</param>
        public void SetOnStopDelegate(OnStopDelegate del)
        {
            this.onStopDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnStopDelegate</param>
        public void RemoveOnStopDelegate(OnStopDelegate del)
        {
            this.onStopDelegates -= del;
        }
        #endregion OnStopDelegates Add/Set/Remove



        #region OnPreFireDelegates Add/Set/Remove
        /// <summary>
        /// Runs just before any OnFire target checks to allow custom target list 
        /// manipulation and other pre-fire logic.
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided '...'
        /// </summary>
        /// <param name="del">An OnPreFireDelegate</param>
        public void AddOnPreFireDelegate(OnPreFireDelegate del)
        {
            this.onPreFireDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnPreFireDelegate()
        /// </summary>
        /// <param name="del">An OnPreFireDelegate</param>
        public void SetOnPreFireDelegate(OnPreFireDelegate del)
        {
            this.onPreFireDelegates = del;
        }

        /// <summary>
        /// Removes a OnPostSortDelegate
        /// See docs for AddOnPreFireDelegate()
        /// </summary>
        /// <param name="del">An OnPreFireDelegate</param>
        public void RemoveOnPreFireDelegate(OnPreFireDelegate del)
        {
            this.onPreFireDelegates -= del;
        }
        #endregion OnPreFireDelegates Add/Set/Remove



        #region OnFireDelegates Add/Set/Remove
        /// <summary>
        /// Add a new delegate to be triggered when it is time to fire/notify a target(s).
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided 'Target' in this list.
        /// </summary>
        /// <param name="del">An OnFireDelegate</param>
        public void AddOnFireDelegate(OnFireDelegate del)
        {
            this.onFireDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnFireDelegate</param>
        public void SetOnFireDelegate(OnFireDelegate del)
        {
            this.onFireDelegates = del;
        }

        /// <summary>
        /// Removes a OnDetectedDelegate
        /// See docs for ()
        /// </summary>
        /// <param name="del">An OnFireDelegate</param>
        public void RemoveOnFireDelegate(OnFireDelegate del)
        {
            this.onFireDelegates -= del;
        }
        #endregion OnFireDelegates Add/Set/Remove

        #endregion Delegate Add/Set/Remove methods

        #endregion Events



        #region Public Methods
        /// <summary>
        /// Can be run to trigger this FireController to fire immediatly regardless of
        /// counter or other settings.
        /// 
        /// This still executes any PreFireDelegates
        /// </summary>
        /// <param name="resetIntervalCounter">Should the count be reset or continue?</param>
        public void FireImmediately(bool resetIntervalCounter)
        {
            if (resetIntervalCounter)
                this.fireIntervalCounter = this.interval;

            // Can alter this.targets
            if (this.onPreFireDelegates != null) this.onPreFireDelegates(this.targets);

            this.OnFire();
        }

        #endregion Public Methods



        #region Private Methods
        /// <summary>
        /// Handles all firing events including target aquisition and firing. 
        /// Events are:
        ///     OnStart() :
        ///         Runs once when the firing system first becomes active
        ///     OnUpdate() :
        ///         Runs each frame while the firing system is active
        ///     OnTargetUpdate() : 
        ///         Runs each frame while tracking a target (there is at least one target.)
        ///     OnIdleUpdate() :
        ///         Runs each frame while the firing system is idle (no targets)
        ///     OnFire() :
        ///         Runs when it is time to fire.
        ///     
        /// Counter Behavior Notes:
        ///   * If there are no targets. the counter will keep running up. 
        ///     This means the next target to enter will be fired upon 
        ///     immediatly.
        ///     
        ///   * The counter is always active so if the last target exits, then a 
        ///     new target enters right after that, there may still be a wait.	
        /// </summary>
        private IEnumerator FiringSystem()
        {
            // While (true) because of the timer, we want this to run all the time, not 
            //   start and stop based on targets in range
            if (this.initIntervalCountdownAtZero)
                this.fireIntervalCounter = 0;
            else
                this.fireIntervalCounter = this.interval;

            this.targets.Clear();
            this.OnStart();   // EVENT TRIGGER

            while (true)
            {
                // if there is no target, counter++, handle idle behavior, and 
                //   try next frame.
                // Will init this.targets for child classes as well.
                this.targets = this.targetTracker.targets;

                if (this.targets.Count != 0)
                {
                    // Let the delegate filter a copy of the list just for the OnFire 
                    //   Test. We still want this.targets to remain as is.
                    //   Do this in here to still trigger OnTargetUpdate
                    var targetsCopy = new TargetList();
                    targetsCopy.AddRange(this.targets);

                    if (this.onPreFireDelegates != null)
                        this.onPreFireDelegates(targetsCopy);

                    // if all is right, fire 
                    if (targetsCopy.Count != 0 &&    // Incase of pre-fire delegate changes
                        this.fireIntervalCounter <= 0 &&
                        this.isLockedOnTarget)    // Always true if WaitForAlignment is OFF
                    {
                        this.OnFire();
                        this.fireIntervalCounter = this.interval;  // Reset
                    }
                    else if (this.debugLevel > DEBUG_LEVELS.Off)
                    {
                        // Just for debug. Show a gizmo line to each target being tracked
                        //   OnFire() has another color, so keep this here where this 
                        //   won't overlay the OnFired line.
                        foreach (Target target in targets)
                            Debug.DrawLine
                            (
                                this.emitter.position,
                                target.transform.position,
                                Color.gray
                            );
                    }

                    // Update event while tracking a target
                    this.OnTargetUpdate(targets);   // EVENT TRIGGER
                }
                else
                {
                    // Update event while NOT tracking a target
                    this.OnIdleUpdate();   // EVENT TRIGGER
                }

                this.fireIntervalCounter -= Time.deltaTime;

                // Update event no matter what
                this.OnUpdate();   // EVENT TRIGGER

                // Stager calls to get Target (the whole system actually)
                yield return null;
            }
        }

        /// <summary>
        /// Returns true of the target is in front of the emiiter
        /// Always returns true of 'waitForAlignment' is false!
        /// </summary>
        private bool isLockedOnTarget
        {
            get
            {
                if (!this.waitForAlignment) return true;

                Vector3 targetDir = this.targets[0].transform.position - this.emitter.position;
                Vector3 forward = this.emitter.forward;
                if (this.flatAngleCompare) targetDir.y = forward.y = 0; // Flaten Vectors
                float angle = Vector3.Angle(targetDir, forward);

                // Just for debug. Show a gizmo line to each target being tracked
                //   OnFire() has another color, so keep this here where this 
                //   won't overlay the OnFired line.
                if (this.debugLevel > DEBUG_LEVELS.Off)
                {
                    Debug.DrawRay(this.emitter.position, targetDir * 3, Color.cyan);
                    Debug.DrawRay(this.emitter.position, forward * 3, Color.cyan);
                }

                if (angle < this.lockOnAngleTolerance)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// For debugging, builds a nice comma-sepearted string from the target names
        /// </summary>
        private string targetsString
        {
            get
            {
                string[] names = new string[this.targets.Count];
                int i = 0;
                foreach (Target target in this.targets)
                {
                    names[i] = target.transform.name;
                    i++;
                }
                return System.String.Join(", ", names);
            }
        }
        #endregion Private Methods

    }
}