using System.Collections.Generic;
using UnityEngine;

namespace DallasSkinChanger
{
    public class LiveryMenu : MonoBehaviour
    {
        private escmenu _escMenu;
        private GameObject _overlayRoot;
        public void BuildMenu(escmenu menu)
        {
            _escMenu = menu;
            if (_overlayRoot != null )
                Destroy(_overlayRoot);
            // Switch the overlay parent to the Misc screen on the rearm menu
            var othersContainer = _escMenu.RearmUiManager.rearmmenu.Find("Rearming/MenuBranches/Others");

            // Move existing scroll menus out of the way
            var cosmeticTagButtonList = othersContainer.Find("CosmeticTagButtonList");
            cosmeticTagButtonList.transform.localPosition = new Vector3(-300, -330, 0);
            var cosmeticOptionButtonList = othersContainer.Find("CosmeticOptionButtonList");
            cosmeticOptionButtonList.transform.localPosition = new Vector3(120, -330, 0);

            _overlayRoot = Instantiate(cosmeticOptionButtonList.gameObject, othersContainer.transform);
            _overlayRoot.name = "LiveryMenu_Overlay";
            _overlayRoot.transform.localPosition = new Vector3(540, -330, 0);

            if (!_overlayRoot.TryGetComponent<RearmingMenuButtonLister>(out var rearmingMenuLister))
                return;

            rearmingMenuLister.purpose = RearmingMenuButtonLister.Purpose.NoPurpose;
            var testList = Plugin._liveryLoader.GetAllTextureNames();
            rearmingMenuLister.MakeButtons(testList, rearmingMenuLister.buttonSelected);

            foreach (var button in rearmingMenuLister.GeneratedButtons)
            {
                button.myButton.onClick.AddListener(() => Plugin._liveryLoader.ChangeSelectedLiveryTexture(button.myText.text));
            }
        }
    }
}