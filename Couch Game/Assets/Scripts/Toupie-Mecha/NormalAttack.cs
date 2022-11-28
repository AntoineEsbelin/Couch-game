using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{

    [System.Serializable] public class AttackSettings
    {
        [Header("Color")]
        public MeshRenderer mesh;
        public Color startingColor;
        [Header("Pre-Attack")]
        public float preAttackTimer;
        public float maxPreAttackTimer = 2f;
        public bool isPreparing;
    
        [Header("Attack")]
        public float attackTimer;
        public float maxAttackTimer = .5f;
        public bool isAttacking;
        public BoxCollider attackHitBox;
    }

    public AttackSettings attacksettings;
    private void OnEnable()
    {
        PrepareAttack();
    }

    private void OnDisable()
    {
        ResetAttack();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        PreAttackCD();
        Attack();
    }

    public void PrepareAttack()
    {
        attacksettings.preAttackTimer = attacksettings.maxPreAttackTimer;
        attacksettings.startingColor = attacksettings.mesh.material.color;
    }

    public void PreAttackCD()
    {
        if(attacksettings.isPreparing)
        {
            if(attacksettings.preAttackTimer > 0)
            {
                attacksettings.preAttackTimer -= Time.deltaTime;
                attacksettings.mesh.material.color = Color.yellow;
            }
            else
            {
                attacksettings.isAttacking = true;
                attacksettings.isPreparing = false;
            }
        }
    }

    public void Attack()
    {
        if(attacksettings.isAttacking)
        {
            if(attacksettings.attackTimer > 0)
            {
                //attack
                attacksettings.attackTimer -= Time.deltaTime;
                attacksettings.attackHitBox.gameObject.SetActive(true);
                attacksettings.mesh.material.color = Color.magenta;
            }   
            else
            {
                attacksettings.attackHitBox.gameObject.SetActive(false);
                attacksettings.isAttacking = false;
                attacksettings.mesh.material.color = attacksettings.startingColor;
            }
        }
    }

    public void ResetAttack()
    {
        //pre attack
        attacksettings.isPreparing = false;
        attacksettings.preAttackTimer = 0;

        //attack
        attacksettings.isAttacking = false;
        attacksettings.attackTimer = 0;
        
        //color
        attacksettings.mesh.material.color = attacksettings.startingColor;
    }
}
