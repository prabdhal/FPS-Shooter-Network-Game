using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public string _name = "Default Weapon";
    public float _damage = 5f;
    public float _range = 15f;
    public int _ammo = 5;
    public float _fireRate = 1f;
    public float fireRateTimer = 0f;
    public WeaponFireType fireType;

    public virtual void FireWeapon(PlayerController player)
    {
        switch (fireType)
        {
            case WeaponFireType.SemiAutomatic:
                if (Input.GetKeyUp(KeyCode.F))
                {
                    Debug.Log(WeaponFireType.SemiAutomatic.ToString() + " activate raycast!");
                    ActivateRaycast(player.vCam);
                }
                break;
            case WeaponFireType.Automatic:
                if (Input.GetKey(KeyCode.F))
                {
                    Debug.Log(WeaponFireType.Automatic.ToString() + " activate raycast!");
                    ActivateRaycast(player.vCam);
                }
                break;
            case WeaponFireType.Burst:
                if (Input.GetKey(KeyCode.F))
                {
                    Debug.Log(WeaponFireType.Burst.ToString() + " activate raycast!");
                    ActivateRaycast(player.vCam);
                }
                break;
            default:
                Debug.LogError("Weapon Firing Type not specified!");
                break;
        }
    }

    private void ActivateRaycast(CinemachineVirtualCamera cam)
    {
        Vector3 origin = cam.transform.position;
        Vector3 direction = cam.transform.forward;
        RaycastHit hit;
        Debug.Log("Firing raycast!");
        if (Physics.Raycast(origin, direction, out hit, _range))
        {
            Debug.Log("Hit Player");
            Debug.DrawRay(origin, direction * _range, Color.red);
            if (hit.collider.tag.Equals("Player"))
            {
                Debug.Log("Hit: " + hit.collider.name);
                PlayerController target = hit.collider.gameObject.GetComponentInParent<PlayerController>();
                TargetHitHandler(target);
            }
        }
    }

    //private void ActivateRaycastSemiAutomatic(CinemachineVirtualCamera cam)
    //{
    //    Vector3 origin = cam.transform.position;
    //    Vector3 direction = cam.transform.forward;
    //    RaycastHit hit;
    //    Debug.Log("Firing raycast!");
    //    if (Physics.Raycast(origin, direction, out hit, _range))
    //    {
    //        Debug.Log("Hit Player");
    //        Debug.DrawRay(origin, direction * _range, Color.red);
    //        if (hit.collider.tag.Equals("Player"))
    //        {
    //            Debug.Log("Hit: " + hit.collider.name);
    //            PlayerController target = hit.collider.gameObject.GetComponentInParent<PlayerController>();
    //            TargetHitHandlerSemi(target);
    //        }
    //    }
    //}

    //private void ActivateRaycastAutomatic(CinemachineVirtualCamera cam)
    //{
    //    Vector3 origin = cam.transform.position;
    //    Vector3 direction = cam.transform.forward;
    //    RaycastHit hit;
    //    Debug.Log("Firing raycast!");
    //    if (Physics.Raycast(origin, direction, out hit, _range))
    //    {
    //        Debug.Log("Hit Player");
    //        Debug.DrawRay(origin, direction * _range, Color.red);
    //        if (hit.collider.tag.Equals("Player"))
    //        {
    //            Debug.Log("Hit: " + hit.collider.name);
    //            PlayerController target = hit.collider.gameObject.GetComponentInParent<PlayerController>();
    //            TargetHitHandler(target);
    //        }
    //    }
    //}

    private void TargetHitHandler(PlayerController target)
    {
        Debug.Log("fire rate timer: " + fireRateTimer);
        if (fireType.Equals(WeaponFireType.Automatic))
        {
            if (fireRateTimer <= 0)
            {
                ApplyDamage(target);
                fireRateTimer = _fireRate;
            }
            else
            {
                fireRateTimer -= Time.deltaTime;
            }
        }
        else
        {
            ApplyDamage(target);
        }
    }

    [ServerRpc]
    private void ApplyDamage(PlayerController target)
    {
        Debug.Log(-_damage + " points of damage applied to " + target.name);
        target.currentHealth -= _damage;
    }

}
