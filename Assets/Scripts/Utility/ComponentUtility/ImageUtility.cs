using UnityEngine;
using UnityEngine.UI;

public class ImageUtility : ComponentUtility<Image>
{
    Sprite defaultSprite;
    Color defaultColor;

    internal override void Start()
    {
        base.Start();

        if (component.sprite != null) defaultSprite = component.sprite;

        defaultColor = component.color;
    }

    public void SetEnability(bool setTo)
    {
        if (isInitialized == false) return;

        component.enabled = setTo;
    }
    public void SetSprite(Sprite setTo)
    {
        if (isInitialized == false) return;

        component.sprite = setTo;
    }
    public void SetColor(Color setTo)
    {
        if (isInitialized == false) return;

        component.color = setTo;
    }

    public void SetWeaponSprite(PlayerWeapon weapon)
    {
        if (isInitialized == false) return;

        if(defaultSprite != null)
        {
            if (weapon == null)
            {
                component.sprite = defaultSprite;
                return;
            }
            else if (weapon.WeaponIcon == null)
            {
                component.sprite = defaultSprite;
                return;
            }
        }

        component.sprite = weapon.WeaponIcon;
    }
}
