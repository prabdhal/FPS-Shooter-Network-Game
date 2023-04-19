using FishNet.Object;
using UnityEngine;

public class AnimEventManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private AudioSource fireAudioSource;
    [SerializeField]
    private AudioSource reloadAudioSource;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
        }
        else
        {
            gameObject.GetComponent<AnimEventManager>().enabled = false;
        }
    }

    private void Start()
    {
        controller.OnWeaponSwapEvent += SetWeaponAudioKit;
        
        // Initialize weapon audio source
        SetWeaponAudioKit(controller.activeWeapon);
    }

    private void SetWeaponAudioKit(Weapon weapon)
    {
        if (weapon == null)
        {
            fireAudioSource.clip = controller.defaultWeapon.fireSound;
            reloadAudioSource.clip = controller.defaultWeapon.reloadSound;
        }

        fireAudioSource.clip = weapon.fireSound;
        reloadAudioSource.clip = weapon.reloadSound;
    }

    public void PlayWeaponFireSound()
    {
        fireAudioSource.Play();
    }

    public void PlayWeaponReloadSound()
    {
        reloadAudioSource.Play();
    }
}
