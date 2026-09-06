using UnityEngine;
using UnityEngine.UIElements; // Wajib ditambahkan untuk UI Toolkit

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public EnemyDetector enemyDetector; // Tarik objek Enemy ke sini nanti

    private Label distanceLabel;
    private Button modeButton;

    void OnEnable()
    {
        // Mendapatkan referensi UI Document yang menempel di objek ini
        var uiDocument = GetComponent<UIDocument>();
        var root = uiDocument.rootVisualElement;

        // Mencari elemen berdasarkan 'Name' yang kita atur di UI Builder
        distanceLabel = root.Q<Label>("DistanceLabel");
        modeButton = root.Q<Button>("ModeButton");

        // Menambahkan fungsi saat tombol diklik
        if (modeButton != null)
        {
            modeButton.clicked += OnModeButtonClicked;
        }
    }

    void Update()
    {
        if (enemyDetector != null && distanceLabel != null)
        {
            // Memperbarui teks jarak setiap frame (dibulatkan 1 angka di belakang koma)
            distanceLabel.text = "Distance: " + enemyDetector.currentDistance.ToString("F1");
        }
    }

    void OnModeButtonClicked()
    {
        if (enemyDetector != null)
        {
            enemyDetector.ToggleMode(); 
            
            // Perbarui teks tombol menjadi lebih singkat
            if (enemyDetector.isAdvancedMode)
                modeButton.text = "3 States";
            else
                modeButton.text = "2 States";
        }
    }
}
