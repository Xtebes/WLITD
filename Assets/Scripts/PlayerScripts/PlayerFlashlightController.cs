using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class PlayerFlashlightController : MonoBehaviour, ImLoadedByPlayer
{
    private Canvas fogCameraCanvas;
    private Camera fogCamera;
    private Lantern _equippedLantern;
    private PlayerInputController input;
    private float swayTime;
    public Lantern equippedLantern
    {
        get
        {
            return _equippedLantern;
        }
        private set
        {
            _equippedLantern = value;
            SetLanternToWalkSettings();
        }
    }
    public bool isFlashLightOn { private set; get; }
    public Light2D light2D { private set; get;}
    [SerializeField]
    private float 
        walkingSwayMagnitude, runningSwayMagnitude, 
        walkingSwaySpeed, runningSwaySpeed, 
        baseWalkingRadius,
        baseFogCameraSize;
    public float rotationSpeed, rotationOffset;
    void ImLoadedByPlayer.Load(Player player)
    {
        Lantern.onEquip = lantern => equippedLantern = lantern;
        input = player.input;
        Lantern.onEquip.Invoke(SavedInfo.save.playerInfo.equippedLantern);
        player.input.toggleFlashlightAction.performed += delegate { ToggleFlashLight(!isFlashLightOn); };
        player.input.runAction.started += delegate { SetLanternToRunSettings(); };
        player.input.runAction.performed += delegate { SetLanternToWalkSettings(); };
    }
    private void Start()
    {
        light2D = GetComponentInChildren<Light2D>();
        fogCamera = GetComponentInChildren<Camera>();
        fogCameraCanvas = GetComponentInChildren<Canvas>();
    }
    //disgusting lantern movement
    private void Update()
    {
        swayTime = (swayTime + Time.deltaTime) * input.directionalInput.magnitude; 
        light2D.transform.localEulerAngles = new Vector3(0,0, Mathf.Sin(swayTime * (walkingSwaySpeed * input.directionalInput.magnitude + (runningSwaySpeed * input.toggle))) * (walkingSwayMagnitude + (runningSwayMagnitude * input.toggle)));
        float lanternRotation = ((Mathf.Atan2(input.directionalInput.y, input.directionalInput.x) * Mathf.Rad2Deg - rotationOffset) * input.toggle) + ((Mathf.Atan2(input.mousePositionInWorld.y - transform.position.y, input.mousePositionInWorld.x - transform.position.x) * Mathf.Rad2Deg + rotationOffset) * ((input.toggle - 1) * -1));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, lanternRotation), Time.deltaTime * rotationSpeed);
    }
    public void SetLanternToWalkSettings()
    {
        light2D.pointLightOuterRadius = _equippedLantern.walkingLengthMultiplier * baseWalkingRadius;
        fogCamera.orthographicSize = baseFogCameraSize * _equippedLantern.walkingLengthMultiplier;
        fogCameraCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(baseFogCameraSize  * 2 * _equippedLantern.walkingLengthMultiplier, baseFogCameraSize * 2 * _equippedLantern.walkingLengthMultiplier);
    }
    public void SetLanternToRunSettings()
    {
        light2D.pointLightOuterRadius = _equippedLantern.runningLengthMultiplier * baseWalkingRadius;
        fogCamera.orthographicSize = baseFogCameraSize * _equippedLantern.runningLengthMultiplier;
        fogCameraCanvas.GetComponent<RectTransform>().sizeDelta = new Vector2(baseFogCameraSize * 2 * _equippedLantern.runningLengthMultiplier, baseFogCameraSize * 2 * _equippedLantern.runningLengthMultiplier);
    }
    public void ToggleFlashLight(bool on)
    {
        isFlashLightOn = on;
        light2D.gameObject.SetActive(on);
    }
}
