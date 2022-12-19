using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject
{
    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Sends a message to the target GameObject. Returns Success.")]
    public class SendMessage : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The message to send")]
        public SharedString message;
        [Tooltip("The value to send")]
        public SharedString value;
        Damager Damager;
        public override void OnAwake()
        {
            base.OnAwake();

            Damager = GetDefaultGameObject(targetGameObject.Value).SearchComponent<Damager>();
        }

        public override TaskStatus OnUpdate()
        {
            if (value.Value != null) 
            {
                string passedString = value.Value.ToUpper();
                //GetDefaultGameObject(targetGameObject.Value).SendMessage(message.Value, value.Value.value.GetValue());
                if(passedString == "MELEE")
                {
                    Damager.EquipAttack(EAttackType.Melee);
                }
                else if(passedString == "SHOOT")
                {
                    Damager.EquipAttack(EAttackType.Shoot);
                }
            } 
            //else 
            //{
            //    GetDefaultGameObject(targetGameObject.Value).SendMessage(message.Value);
            //}

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            message = "";
        }
    }


    [TaskCategory("Unity/GameObject")]
    [TaskDescription("Sends a message to the target GameObject. Returns Success.")]
    public class SendMessageSubCategoryAttackType : Action
    {
        [Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [Tooltip("The message to send")]
        public SharedString message;
        [Tooltip("The value to send")]
        public SharedString value;
        Damager Damager;
        public override void OnAwake()
        {
            base.OnAwake();

            Damager = GetDefaultGameObject(targetGameObject.Value).SearchComponent<Damager>();
        }

        public override TaskStatus OnUpdate()
        {
            if (value.Value != null)
            {
                string passedString = value.Value.ToUpper();

                switch (Damager.EquippedAttack.AttackType)
                {

                    case EAttackType.Melee:
                        if(passedString == "SWORD")
                        {
                            Damager.EquipAttackMeleeSubCategory(EMeleeAttackType.Sword);
                        }
                        else if(passedString == "PUNCH")
                        {
                            Damager.EquipAttackMeleeSubCategory(EMeleeAttackType.Punch);

                        }
                        break;
                    case EAttackType.Shoot:
                        if(passedString == "SPELL")
                        {
                            Damager.EquipAttackShootSubCategory(EShootingAttackType.Spell);
                        }
                        else if(passedString == "ARROW")
                        {
                            Damager.EquipAttackShootSubCategory(EShootingAttackType.Arrow);

                        }
                        break;
                }
            }

            return TaskStatus.Success;
        }

        public override void OnReset()
        {
            targetGameObject = null;
            message = "";
        }
    }
}