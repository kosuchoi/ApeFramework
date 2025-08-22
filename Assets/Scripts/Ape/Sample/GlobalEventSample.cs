using System;
using Ape.Runtime.GlobalEvent;
using UnityEngine;
using UnityEngine.UI;

namespace Ape.Sample
{
    public enum BattleEventType
    {
        OnAttack,
        OnDamaged,
    }

    public class BattleEventParam : GlobalEventParamBase
    {
        public override Enum EventType => BattleEventType;
        public BattleEventType BattleEventType { get; private set; }
        public string CharacterName { get; private set; }
        
        public BattleEventParam(BattleEventType battleEventType, string characterName)
        {
            BattleEventType = battleEventType;
            CharacterName = characterName;
        }
    }
    
    public enum GameEventType
    {
        OnGamePaused,
    }

    public class GlobalEventSample : MonoBehaviour
    {
        private void Awake()
        {
            transform.Find("Canvas/VerticalLayoutGroup/AttackButton").GetComponent<Button>().onClick
                .AddListener(OnAttackButtonClick);
            transform.Find("Canvas/VerticalLayoutGroup/DamagedButton").GetComponent<Button>().onClick
                .AddListener(OnDamgedButtonClick);
            transform.Find("Canvas/VerticalLayoutGroup/PauseButton").GetComponent<Button>().onClick
                .AddListener(OnPauseButtonClick);
            transform.Find("Canvas/VerticalLayoutGroup/ResumeButton").GetComponent<Button>().onClick
                .AddListener(OnResumeButtonClick);
        }
        
        private void OnEnable()
        {
            GlobalEventManager.Instance.AddAllListeners<BattleEventType>(OnBattleEventTriggered);
            GlobalEventManager.Instance.TryAddListener(GameEventType.OnGamePaused, OnGamePaused);
        }

        private void OnDisable()
        {
            if (GlobalEventManager.Instance)
            {
                GlobalEventManager.Instance.RemoveObjectListeners(this);
            }
        }
        
        private void OnBattleEventTriggered(GlobalEventParamBase param)
        {
            BattleEventParam battleEventParam = param as BattleEventParam;
            Debug.Log($"OnBattleEventTriggered() BattleEventType:{battleEventParam.BattleEventType}, CharacterName:{battleEventParam.CharacterName}" );
        }

        private void OnGamePaused(GlobalEventParamBase param)
        {
            GlobalEventParam<bool> globalEventParam = param as GlobalEventParam<bool>;
            Debug.Log($"[GameEvent] Game Paused! value:{globalEventParam.Value}");
        }

        private void OnAttackButtonClick()
        {
            GlobalEventManager.Instance.Trigger(new BattleEventParam(BattleEventType.OnAttack,"Noname"));
        }

        private void OnDamgedButtonClick()
        {
            GlobalEventManager.Instance.Trigger(new BattleEventParam(BattleEventType.OnDamaged, "Noname"));
        }
        
        // 게임 일시정지
        private void OnPauseButtonClick()
        {
            GlobalEventManager.Instance.Trigger(new GlobalEventParam<bool>(GameEventType.OnGamePaused, true));
        }

        // 게임 재개
        private void OnResumeButtonClick()
        {
            GlobalEventManager.Instance.Trigger(new GlobalEventParam<bool>(GameEventType.OnGamePaused, false));
        }
    }
}