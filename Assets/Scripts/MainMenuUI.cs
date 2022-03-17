using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class MainMenuUI : MonoBehaviour
{
    public GameObject activeMenu;
    public CanvasGroup mainButtonsCG;
    public Button backButton;
    public CanvasGroup menusParentCG;
    void Start()
    {
        menusParentCG.alpha = 0;
        menusParentCG.interactable = false;
        menusParentCG.gameObject.SetActive(false);
        Button[] MenuButtons = mainButtonsCG.GetComponentsInChildren<Button>();
        GameObject[] menusParentChildren = menusParentCG.transform.GetFirstLevelChildren();
        MenuButtons[MenuButtons.Length - 1].onClick.AddListener(Application.Quit);
        backButton.onClick.AddListener(delegate 
        { 
            Tween tween1 = menusParentCG.DOFade(0, 0.2f);
            tween1.onComplete += delegate 
            {
                activeMenu.SetActive(false);
                menusParentCG.gameObject.SetActive(false);
                mainButtonsCG.gameObject.SetActive(true);
                Tween tween2 = mainButtonsCG.DOFade(1, 0.2f); 
                tween2.onComplete += delegate 
                { 
                    mainButtonsCG.interactable = true; 
                }; 
            }; 
        });
        foreach (Button button in MenuButtons)
        {
            button.onClick.AddListener(delegate
            {
                mainButtonsCG.interactable = false;
                Tween tween = mainButtonsCG.DOFade(0, 0.2f);
                menusParentChildren[menusParentChildren.Length - 1].SetActive(true);
                tween.onComplete += delegate
                {
                    menusParentCG.gameObject.SetActive(true);
                    Tween tween2 = menusParentCG.DOFade(1, 0.2f);
                    tween2.onComplete += delegate
                    {
                        menusParentCG.interactable = true;
                    };
                };
            });
        }
        backButton.AddScalingToButton();
        for (int i = 0; i < MenuButtons.Length; i++)
        {
            int index = i;
            MenuButtons[index].AddScalingToButton();
            MenuButtons[index].onClick.AddListener(delegate 
            { 
                activeMenu = menusParentChildren[index].gameObject;
                activeMenu.SetActive(true); 
            });
        }
    }
}