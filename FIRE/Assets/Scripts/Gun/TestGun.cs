using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestGun : BaseGun
{
    public override bool IsThrowable { get { return false; } }

    protected override bool canFire { get { return currentCooldown <= 0f; } }

    [SerializeField] Vector3 localBulletSpawnPoint = Vector3.zero;
    [SerializeField] GameObject bullet = null;
    [Tooltip("Meters per second")]
    [SerializeField] float bulletSpeed = 30;
    Camera playerCam;
    [SerializeField]Rigidbody rb;
    [SerializeField] float fireDelay = .5f;
    float currentCooldown;

    public override void DropHeldItem()
    {
        rb = gameObject.AddComponent<Rigidbody>();
    }

    public override void OnPlacedInItemHolder(ItemHolder holder)
    {
        Destroy(rb);
        playerCam = holder.PlayerCamera;
        transform.SetParent(holder.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(new Vector3(0,-90,-90));
        currentCooldown = fireDelay;
    }

    void SpawnBullet()
    {
        GameObject obj = Instantiate(bullet, transform.TransformPoint(localBulletSpawnPoint), transform.rotation);

        Vector3 dir = playerCam.ViewportPointToRay(new Vector3(.5f, .5f)).direction;
        obj.GetComponent<Bullet>().Fire(dir, bulletSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.TransformPoint(localBulletSpawnPoint), .1f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.TransformPoint(localBulletSpawnPoint), transform.up);
    }

    protected override void Fire()
    {
        currentCooldown = fireDelay;

        SpawnBullet();
    }

    public void UpdateRoutineIfNotShooting()
    {
        currentCooldown -= Time.deltaTime;
    }

    public override void UpdateRoutineWhenHeld()
    {
        if (Input.GetKey(fireKey) && canFire)
            Fire();
        else
            UpdateRoutineIfNotShooting();
    }
}
