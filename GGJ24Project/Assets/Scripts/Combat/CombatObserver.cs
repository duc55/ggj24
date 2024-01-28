using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace LeftOut.GameJam
{
    public class CombatObserver : MonoBehaviour
    {
        private uint _lastSeenHitConnect;
        private HashSet<RagdollCombat> _allCombatants;
        public CombatObserver Instance { get; private set; }

        [SerializeField, Range(0.01f, 1f)]
        private float defaultRagdollHitboxCooldown;

        public UnityEvent<HitConnect, HitResolution> hitResolvedEvent;

        private bool _wasLogging;
        [BoxGroup("DEBUG"), SerializeField]
        private bool logHitResolution;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _lastSeenHitConnect = 0;
            HitConnect.ResetIds();
            HitResolution.ResetIds();
            foreach (var baby in InstanceRegistry<RagdollCombat>.All)
            {
                AddCombatant(baby);
            }
            if (logHitResolution)
                hitResolvedEvent.AddListener(LogHitResolution);
            _wasLogging = logHitResolution;
        }

        private void OnEnable()
        {
            _allCombatants = new HashSet<RagdollCombat>();
            InstanceRegistry<RagdollCombat>.OnAdd += AddCombatant;
            InstanceRegistry<RagdollCombat>.OnRemove += RemoveCombatant;
        }

        private void OnDisable()
        {
            InstanceRegistry<RagdollCombat>.OnAdd -= AddCombatant;
            InstanceRegistry<RagdollCombat>.OnRemove -= RemoveCombatant;
            if (logHitResolution)
                hitResolvedEvent.RemoveListener(LogHitResolution);
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
                return;

            if (_wasLogging != logHitResolution)
            {
                if(logHitResolution)
                    hitResolvedEvent.AddListener(LogHitResolution);
                else
                    hitResolvedEvent.RemoveListener(LogHitResolution);

                _wasLogging = logHitResolution;
            }
        }

        private void LogHitResolution(HitConnect hit, HitResolution result)
        {
            Debug.Log($"{hit} ==> {result}");
        }

        private void AddCombatant(RagdollCombat baby)
        {
            if (!Application.isPlaying)
                return;
            
            _allCombatants.Add(baby);
            baby.onHitConnect.AddListener(OnHitConnect);
        }
        
        private void RemoveCombatant(RagdollCombat baby)
        {
            if (!_allCombatants.Contains(baby))
                return;

            baby.onHitConnect.RemoveListener(OnHitConnect);
            _allCombatants.Remove(baby);
        }
        
        private void OnHitConnect(HitConnect hit)
        {
            if (hit.Id <= _lastSeenHitConnect)
            {
                Debug.LogWarning($"Stale HitConnect (id: {hit.Id}) observed. " +
                                 $"Ignoring because last observed was {_lastSeenHitConnect}");
                return;
            }
            if (hit.Id > _lastSeenHitConnect + 1)
            {
                Debug.LogWarning($"HitConnect with id {hit.Id} observed when last was {_lastSeenHitConnect}");
            }
            //Debug.Log($"{hit.Instigator.name} just hit {hit.Other.name}!");
            //hit.Hitbox.GoOnCooldownFor(defaultRagdollHitboxCooldown);
            // >>> TODO: Buffer these hits?
            _lastSeenHitConnect = hit.Id;
            hit.Hitbox.TurnOff();
            var hitResult = hit.Other.GetHit(hit);
            hit.Instigator.AttackSucceeded(hit, hitResult);
            hitResolvedEvent.Invoke(hit, hitResult);
        }
    }
}