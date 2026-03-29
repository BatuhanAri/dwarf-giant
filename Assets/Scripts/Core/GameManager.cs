using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Singleton Instance
    public static GameManager Instance { get; private set; }

    public enum GameState { SelectionMenu, Playing, GameOver }
    
    [Header("Game State")]
    public GameState currentState = GameState.SelectionMenu;

    [Header("Character References")]
    public GameObject dwarfPlayer;
    public GameObject giantPlayer;

    void Awake()
    {
        // Singleton Implementation
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        ShowSelectionMenu();
    }

    public void ShowSelectionMenu()
    {
        currentState = GameState.SelectionMenu;
        
        // Lock player scripts
        SetPlayerActive(dwarfPlayer, false);
        SetPlayerActive(giantPlayer, false);

        // Unlock cursor for GUI clicking
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SelectCharacter(bool playAsDwarf)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentState = GameState.Playing;

        if (playAsDwarf)
        {
            SetPlayerActive(dwarfPlayer, true);
            SetPlayerActive(giantPlayer, false);
        }
        else
        {
            SetPlayerActive(dwarfPlayer, false); 
            SetPlayerActive(giantPlayer, true);  
        }
    }

    private void SetPlayerActive(GameObject playerObj, bool isActive)
    {
        if (playerObj == null) return;
        
        MonoBehaviour controller = playerObj.GetComponent<DwarfController>();
        if (controller == null) controller = playerObj.GetComponent<GiantController>();
        
        if (controller != null)
        {
            if (controller is DwarfController d) d.isLocalPlayer = isActive;
            if (controller is GiantController g) g.isLocalPlayer = isActive;
        }

        Camera[] cams = playerObj.GetComponentsInChildren<Camera>(true); 
        foreach(var cam in cams)
        {
            cam.enabled = isActive;
            AudioListener audio = cam.GetComponent<AudioListener>();
            if (audio != null) audio.enabled = isActive;
        }

        if (isActive && controller is DwarfController dc)
        {
            if (dc.firstPersonCam != null) dc.firstPersonCam.enabled = false;
            if (dc.thirdPersonCam != null) dc.thirdPersonCam.enabled = true;
        }
    }

    private void OnGUI()
    {
        if (currentState == GameState.SelectionMenu)
        {
            // Darken screen
            GUI.color = new Color(1, 1, 1, 0.9f);
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            // Setup styling
            GUI.color = Color.white;
            GUIStyle btnStyle = new GUIStyle(GUI.skin.button) { fontSize = 24 };
            GUIStyle txtStyle = new GUIStyle(GUI.skin.label) { fontSize = 32, alignment = TextAnchor.MiddleCenter };
            txtStyle.normal.textColor = Color.white;

            GUI.Label(new Rect(Screen.width / 2f - 250, Screen.height / 2f - 150, 500, 60), "WHO DO YOU WANT TO PLAY AS?", txtStyle);

            // Green tinted Dwarf button
            GUI.backgroundColor = Color.green;
            if (GUI.Button(new Rect(Screen.width / 2f - 250, Screen.height / 2f - 50, 200, 80), "CÜCE (DWARF)", btnStyle))
            {
                SelectCharacter(true);
            }

            // Red tinted Giant button
            GUI.backgroundColor = Color.red;
            if (GUI.Button(new Rect(Screen.width / 2f + 50, Screen.height / 2f - 50, 200, 80), "DEV (GIANT)", btnStyle))
            {
                SelectCharacter(false);
            }
        }
    }

    public void OnPlayerCaught()
    {
        if (currentState == GameState.GameOver) return;
        
        currentState = GameState.GameOver;
        Debug.Log("Game Over! The Giant caught the Dwarf.");
        StartCoroutine(RestartRoutine());
    }

    public void OnPlayerEscaped()
    {
        if (currentState == GameState.GameOver) return;
        
        currentState = GameState.GameOver;
        Debug.Log("Victory! The Dwarf escaped the house.");
        StartCoroutine(RestartRoutine());
    }

    private IEnumerator RestartRoutine()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
