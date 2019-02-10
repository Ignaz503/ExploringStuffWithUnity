using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    [SerializeField] float dmg = 5f;
    Vector3 lastPosition;

    Vector3 velocity;

    public override void Fire(Vector3 direction, float speed)
    {
        lastPosition = transform.position;
        velocity = direction * speed;
    }

    private void LateUpdate()
    {
        Move();
    }

    void Move()
    {

        //calculate new Position
        Vector3 newPos = lastPosition + velocity * Time.deltaTime;

        //check if hit anyting
        RaycastHit hit;
        Ray r = new Ray(lastPosition, (newPos - lastPosition).normalized);
        if(Physics.Raycast(r,out hit,(newPos-lastPosition).magnitude))
        {
            //see if damagable
            IDamagable d = hit.transform.gameObject.GetComponent<IDamagable>();
            if(d != null)
            {
                //DO damage
                d.Damage(new HitInfo(hit.point,hit.normal,dmg,hit.collider));
            }
            Destroy(gameObject);
        }
        else
        {
            //no hit
            lastPosition = newPos;
            transform.position = newPos;
            ModifyVelocity();
        }

    }

    void ModifyVelocity()
    {
        //velocity *= .9f;
    }

}
