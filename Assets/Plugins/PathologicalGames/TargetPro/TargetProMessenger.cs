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
    ///	Provides messaging for event delegates primarily for use with UnityScript since only
    ///	C# has delegates
    /// </summary>
    [AddComponentMenu("Path-o-logical/TargetPro/Messenger")]
    public class TargetProMessenger : MonoBehaviour
    {
        public enum COMPONENTS
        {
            FireController,
            Projectile,
            Targetable
        }
        public COMPONENTS forComponent;


        public enum MESSAGE_MODE
        {
            Send,
            Broadcast
        }
        public MESSAGE_MODE messageMode;

        /// <summary>
        /// An optional GameObject to message instead of this component's GameObject
        /// </summary>
        public GameObject otherTarget = null;

        public DEBUG_LEVELS debugLevel = DEBUG_LEVELS.Off;

        // FireController Events
        public bool fireController_OnStart = false;
        public bool fireController_OnUpdate = false;
        public bool fireController_OnTargetUpdate = false;
        public bool fireController_OnIdleUpdate = false;
        public bool fireController_OnFire = false;
        public bool fireController_OnStop = false;

        // Projectile Events
        public bool projectile_OnLaunched = false;
        public bool projectile_OnLaunchedUpdate = false;
        public bool projectile_OnDetonation = false;


        // Targetable Events
        public bool targetable_OnHit = false;
        public bool targetable_OnDetected = false;
        public bool targetable_OnNotDetected = false;

        private void Awake()
        {
            // Register all to allow run-time changes to individual events
            //  The delegates don't send messages unless the bool is true anyway.
            this.RegisterFireController();
            this.RegisterProjectile();
            this.RegisterTargetable();
        }


        /// <summary>
        /// Generic message handler to process options.
        /// </summary>
        /// <param name="msg"></param>
        private void handleMsg(string msg)
        {
            GameObject GO;
            if (this.otherTarget == null)
                GO = this.gameObject;
            else
                GO = this.otherTarget;

            if (this.debugLevel > DEBUG_LEVELS.Off)
                Debug.Log(string.Format("Sending message '{0}' to '{1}'", msg, GO));

            if (this.messageMode == MESSAGE_MODE.Send)
                GO.SendMessage(msg, SendMessageOptions.DontRequireReceiver);
            else
                GO.BroadcastMessage(msg, SendMessageOptions.DontRequireReceiver);
        }

        /// <summary>
        /// Generic message handler to process options.
        /// </summary>
        /// <param name="msg"></param>
        private void handleMsg<T>(string msg, T arg)
        {
            GameObject GO;
            if (this.otherTarget == null)
                GO = this.gameObject;
            else
                GO = this.otherTarget;

            if (this.debugLevel > DEBUG_LEVELS.Off)
                Debug.Log
                (
                    string.Format("Sending message '{0}' to '{1}' with argument {2}",
                    msg,
                    GO,
                    arg)
                );

            if (this.messageMode == MESSAGE_MODE.Send)
                GO.SendMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
            else
                GO.BroadcastMessage(msg, arg, SendMessageOptions.DontRequireReceiver);
        }



        # region FireController
        private void RegisterFireController()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<FireController>();
            if (component == null) return;

            component.AddOnStartDelegate(this.OnStartDelegate);
            component.AddOnUpdateDelegate(this.OnUpdateDelegate);
            component.AddOnTargetUpdateDelegate(this.OnTargetUpdateDelegate);
            component.AddOnIdleUpdateDelegate(this.OnIdleUpdateDelegate);
            component.AddOnFireDelegate(this.OnFireDelegate);
            component.AddOnStopDelegate(this.OnStopDelegate);
        }

        private void OnStartDelegate()
        {
            if (fireController_OnStart == false) return;
            this.handleMsg("FireController_OnStart");
        }

        private void OnUpdateDelegate()
        {
            if (fireController_OnUpdate == false) return;
            this.handleMsg("FireController_OnUpdate");
        }

        private void OnTargetUpdateDelegate(List<Target> targets)
        {
            if (fireController_OnTargetUpdate == false) return;
            this.handleMsg<List<Target>>("FireController_OnTargetUpdate", targets);
        }

        private void OnIdleUpdateDelegate()
        {
            if (fireController_OnIdleUpdate == false) return;
            this.handleMsg("FireController_OnIdleUpdate");
        }

        private void OnFireDelegate(List<Target> targets)
        {
            if (fireController_OnFire == false) return;
            this.handleMsg<List<Target>>("FireController_OnFire", targets);
        }

        private void OnStopDelegate()
        {
            if (fireController_OnStop == false) return;
            this.handleMsg("Projectile_OnStop");
        }
        # endregion FireController



        # region Projectile
        private void RegisterProjectile()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<Projectile>();
            if (component == null) return;

            component.AddOnLaunchedDelegate(this.OnLauchedDelegate);
            component.AddOnLaunchedUpdateDelegate(this.LaunchedUpdateDelegate);
            component.AddOnDetonationDelegate(this.OnDetonationDelegate);
        }

        private void OnLauchedDelegate()
        {
            if (projectile_OnLaunched == false) return;
            this.handleMsg("Projectile_OnLauched");
        }

        private void LaunchedUpdateDelegate()
        {
            if (projectile_OnLaunchedUpdate == false) return;
            this.handleMsg("Projectile_LaunchedUpdate");
        }

        private void OnDetonationDelegate(List<Target> targets)
        {
            if (projectile_OnDetonation == false) return;
            this.handleMsg<List<Target>>("Projectile_OnDetonation", targets);
        }
        # endregion Projectile



        # region Targetable
        private void RegisterTargetable()
        {
            // There is no need to initialize delegates if there is no component
            var component = this.GetComponent<Targetable>();
            if (component == null) return;

            component.AddOnHitDelegate(this.OnHitDelegate);
            component.AddOnDetectedDelegate(this.OnDetectedDelegate);
            component.AddOnNotDetectedDelegate(this.OnNotDetectedDelegate);
        }

        private void OnHitDelegate(HitEffectList effects, Target target)
        {
            if (targetable_OnHit == false) return;

            // Pack the data in to a struct since we can only pass one argument
            var data = new MessageData_TargetableOnHit(effects, target);

            this.handleMsg<MessageData_TargetableOnHit>("Targetable_OnHit", data);
        }

        private void OnDetectedDelegate(TargetTracker source)
        {
            if (targetable_OnDetected == false) return;

            this.handleMsg<TargetTracker>("Targetable_OnDetected", source);
        }

        private void OnNotDetectedDelegate(TargetTracker source)
        {
            if (targetable_OnNotDetected == false) return;

            this.handleMsg<TargetTracker>("Targetable_OnNotDetected", source);
        }
        # endregion Targetable

    }

    /// <summary>
    /// This is used pass both arguments for the Projectile OnHit delegate because Unity's
    /// SendMessage only takes a single argument
    /// </summary>
    public struct MessageData_TargetableOnHit
    {
        public HitEffectList effects;
        public Target target;

        public MessageData_TargetableOnHit(HitEffectList effects, Target target)
        {
            this.effects = effects;
            this.target = target;
        }
    }
}