using UnityEngine;
using UnityEngine.UI;

public class ImageUtility : ComponentUtility<Image>
{
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
        if (weapon == null) return;
        if (weapon.WeaponIcon == null) return;

        component.sprite = weapon.WeaponIcon;
    }
}
