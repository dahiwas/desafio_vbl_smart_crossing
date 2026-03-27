using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 7f;
    [SerializeField] private Vector2 spawnPosition;

    private float weatherMultiplier = 1f;
    private bool inputEnabled = true;

    public void SetWeatherMultiplier(float value) => weatherMultiplier = Mathf.Max(0.1f, value);
    public void EnableInput(bool enable) => inputEnabled = enable;
    public void ResetToSpawn() => transform.position = (Vector3)spawnPosition;

    private void Update()
    {
        if (!inputEnabled) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float x = 0f;
        float y = 0f;

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)    y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)  y -= 1f;

        Vector2 move = new Vector2(x, y).normalized;
        transform.position += (Vector3)(move * (baseSpeed * weatherMultiplier) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Vehicle")) return;

        transform.position = (Vector3)spawnPosition;
    }
}
