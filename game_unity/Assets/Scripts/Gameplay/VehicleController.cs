using UnityEngine;

//Após ser instanciado serve para controlar o veículo
public class VehicleController : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float despawnX;

    public void Init(Vector2 dir, float spd, float limitX)
    {
        direction = dir.normalized;
        speed = spd;
        despawnX = limitX;
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > despawnX)
            Destroy(gameObject);
    }

}
