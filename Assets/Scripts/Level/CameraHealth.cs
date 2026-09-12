using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHealth : EnemyBase
{
    [SerializeField] CameraProp camP;

    // Start is called before the first frame update
    void Start()
    {
        camP = gameObject.GetComponent<CameraProp>();
        base.Start();
        contactDamage = 0;
    }

    public override void TakeDamage(float damage, Vector2 knockback)
    {
        currentHealth -= damage;

        if (currentHealth < 0 || currentHealth == 0 && dead == false)
        {
            dead = true;
            SoundFXManager.instance.PlayRandomProximitySoundEffectClip(deathSounds, gameObject, enemyVol);
            player.GetComponent<Respawn>().deadEnemies.Add(this.gameObject);
            camP.Break();
        }

        return;
    }
}
