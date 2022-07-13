using UnityEngine;

public class LimitsSubmenuManager : MenuManager
{
    [SerializeField] private GameObject numbersSubmenu;
    [SerializeField] private GameObject textSubmenu;
    [SerializeField] private GameObject noLimitsText;
    [SerializeField] private GameObject[] optionalItems;

     public void SetupScene(string variable, string type)
    {
        // all menus are initially disabled to avoid an over-cluttered switch statement
        DisableAll();
        switch (type)
        {
            case "Integer":
                numbersSubmenu.SetActive(true);
                ToggleOptionalItems(false);
                break;
            case "Float":
                numbersSubmenu.SetActive(true);
                break;
            case "String":
                textSubmenu.SetActive(true);
                break;
            case "Character":
                textSubmenu.SetActive(true);
                ToggleOptionalItems(false);
                break;
            default:
                noLimitsText.SetActive(true);
                break;
        }
    }


    private void DisableAll()
    {
        // optionals are toggled to true so that they'll be already enabled when their menu is enabled
        ToggleOptionalItems(true);
        numbersSubmenu.SetActive(false);
        textSubmenu.SetActive(false);
        noLimitsText.SetActive(false);
    }

    private void ToggleOptionalItems(bool toggle)
    {
        Debug.Log(toggle);
        foreach (var item in optionalItems)
        {
            item.SetActive(toggle);
        }
    }
}