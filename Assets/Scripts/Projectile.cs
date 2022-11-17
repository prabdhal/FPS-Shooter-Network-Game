using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    public PlayerController _player;
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private float _damage = 5f;
    [SerializeField]
    private float _deathTimer = 3f;
    private float timer = 0;

    private void Update()
    {
        if (timer > _deathTimer)
            DespawnObject(gameObject);
        else
            timer += Time.deltaTime;

        transform.position += transform.forward * _speed * Time.deltaTime;
    }

    public void Init(PlayerController player, float speed, float damage)
    {
        _player = player;
        _speed = speed;
        _damage = damage;
        Debug.Log("Inited: " + _player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (_player != player)
            {
                player.UpdateHealth(-_damage);
                DespawnObject(gameObject);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnObject(GameObject obj)
    {
        ServerManager.Despawn(obj);
    }
}
