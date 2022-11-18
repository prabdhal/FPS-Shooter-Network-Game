using Cinemachine;
using FishNet.Object;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    private string _name = "Default Weapon";
    [SerializeField]
    private float _damage = 5f;
    [SerializeField]
    private float _range = 15f;
    [SerializeField]
    private float _fireRate = 1f;
    [SerializeField]
    private float fireRateTimer = 0f;
    [SerializeField]
    private WeaponFireType fireType;

    public virtual void FireWeapon(PlayerController player)
    {
        Debug.Log("Firing with: " + _name + " with Fire type of: " + fireType);
        switch (fireType)
        {
            case WeaponFireType.SemiAutomatic:
                if (Input.GetKeyUp(KeyCode.F))
                {
                    Debug.Log("Semi automatic activate raycast!");
                    ActivateRaycastSemiAutomatic(player.vCam);
                }
                break;
            case WeaponFireType.Automatic:
                if (Input.GetKey(KeyCode.F))
                {
                    Debug.Log("Automatic activate raycast!");
                    ActivateRaycastAutomatic(player.vCam);
                }
                break;
            case WeaponFireType.Burst:
                if (Input.GetKey(KeyCode.F))
                {
                    Debug.Log("Burst activate raycast!");
                    ActivateRaycastAutomatic(player.vCam);
                }
                break;
            default:
                Debug.LogError("Weapon Firing Type not specified!");
                break;
        }
    }

    private void ActivateRaycastSemiAutomatic(CinemachineVirtualCamera cam)
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
                target.currentHealth -= _damage;
            }
        }
    }

    private void ActivateRaycastAutomatic(CinemachineVirtualCamera cam)
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

    private void TargetHitHandler(PlayerController target)
    {
        Debug.Log("fire rate timer: " + fireRateTimer);
        if (fireRateTimer <= 0)
        {
            Debug.Log(-_damage + " points of damage applied to " + target.name);
            target.currentHealth -= _damage;
            fireRateTimer = _fireRate;
        }
        else
        {
            fireRateTimer -= Time.deltaTime;
        }
    }
}
