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
        Debug.Log("projectile hit something!: " + other.tag);
        if (other.CompareTag("Player"))
        {
            Debug.Log("projectile hit a Player!");
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (_player == null) Debug.LogError("Player Controller reference cannot be null on projectile!");
            if (_player != player && _player != null)
            {
                Debug.Log("Player: " + player.gameObject.name + " took " + _damage + " points of damage.");
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
