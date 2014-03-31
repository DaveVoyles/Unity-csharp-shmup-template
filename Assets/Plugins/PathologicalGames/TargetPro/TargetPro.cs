/// <Licensing>
/// © 2011 (Copyright) Path-o-logical Games, LLC
/// If purchased from the Unity Asset Store, the following license is superseded 
/// by the Asset Store license.
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>

#define POOLMANAGER_INSTALLED  // Uncomment if you have PoolManager. Use Pool "Ammo"

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PathologicalGames
{
    /// <summary>
    /// Options used to print a stream of messages for the implimenting conponent
    /// </summary>
    public enum DEBUG_LEVELS { Off, Normal, High }

    /// <summary>
    /// Methods which interface with PoolManager if installed and the preprocessor
    /// directive at the top of this file is uncommented. Otherwise will run 
    /// Unity's Instantiate() and Destroy() based functionality.
    /// </summary>
    public static class InstanceManager
    {
        public static string poolName = "Projectiles";

        #region Static Methods
        /// <summary>
        /// Creates a new instance. 
        /// 
        /// If PoolManager is installed and the pre-proccessor directive is 
        /// uncommented at the top of TargetTracker.cs, this will use PoolManager to 
        /// pool ammo instances.
        /// 
        /// If the pool doesn't exist before this is used, it will be created.
        /// 
        /// Otherwise, Unity's Object.Instantiate() is used.
        /// </summary>
        /// <param name="prefab">The prefab to spawn an instance from</param>
        /// <param name="pos">The position to spawn the instance</param>
        /// <param name="rot">The rotation of the new instance</param>
        /// <returns></returns>
        public static Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
        {
#if POOLMANAGER_INSTALLED
            // If the pool doesn't exist, it will be created before use
            if (!PoolManager.Pools.ContainsKey(poolName))
                (new GameObject(poolName)).AddComponent<SpawnPool>();

            return PoolManager.Pools[poolName].Spawn(prefab, pos, rot);
#else
             return (Transform)Object.Instantiate(prefab, pos, rot);
#endif
        }


        /// <summary>
        /// Despawnsan instance. 
        /// 
        /// If PoolManager is installed and the pre-proccessor directive is 
        /// uncommented at the top of TargetTracker.cs, this will use PoolManager
        /// to despawn pooled ammo instances.
        /// 
        /// Otherwise, Unity's Object.Destroy() is used.
        /// </summary>
        public static void Despawn(Transform instance)
        {
#if POOLMANAGER_INSTALLED
            PoolManager.Pools[poolName].Despawn(instance);
#else
            Object.Destroy(instance.gameObject);
#endif
        }
    }
    #endregion Static Methods


    #region Classes, Structs, Interfaces
    /// <summary>
    /// Carries information about a target including a reference to its Targetable
    /// component (targetable), Transform (transform) and GameObject (gameObject) as 
    /// well as the source TargetTracker (targetTracker) and 
    /// FireController (fireController) components. FireController is null if not used.
    /// </summary>
    public struct Target : System.IComparable<Target>
    {
        // Target cache
        public GameObject gameObject;
        public Transform transform;
        public Targetable targetable;

        // Source cache for easy access/reference passing/etc
        public TargetTracker targetTracker;
        public FireController fireController;
        public Projectile projectile;

        /// <summary>
        /// A cached "empty" Target struct which can be used for null-like 'if' checks
        /// </summary>
        public static Target Null { get { return _Null; } }
        private static Target _Null = new Target();

        public Target(Transform transform, TargetTracker targetTracker)
        {
            this.gameObject = transform.gameObject;
            this.transform = transform;
            this.targetable = this.transform.GetComponent<Targetable>();

            this.targetTracker = targetTracker;

            // The targetTracker arg could also be a projectile because it is derrived
            //   from a TargetTracker
            if (targetTracker is Projectile)
                this.projectile = (Projectile)targetTracker;
            else
                this.projectile = null;

            this.fireController = null;
        }

        // Init by copy
        public Target(Target otherTarget)
        {
            this.gameObject = otherTarget.gameObject;
            this.transform = otherTarget.transform;
            this.targetable = otherTarget.targetable;

            this.targetTracker = otherTarget.targetTracker;
            this.fireController = otherTarget.fireController;
            this.projectile = otherTarget.projectile;

        }

        public static bool operator ==(Target tA, Target tB)
        {
            return tA.gameObject == tB.gameObject;
        }

        public static bool operator !=(Target tA, Target tB)
        {
            return tA.gameObject != tB.gameObject;
        }

        // These are required to shut the cimpiler up when == or != is overriden
        // This are implimented as recomended by the msdn documentation.
        public override int GetHashCode(){ return base.GetHashCode(); }
        public override bool Equals(System.Object other) 
        {
            if (other == null) return false;
            return this == (Target)other; // Uses overriden ==
        }

        /// <summary>
        /// Returns true if the target is in a spawned state. Spawned means the target 
        /// is not null (not destroyed) and it IS enabled (not despawned by an instance 
        /// pooling system like PoolManager)
        /// </summary>
        public bool isSpawned 
        { 
            get 
            {
                // Null means destroyed so false, if pooled, disabled is false
                return this.gameObject == null ? false : this.gameObject.activeInHierarchy;
            } 
        }

        /// <summary>
        /// For internal use only.
        /// The default for IComparable. Will test if this target is equal to another
        /// by using the GameObject reference equality test
        /// </summary>
        public int CompareTo(Target obj)
        {
            return this.gameObject == obj.gameObject ? 1 : 0;
        }

    }

    /// <summary>
    /// A custom List implimentation to allow for user-friendly usage and ToString() 
    /// representation as well as an extra level of abstraction for future 
    /// extensibility
    /// </summary>
    public class TargetList : List<Target>
    {
        public override string ToString()
        {
            string[] names = new string[base.Count];
            int i = 0;
			Target target;
			IEnumerator<Target> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				target = enumerator.Current;
	            if (target.transform == null)
	                continue;

	            names[i] = target.transform.name;
	            i++;
			}

            return System.String.Join(", ", names);
        }
    }

    /// <summary>
    /// Used to pass information to a target when it is hit
    /// </summary>
    public struct HitEffect
    {
        public string name;
        public float value;
        public float duration;
        public float hitTime;

        /// <summary>
        /// Copy constructor to populate a new struct with an old
        /// </summary>
        /// <param name="hitEffect"></param>
        public HitEffect(HitEffect hitEffect)
        {
            this.name = hitEffect.name;
            this.value = hitEffect.value;
            this.duration = hitEffect.duration;
            this.hitTime = hitEffect.hitTime;
        }

        /// <summary>
        /// This returns how much of the duration is left based on the current time
        /// and the hitTime (time stamped by Targetable OnHit)
        /// </summary>
        public float deltaDurationTime
        {
            get 
            {
                // If smaller than 0, return 0.
                return Mathf.Max((hitTime + duration) - Time.time, 0);
            }
        }

        public override string ToString()
        {
            return string.Format
            (
                "(name '{0}', value {1}, duration {2}, hitTime {3}, deltaDurationTime {4})",
                this.name, 
                this.value,
                this.duration,
                this.hitTime,
                this.deltaDurationTime
            );
        }
    }


    /// <summary>
    /// A custom List implimentation to allow for user-friendly usage and ToString() 
    /// representation as well as an extra later of abstraction for futre 
    /// extensibility
    /// </summary>
    public class HitEffectList : List<HitEffect>
    {
        // Impliment both constructors to enable the 1 arg copy-style initilizer
        public HitEffectList() : base() { }
        public HitEffectList(HitEffectList hitEffectList) : base(hitEffectList) {}

        /// <summary>
        /// Print a nice message showing the contents of this list.
        /// </summary>
        public override string ToString()
        {
            string[] effectStrings = new string[base.Count];
            int i = 0;
			HitEffect effect;
			IEnumerator<HitEffect> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				effect = enumerator.Current;
				effectStrings[i] = effect.ToString();				
				i++;
			}

            return System.String.Join(", ", effectStrings);
        }


        /// <summary>
        /// Get a copy of this list with effects hitTime set to 'now'.
        /// </summary>
        /// <returns>HitEffectList</returns>
        public HitEffectList CopyWithHitTime()
        {
            var newlist = new HitEffectList();
			HitEffect effect;
			IEnumerator<HitEffect> enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				effect = enumerator.Current;
				effect.hitTime = Time.time;
				newlist.Add(effect);
			}

            return newlist;
        }
    }


    /// <summary>
    /// Used by the Editor custom Inspector to get user input
    /// </summary>
    [System.Serializable]
    public class HitEffectGUIBacker
    {
        public string name = "Effect";
        public float value = 0;
        public float duration;

        /// <summary>
        /// Create a new HitEffectGUIBacker with default values
        /// </summary>
        /// <returns></returns>
        public HitEffectGUIBacker() { }

        /// <summary>
        /// Create a new HitEffectGUIBacker from a HitEffect struct
        /// </summary>
        /// <returns></returns>
        public HitEffectGUIBacker(HitEffect effect)
        {
            this.name = effect.name;
            this.value = effect.value;
            this.duration = effect.duration;
        }

        /// <summary>
        /// Return a HitEffect struct.
        /// </summary>
        /// <returns></returns>
        public HitEffect GetHitEffect()
        {
            return new HitEffect
            {
                name = this.name,
                value = this.value,
                duration = this.duration,
            };
        }
    }

    #endregion Classes, Structs, Interfaces
}