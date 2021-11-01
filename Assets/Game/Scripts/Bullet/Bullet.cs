using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletPower;
    private float bulletSpeed;
    private float bulletSplashRadius;

    private Enemy targetEnemy;

    private void FixedUpdate()
    {
        if (PlayerData.IS_OVER)
        {
            return;
        }

        if (targetEnemy != null)
        {
            if (!targetEnemy.gameObject.activeSelf)
            {
                gameObject.SetActive(false);
                targetEnemy = null;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position, bulletSpeed * Time.fixedDeltaTime);

            Vector3 direction = targetEnemy.transform.position - transform.position;

            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetEnemy == null)
        {
            return;
        }

        if (collision.gameObject.Equals(targetEnemy.gameObject))
        {
            gameObject.SetActive(false);

            if (bulletSplashRadius > 0f)
            {
                LevelManager.Instance.ExplodeAt(transform.position, bulletSplashRadius, bulletPower);
            }
            else
            {
                targetEnemy.ReduceEnemyHealth(bulletPower);
            }

            targetEnemy = null;
        }
    }

    public void SetProperties(int bulletPower, float bulletSpeed, float bulletSplashRadius)
    {
        this.bulletPower = bulletPower;
        this.bulletSpeed = bulletSpeed;
        this.bulletSplashRadius = bulletSplashRadius;
    }

    public void SetTargetEnemy(Enemy enemy)
    {
        targetEnemy = enemy;
    }

}
