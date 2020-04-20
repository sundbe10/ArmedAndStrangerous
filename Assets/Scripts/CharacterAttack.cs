using UnityEngine;
using System.Collections;

public class CharacterAttack : MonoBehaviour
{
    Collider collider;

    private bool hasAttacked;

    // Use this for initialization
    void Start()
    {
        collider = GetComponent<BoxCollider>();
        collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerStay(Collider other)
    {
        if (hasAttacked) return;
        Damage(other.gameObject);
    }

    public  void Attack()
    {
        collider.enabled = true;
        StartCoroutine(AttackCoolDown());
    }

    void Damage(GameObject target)
    {
        float damage = 0;
        var weaponDamageScripts = transform.root.GetComponentsInChildren<WeaponDamage>();
        foreach(var weaponDamage in weaponDamageScripts)
        {
            damage += weaponDamage.damage;
        }

        Debug.Log(damage);
        var characterHealth = target.GetComponent<CharacterHealth>();
        if(characterHealth)
        {
            characterHealth.Hurt(damage);
            hasAttacked = true;
        }
    }

    IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        collider.enabled = false;
        hasAttacked = false;
    }
}
