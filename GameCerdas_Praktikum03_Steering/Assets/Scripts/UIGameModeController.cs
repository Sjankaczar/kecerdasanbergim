using UnityEngine;
using UnityEngine.UIElements;

public class UIGameModeController : MonoBehaviour
{
    private UIDocument uiDocument;
    
    private Button btnPengembangan;
    private VisualElement panelSubMenu;
    
    private Button btnColor;
    private Button btnAnimal;
    private Button btnSeparation;
    private Button btnPursue;

    private void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        // Mengambil referensi elemen UI berdasarkan nama
        btnPengembangan = root.Q<Button>("btn-pengembangan");
        panelSubMenu = root.Q<VisualElement>("panel-submenu");

        btnColor = root.Q<Button>("btn-color");
        btnAnimal = root.Q<Button>("btn-animal");
        btnSeparation = root.Q<Button>("btn-separation");
        btnPursue = root.Q<Button>("btn-pursue");

        // Menambahkan Event Listener saat tombol diklik
        if (btnPengembangan != null)
        {
            btnPengembangan.clicked += ToggleSubMenu;
        }

        // Menghubungkan tombol mode ke GameModeManager
        if (btnColor != null)
        {
            btnColor.clicked += () =>
            {
                GameModeManager.Instance.ToggleColor();
                btnColor.ToggleInClassList("btn-active");
            };
        }

        if (btnAnimal != null)
        {
            btnAnimal.clicked += () =>
            {
                GameModeManager.Instance.ToggleAnimal();
                btnAnimal.ToggleInClassList("btn-active");
                btnPursue?.RemoveFromClassList("btn-active");
            };
        }

        if (btnSeparation != null)
        {
            btnSeparation.clicked += () =>
            {
                GameModeManager.Instance.ToggleSeparation();
                btnSeparation.ToggleInClassList("btn-active");
            };
        }

        if (btnPursue != null)
        {
            btnPursue.clicked += () =>
            {
                GameModeManager.Instance.TogglePursue();
                btnPursue.ToggleInClassList("btn-active");
                btnAnimal?.RemoveFromClassList("btn-active");
            };
        }
    }

    private void ToggleSubMenu()
    {
        // Logika Expand / Collapse dengan mengubah style display
        if (panelSubMenu.style.display == DisplayStyle.None)
        {
            panelSubMenu.style.display = DisplayStyle.Flex;
        }
        else
        {
            panelSubMenu.style.display = DisplayStyle.None;
        }
    }

    private void OnDisable()
    {
        // Membersihkan event untuk mencegah error saat objek hancur
        if (btnPengembangan != null) 
            btnPengembangan.clicked -= ToggleSubMenu;
    }
}
