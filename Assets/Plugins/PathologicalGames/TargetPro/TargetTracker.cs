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
    [AddComponentMenu("Path-o-logical/TargetPro/TargetTracker")]
    public class TargetTracker : MonoBehaviour
    {

        #region Parameters

        #region Inspector Fields
        /// <summary>
        /// The number of targets to return. Set to -1 to return all targets
        /// </summary>
        public int numberOfTargets = 1;

        /// <summary>
        /// The style of sorting for the perimeter to use
        /// </summary>
        public SORTING_STYLES sortingStyle
        {
            get { return _sortingStyle; }
            set
            {
                this._sortingStyle = value;

                // Trigger sorting due to this change
                if (this.perimeter != null) this.perimeter.dirty = true;
            }
        }
        [SerializeField]  // Private backing fields must be SerializeField. For instances.
        private SORTING_STYLES _sortingStyle = SORTING_STYLES.Nearest;

        /// <summary>
        /// How often the target list will be sorted. If set to 0, sorting will only be 
        /// triggered when Targets enter or exit range.
        /// </summary>
        public float sortInterval = 0.1f;

        /// <summary>
        /// The range in which targets will be found.
        /// The size from the center to the edge of the perimeter in x, y and z for any 
        /// shape. Depending on the shape some values may be ignored. E.g. Spheres only 
        /// use X for radius
        /// </summary>
        public Vector3 range
        {
            get { return this._range; }
            set
            {
                // Store the passed Vector3 then process the range per collider type
                this._range = value;

                if (this.perimeter != null) this.UpdatePerimeterRange(); // For editor
            }
        }
        [SerializeField]  // Private backing fields must be SerializeField. For instances.
        private Vector3 _range = Vector3.one;

        /// <summary>
        /// The layers in which the perimeter is allowed to find targets.
        /// </summary>
        public LayerMask targetLayers;

        /// <summary>
        /// Perimeter shape options
        /// </summary>
        public enum PERIMETER_SHAPES
        {
            Capsule = 0,
            Box = 1,
            Sphere = 2
        }

        /// <summary>
        /// The shape of the perimeter used to detect targets in range
        /// </summary>
        public PERIMETER_SHAPES perimeterShape
        {
            get { return this._perimeterShape; }
            set
            {
                this._perimeterShape = value;

                // Just in case this is called before Awake runs.
                if (this.perimeter == null) return;
                this.UpdatePerimeterShape();
            }
        }
        [SerializeField]  // Private backing fields must be SerializeField. For instances.
        private PERIMETER_SHAPES _perimeterShape = PERIMETER_SHAPES.Sphere;

        /// <summary>
        /// An optional position offset for the perimeter. 
        /// For example, if you have an object resting on the ground which has a range of 
        /// 4, a position offset of Vector3(0, 4, 0) will place your perimeter so it is 
        /// also sitting on the ground
        /// </summary>
        public Vector3 perimeterPositionOffset
        {
            get { return this._perimeterPositionOffset; }
            set
            {
                this._perimeterPositionOffset = value;

                // Just in case this is called before Awake runs.
                if (this.perimeter == null) return;
                this.perimeter.transform.localPosition = value;
            }
        }
        [SerializeField]  // Private backing fields must be SerializeField. For instances.
        private Vector3 _perimeterPositionOffset = Vector3.zero;

        /// <summary>
        /// An optional rotational offset for the perimeter.
        /// </summary>
        public Vector3 perimeterRotationOffset
        {
            get { return this._perimeterRotationOffset; }
            set
            {
                this._perimeterRotationOffset = value;

                // Just in case this is called before Awake runs.
                if (this.perimeter == null) return;
                this.perimeter.transform.localRotation = Quaternion.Euler(value);
            }
        }
        [SerializeField]  // Private backing fields must be SerializeField. For instances.
        private Vector3 _perimeterRotationOffset = Vector3.zero;


        /// <summary>
        /// The layer to put the perimeter in. 0 is the default layer.
        /// </summary>
        public int perimeterLayer
        {
            get { return this._perimeterLayer; }
            set
            {
                this._perimeterLayer = value;

                // Just in case this is called before Awake runs.
                if (this.perimeter == null) return;
                this.perimeter.gameObject.layer = value;

            }
        }
        [SerializeField]  // Private backing fields must be SerializeField. For instances.
        private int _perimeterLayer = 0;

        /// <summary>
        /// Set to get visual and log feedback of what is happening in this TargetTracker
        /// and perimeter
        /// </summary>
        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        /// <summary>
        /// Displays the perimeter gizmo
        /// </summary>
        public bool drawGizmo = false;

        /// <summary>
        /// The color of the gizmo when displayed
        /// </summary>
        public Color gizmoColor = new Color(0, 0.7f, 1, 1); // Cyan-blue mix
        public Color defaultGizmoColor { get { return new Color(0, 0.7f, 1, 1); } }

        // Used by inspector scripts as an override to hide the gizmo
        public bool overrideGizmoVisibility = false;
        #endregion Inspector Fields

        /// <summary>
        /// Access to the perimeter component/list
        /// </summary>
        public Perimeter perimeter { get; private set; }

        /// <summary>
        /// A list of sorted targets. The contents depend on numberOfTargets requested
        /// (-1 for all targets in the perimeter), and the sorting style userd.
        /// </summary>
        public virtual TargetList targets
        {
            get
            {
                // Start with an empty list each pass.
                this._targets.Clear();

                // Just in case this is called before Awake runs.
                if (this.perimeter == null) return this._targets;

                // If none are wanted or no targets available, quit
                if (this.numberOfTargets == 0 || this.perimeter.Count == 0)
                    return this._targets;

                // None == Area-of-effect, so get everything in range, otherwise, 
                //   Get the first item(s). Since everything is sorted based on the
                //   sortingStyle, the first item(s) will always work
                if (this.numberOfTargets == -1)
                {
                    this._targets.AddRange(this.perimeter);
                }
                else
                {
                    // Grab the first item(s)
                    int num = Mathf.Clamp(this.numberOfTargets, 0, this.perimeter.Count);
                    for (int i = 0; i < num; i++)
                        this._targets.Add(this.perimeter[i]);
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
        protected TargetList _targets = new TargetList();

        #endregion Perameters



        #region Cache and Setup
        public Transform xform;

        protected virtual void Awake()
        {
            this.xform = this.transform;

            #region Build Perimter
            // PERINETER GAME OBJECT
            // Create the perimeter object at run-time. The name is really just for debugging
            GameObject perimeterGO = new GameObject(this.name + "_Perimeter");
            perimeterGO.transform.parent = this.xform;
            //perimeterGO.hideFlags = HideFlags.HideInHierarchy;
            perimeterGO.SetActive(false);
            perimeterGO.SetActive(true);

            // These are set by properties but need to be synced when first generated
            perimeterGO.transform.localPosition = this.perimeterPositionOffset;
            perimeterGO.transform.localRotation = Quaternion.Euler(this.perimeterRotationOffset);
            perimeterGO.layer = this.perimeterLayer;

            // PERINETER COLLIDER

            // PERIMETER COMPONENT
            // Add the Perimeter script and return it
            this.perimeter = perimeterGO.AddComponent<Perimeter>();
            this.perimeter.targetTracker = this;

            // If not derrived and altered, this will turn on next in enable
            this.perimeter.enabled = false;

            // INIT OTHER
            this.UpdatePerimeterShape();  // Adds the collider
            this.UpdatePerimeterRange();  // Sets the collider's size
            #endregion Build Perimter
        }

        /// <summary>
        /// Update the perimeter range buy removing the old collider and making a new one.
        /// </summary>
        private void UpdatePerimeterShape()
        {
            GameObject perimeterGO = this.perimeter.gameObject;

            // Remove the old collider
            Collider oldCol = perimeterGO.GetComponent<Collider>();

            // If the current collider is already the correct component, then quit
            switch (this.perimeterShape)
            {
                case PERIMETER_SHAPES.Sphere:
                    if (oldCol is SphereCollider)
                        return;
                    break;

                case PERIMETER_SHAPES.Box:
                    if (oldCol is BoxCollider)
                        return;
                    break;

                case PERIMETER_SHAPES.Capsule:
                    if (oldCol is CapsuleCollider)
                        return;
                    break;
            }


            if (oldCol != null) Destroy(oldCol);

            // Add the new collider
            Collider col = null;
            switch (this.perimeterShape)
            {
                case PERIMETER_SHAPES.Sphere:
                    col = perimeterGO.AddComponent<SphereCollider>();
                    break;

                case PERIMETER_SHAPES.Box:
                    col = perimeterGO.AddComponent<BoxCollider>();
                    break;

                case PERIMETER_SHAPES.Capsule:
                    col = perimeterGO.AddComponent<CapsuleCollider>();
                    break;
            }

            col.isTrigger = true; // No collisions

            // Trigger sorting due to this change
            this.perimeter.dirty = true;
        }

        /// <summary>
        /// Sets the range based on this._range and the collider type
        /// </summary>
        private void UpdatePerimeterRange()
        {
            Vector3 normRange = this.GetNormalizedRange();
            Collider col = this.perimeter.collider;
            if (col is SphereCollider)
            {
                var collider = (SphereCollider)col;
                collider.radius = normRange.x;
            }
            else if (col is BoxCollider)
            {
                var collider = (BoxCollider)col;
                collider.size = normRange;
            }
            else if (col is CapsuleCollider)
            {
                var collider = (CapsuleCollider)col;
                collider.radius = normRange.x;
                collider.height = normRange.y;
            }
            else
            {
                Debug.LogWarning("Unsupported collider type.");
            }

            // Trigger sorting due to this change
            this.perimeter.dirty = true;
        }

        /// <summary>
        /// Calculate the range based on the collider shape. The user input creates
        /// different results depending on the shape used. A sphere radius is different
        /// than a box width. This function creates a mapping which normalizes the result.
        /// The shape is a string so this can be used before the collider exits
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetNormalizedRange()
        {
            Vector3 normRange = Vector3.zero;
            switch (this.perimeterShape)
            {
                case PERIMETER_SHAPES.Sphere:
                    normRange = new Vector3
                    (
                        this._range.x,
                        this._range.x,
                        this._range.x
                    );
                    break;

                case PERIMETER_SHAPES.Box:
                    normRange = new Vector3
                    (
                        this._range.x * 2,
                        this._range.y,
                        this._range.z * 2
                    );
                    break;

                case PERIMETER_SHAPES.Capsule:
                    normRange = new Vector3
                    (
                        this._range.x,
                        this._range.y * 2,  // Capsules work this way
                        this._range.x
                    );
                    break;
            }

            return normRange;
        }


        protected virtual void OnEnable()
        {
            this.perimeter.enabled = true;
        }


        protected virtual void OnDisable()
        {
            // Needed to avoid error when stoping the game or if the perimeter was destroyed 
            //   for some reason.
            if (this.perimeter == null) return;

            this.perimeter.Clear();
            this.perimeter.enabled = false;
        }

        #endregion Cache and Setup



        #region Sorting Options and Functionality
        /// <summary>
        /// Target Style Options used to keep targets sorted
        /// </summary>
        public enum SORTING_STYLES
        {
            None = 0,           // Also good for area-of-effects where no sort is needed
            Nearest = 1,       // Closest to the perimter's center (localPostion)
            Farthest = 2,      // Farthest from the perimter's center (localPostion)
            NearestToDestination = 3,    // Nearest to a destination along waypoints
            FarthestFromDestination = 4, // Farthest from a destination along waypoints
            MostPowerful = 5,  // Most powerful based on a iTargetable parameter
            LeastPowerful = 6, // Least powerful based on a iTargetable parameter
        }

        public delegate void OnPostSortDelegate(TargetList targets);
        protected OnPostSortDelegate onPostSortDelegates;

        #region OnPostSortDelegate Add/Set/Remove
        /// <summary>
        /// Runs just before returning the targets list to allow for custom sorting.
        /// The delegate signature is:  delegate(TargetList targets)
        /// See TargetTracker documentation for usage of the provided '...'
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        public void AddOnPostSortDelegate(OnPostSortDelegate del)
        {
            this.onPostSortDelegates += del;
        }

        /// <summary>
        /// This replaces all older delegates rather than adding a new one to the list.
        /// See docs for AddOnPostSortDelegate()
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        public void SetOnPostSortDelegate(OnPostSortDelegate del)
        {
            this.onPostSortDelegates = del;
        }

        /// <summary>
        /// Removes a OnPostSortDelegate
        /// See docs for AddOnPostSortDelegate()
        /// </summary>
        /// <param name="del">An OnPostSortDelegate</param>
        public void RemoveOnPostSortDelegate(OnPostSortDelegate del)
        {
            this.onPostSortDelegates -= del;
        }
        #endregion OnPostSortDelegates Add/Set/Remove

        #endregion Sorting Options and Functionality



        #region Comparers for Perimter.Sort()
        /// <summary>
        /// The interface for all target comparers.
        /// </summary>
        public interface iTargetComparer : IComparer<Target>
        {
            new int Compare(Target targetA, Target targetB);
        }


        /// <summary>
        /// Returns a comparer based on a targeting style.
        /// </summary>
        public class TargetComparer : iTargetComparer
        {
            private Transform perimeterPos;
            private TargetTracker.SORTING_STYLES sortStyle;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sortStyle">The target style used to return a comparer</param>
            /// <param name="perimeterPos">Position for distance-based sorting</param>
            public TargetComparer(TargetTracker.SORTING_STYLES sortStyle, Transform perimeterPos)
            {
                this.perimeterPos = perimeterPos;
                this.sortStyle = sortStyle;
            }


            /// <summary>
            /// Used by List.Sort() to custom sort the perimeter list.
            /// </summary>
            /// <param name="targetA">The first object for comparison</param>
            /// <param name="targetB">The second object for comparison</param>
            /// <returns></returns>
            public int Compare(Target targetA, Target targetB)
            {
                switch (this.sortStyle)
                {
                    case SORTING_STYLES.Farthest:
                        float na = targetA.targetable.GetDistToPos(this.perimeterPos.position);
                        float nb = targetB.targetable.GetDistToPos(this.perimeterPos.position);
                        return nb.CompareTo(na);


                    case SORTING_STYLES.Nearest:
                        float fa = targetA.targetable.GetDistToPos(this.perimeterPos.position);
                        float fb = targetB.targetable.GetDistToPos(this.perimeterPos.position);
                        return fa.CompareTo(fb);


                    case SORTING_STYLES.FarthestFromDestination:
                        return targetB.targetable.distToDest.CompareTo(
                                                            targetA.targetable.distToDest);


                    case SORTING_STYLES.NearestToDestination:
                        return targetA.targetable.distToDest.CompareTo(
                                                            targetB.targetable.distToDest);


                    case SORTING_STYLES.LeastPowerful:
                        return targetA.targetable.strength.CompareTo(
                                                            targetB.targetable.strength);

                    case SORTING_STYLES.MostPowerful:
                        return targetB.targetable.strength.CompareTo(
                                                            targetA.targetable.strength);


                    case TargetTracker.SORTING_STYLES.None:  // Only in error
                        throw new System.NotImplementedException("Unexpected option. " +
                                "SORT_OPTIONS.NONE should bypass sorting altogether.");
                }

                // Anything unexpected
                throw new System.NotImplementedException(
                               string.Format("Unexpected option '{0}'.", this.sortStyle));
            }
        }
        #endregion Target Comparers for Perimter.Sort()

    }

}