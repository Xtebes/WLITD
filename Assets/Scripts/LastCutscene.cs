using System.Collections;
using UnityEngine.SceneManagement;

public class LastCutscene : Interactable
{
    public override void OnPlayerEnter(Player player)
    {
        StartCoroutine(playerEnter(player));
    }
    private IEnumerator playerEnter(Player player)
    {
        FindObjectOfType<MonsterAI>().gameObject.SetActive(false);
        player.input.inputActionAsset.Disable();
        yield return StartCoroutine(FadeInOut.FadeInAndOutCutscene(0.1f, Singleton<GameManager>.Instance.cutscenes[1], 0.1f));
        SceneManager.LoadScene("MainMenu");
    }
}
