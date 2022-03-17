using UnityEngine;
using System;
using System.Collections;

public class PlayerMovementController : MonoBehaviour, ImLoadedByPlayer
{
    private PlayerInputController input;
    private Rigidbody2D playerRigidbody;
    public float currentSpeed, walkSpeed, runSpeed, currentStamina, staminaThreshold, maxStamina, staminaConsumption;
    private Coroutine staminaBehaviour;
    void ImLoadedByPlayer.Load(Player player)
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        input = player.input;
        currentSpeed = walkSpeed;
        currentStamina = maxStamina;
        player.input.runAction.started += ctx => StartRunning();
        player.input.runAction.performed += ctx => StopRunning();
        player.input.runAction.canceled += ctx => StopRunning();
        Debug.Log("loaded");
    }
    private void StartRunning()
    {
        Debug.Log("started running");
        if (currentStamina > staminaThreshold)
        {
            currentSpeed = runSpeed;
            if (staminaBehaviour != null)
                StopCoroutine(staminaBehaviour);
            staminaBehaviour = StartCoroutine(ConsumeStamina());
        }
    }
    private void StopRunning()
    {
        currentSpeed = walkSpeed;
        if (staminaBehaviour != null)
            StopCoroutine(staminaBehaviour);
        staminaBehaviour = StartCoroutine(RecoverStamina());
    }

    private IEnumerator ConsumeStamina()
    {
        if (currentStamina > 0)
        {
            while (currentStamina > 0)
            {
                currentStamina -= staminaConsumption * Time.deltaTime;
                yield return null;
            }
            currentStamina = 0;
            currentSpeed = walkSpeed;
        }
    }
    private IEnumerator RecoverStamina()
    {
        while (currentStamina < maxStamina)
        {
            currentStamina += staminaConsumption / 3 * Time.deltaTime;
            yield return null;
        }
        currentStamina = maxStamina;
    }
    private void Update()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);;
    }
    private void FixedUpdate()
    {
        playerRigidbody.MovePosition(playerRigidbody.position + (input.directionalInput * currentSpeed * Time.fixedDeltaTime));
    }
}