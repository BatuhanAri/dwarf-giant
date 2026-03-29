using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

public class PrototypeSceneBuilder
{
    [MenuItem("Dwarf VS Giant/Build Prototype Scene")]
    public static void BuildScene()
    {
        // 1. Create a new empty scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // 2. Light & Camera Setup
        GameObject lightGo = new GameObject("Directional Light");
        Light light = lightGo.AddComponent<Light>();
        light.type = LightType.Directional;
        lightGo.transform.rotation = Quaternion.Euler(50, -30, 0);

        // 3. Create GameManager
        GameObject gmGo = new GameObject("GameManager");
        var gameManager = gmGo.AddComponent<GameManager>();

        // 4. Create Floor (Map)
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(5, 1, 5); // 50x50 meters
        
        // Set Floor as Navigation Static for NavMesh baking
        GameObjectUtility.SetStaticEditorFlags(floor, StaticEditorFlags.NavigationStatic);

        // 5. Create basic walls/obstacles
        CreateObstacle("Wall_1", new Vector3(0, 1.5f, 10), new Vector3(30, 3, 1));
        CreateObstacle("Wall_2", new Vector3(-10, 1.5f, 0), new Vector3(1, 3, 30));
        CreateObstacle("Hiding_Table", new Vector3(5, 0.75f, 5), new Vector3(4, 1.5f, 4)); // Dwarf can crouch under here, Giant cannot pass
        
        // We do NOT set Hiding Table to Navigation Static so Giant doesn't try to walk on it, or we make it giant blocker. Let's make all static
        
        // 6. Bake NavMesh
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

        // 7. Create Dwarf (Player)
        GameObject dwarf = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dwarf.name = "Player_Dwarf";
        dwarf.tag = "Player";
        dwarf.transform.position = new Vector3(0, 1, 0);
        dwarf.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // 1 meter tall total, standard capsule is 2 units tall
        dwarf.GetComponent<Renderer>().sharedMaterial.color = Color.green;
        var dwarfController = dwarf.AddComponent<DwarfController>();
        dwarfController.normalHeight = 2f; // Character controller height logic
        dwarfController.crouchHeight = 1f;
        
        // Player Camera
        GameObject dwarfCamera = new GameObject("PlayerCamera");
        var cam = dwarfCamera.AddComponent<Camera>();
        dwarfCamera.transform.SetParent(dwarf.transform);
        dwarfCamera.transform.localPosition = new Vector3(0, 0.8f, 0); // Eye level
        var mouseLook = dwarfCamera.AddComponent<MouseLook>();
        mouseLook.playerBody = dwarf.transform;
        
        // AudioListener
        dwarfCamera.AddComponent<AudioListener>();

        // 8. Create Giant
        GameObject giant = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        giant.name = "Giant";
        giant.transform.position = new Vector3(15, 1.5f, 15);
        giant.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // 3 meters tall
        giant.GetComponent<Renderer>().sharedMaterial.color = Color.red;

        var giantAI = giant.AddComponent<GiantAI>();
        giantAI.playerTarget = dwarf.transform;
        // The script has NavMeshAgent requireComponent
        var agent = giant.GetComponent<NavMeshAgent>();
        agent.height = 3f;
        agent.radius = 1f;

        // Create Patrol Points for Giant
        GameObject p1 = new GameObject("PatrolPoint 1"); p1.transform.position = new Vector3(15, 0, -15);
        GameObject p2 = new GameObject("PatrolPoint 2"); p2.transform.position = new Vector3(-15, 0, 15);
        GameObject p3 = new GameObject("PatrolPoint 3"); p3.transform.position = new Vector3(-15, 0, -15);
        giantAI.patrolPoints = new Transform[] { giant.transform, p1.transform, p2.transform, p3.transform };

        // Save scene logic
        Debug.Log("Dwarf vs Giant: Prototype Scene Built Successfully! The NavMesh has been baked. You can now press Play.");
    }

    private static void CreateObstacle(string name, Vector3 pos, Vector3 scale)
    {
        GameObject obs = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obs.name = name;
        obs.transform.position = pos;
        obs.transform.localScale = scale;
        GameObjectUtility.SetStaticEditorFlags(obs, StaticEditorFlags.NavigationStatic);
    }
}
