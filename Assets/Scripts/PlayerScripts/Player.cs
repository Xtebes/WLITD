using UnityEngine;
using System.Linq;

interface ImLoadedByPlayer
{
    void Load(Player player);
}
public class Player : MonoBehaviour
{
    public PlayerInputController input;
    public PlayerMovementController movement;
    public PlayerAnimationController animation;
    public PlayerFlashlightController flashlight;
    public StaminaBarUI stamina;
    [SerializeField]
    private GameObject[] loadedByPlayers;
    private void Start()
    {
        for (int i = 0; i < loadedByPlayers.Length; i++)
        {
            ImLoadedByPlayer[] loadedByPl = loadedByPlayers[i].GetComponentsInChildren<ImLoadedByPlayer>();
            for (int x = 0; x <  loadedByPl.Length; x++)
            {
                loadedByPl[x].Load(this);
            }
        }
    }
}
