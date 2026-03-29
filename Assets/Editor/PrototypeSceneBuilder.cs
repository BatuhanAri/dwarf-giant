#pragma warning disable 0618
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class PrototypeSceneBuilder
{
    [MenuItem("Dwarf VS Giant/Build Prototype Scene")]
    public static void BuildScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        GameObject lightGo = new GameObject("Directional Light");
        Light light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        lightGo.transform.rotation = Quaternion.Euler(50, -30, 0);

        // GameManager
        GameObject gmGo = new GameObject("GameManager");
        var gameManager = gmGo.AddComponent<GameManager>();

        // Map
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(10, 1, 10); 
        GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.NavigationStatic);

        CreateObstacle("Wall_1", new Vector3(0, 1.5f, 10), new Vector3(30, 3, 1));
        CreateObstacle("Wall_2", new Vector3(-10, 1.5f, 0), new Vector3(1, 3, 30));
        CreateObstacle("Hiding_Table", new Vector3(5, 0.75f, 5), new Vector3(4, 1.5f, 4));

        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

        // --- DWARF ---
        GameObject dwarf = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dwarf.name = "Player_Dwarf";
        dwarf.transform.position = new Vector3(0, 1, 0);
        dwarf.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        GameObject dwarfHead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dwarfHead.name = "Head";
        dwarfHead.transform.SetParent(dwarf.transform);
        dwarfHead.transform.localPosition = new Vector3(0, 1.2f, 0);
        dwarfHead.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        dwarfHead.GetComponent<Renderer>().sharedMaterial.color = Color.green;

        var dwarfController = dwarf.AddComponent<DwarfController>();
        dwarfController.normalHeight = 2f; 
        dwarfController.crouchHeight = 1f;
        dwarfController.headRenderer = dwarfHead.GetComponent<Renderer>();
        dwarfController.isLocalPlayer = false; // Set via GameManager now
        
        GameObject dwarfHold = new GameObject("HoldPosition");
        dwarfHold.transform.SetParent(dwarf.transform);
        dwarfHold.transform.localPosition = new Vector3(0, 0, 1.5f);
        dwarfController.holdPosition = dwarfHold.transform;

        GameObject dwarfFPCamera = new GameObject("FirstPersonCamera");
        var fpCam = dwarfFPCamera.AddComponent<Camera>();
        dwarfFPCamera.transform.SetParent(dwarf.transform);
        dwarfFPCamera.transform.localPosition = new Vector3(0, 0.8f, 0); 
        var mouseLookFP = dwarfFPCamera.AddComponent<MouseLook>();
        mouseLookFP.playerBody = dwarf.transform;
        var fpAudio = dwarfFPCamera.AddComponent<AudioListener>();

        GameObject dwarfTPCamera = new GameObject("ThirdPersonCamera");
        var tpCam = dwarfTPCamera.AddComponent<Camera>();
        dwarfTPCamera.transform.SetParent(dwarf.transform);
        dwarfTPCamera.transform.localPosition = new Vector3(0, 3f, -4f); 
        dwarfTPCamera.transform.localRotation = Quaternion.Euler(20, 0, 0);
        var tpAudio = dwarfTPCamera.AddComponent<AudioListener>();
        
        dwarfController.firstPersonCam = fpCam;
        dwarfController.thirdPersonCam = tpCam;

        // --- GIANT ---
        GameObject giant = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        giant.name = "Player_Giant";
        giant.transform.position = new Vector3(15, 5f, 15);
        giant.transform.localScale = new Vector3(5f, 5f, 5f);
        giant.GetComponent<Renderer>().sharedMaterial.color = Color.red;

        var giantController = giant.AddComponent<GiantController>();
        giantController.isLocalPlayer = false;

        GameObject giantHold = new GameObject("HoldPosition");
        giantHold.transform.SetParent(giant.transform);
        giantHold.transform.localPosition = new Vector3(0, 0, 1.5f);
        giantController.holdPosition = giantHold.transform;

        GameObject giantFPCamera = new GameObject("GiantCamera");
        var gCam = giantFPCamera.AddComponent<Camera>();
        giantFPCamera.transform.SetParent(giant.transform);
        giantFPCamera.transform.localPosition = new Vector3(0, 0.8f, 0); 
        var mouseLookGiant = giantFPCamera.AddComponent<MouseLook>();
        mouseLookGiant.playerBody = giant.transform;
        giantFPCamera.AddComponent<AudioListener>();
        
        dwarfController.giantTransform = giant.transform;

        // Connect everything to GameManager
        gameManager.dwarfPlayer = dwarf;
        gameManager.giantPlayer = giant;

        // --- INTERACTABLES ---
        CreateInteractable("Carry_Box_1", new Vector3(2, 0.5f, -2));
        CreateInteractable("Carry_Box_2", new Vector3(4, 0.5f, -2));

        Debug.Log("Dwarf vs Giant Phase 3: Setup Complete. Press PLAY to see the Character Selection Menu!");
    }

    private static void CreateObstacle(string name, Vector3 pos, Vector3 scale)
    {
        GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs.name = name;
        obs.transform.position = pos;
        obs.transform.localScale = scale;
        GameObjectUtility.SetStaticEditorFlags(obs, StaticEditorFlags.NavigationStatic);
    }
    
    private static void CreateInteractable(string name, Vector3 pos)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.position = pos;
        item.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        item.GetComponent<Renderer>().sharedMaterial.color = Color.blue;
        item.AddComponent<InteractableItem>();
    }
}
