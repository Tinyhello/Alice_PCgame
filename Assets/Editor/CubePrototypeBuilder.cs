using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

//把GameScene複製成正方體原型場景：原本的迷宮當第一面（朝上），旁邊加一塊空地板當第二面
//選單 Alice > Build Cube Prototype Scene，可以重複執行，每次都會從GameScene重新產生
public static class CubePrototypeBuilder
{
    private const string SourceScene = "Assets/Scenes/GameScene.unity";
    private const string TargetScene = "Assets/Scenes/CubePrototype.unity";
    private const string DataFolder = "Assets/Scenes/CubePrototype";
    private const string MazeRootName = "Map_02";

    //====正方體的尺寸與位置，數值是從GameScene的NavMesh量出來的
    private const float CubeSize = 56f;
    private const float CubeCenterX = -8f;
    private const float FloorY = 0.33f;//迷宮地板的高度，也就是正方體頂面
    private const float EdgeZ = 36.35f;//入口平台的盡頭，第一面與第二面在這裡相接
    private const float CrossX = -1.3f;//入口走道的中線，越界點放在這裡

    private const float TriggerInset = 0.95f;//trigger中心離邊界多遠
    private const float ArrivalInset = 3.4f;//抵達點離邊界多遠，要在trigger外面
    private static readonly Vector3 TriggerSize = new Vector3(3f, 2.5f, 1f);

    [MenuItem("Alice/Build Cube Prototype Scene")]
    private static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        AssetDatabase.DeleteAsset(TargetScene);
        if (!AssetDatabase.CopyAsset(SourceScene, TargetScene))
        {
            Debug.LogError("複製場景失敗：" + SourceScene);
            return;
        }
        Scene scene = EditorSceneManager.OpenScene(TargetScene);

        GameObject maze = FindRoot(scene, MazeRootName);
        if (maze == null)
        {
            Debug.LogError("場景裡找不到根物件：" + MazeRootName);
            return;
        }

        //正方體的軸心在中心，旋轉時頂面才會維持在同一個高度
        GameObject cube = new GameObject("Cube");
        cube.transform.position = new Vector3(CubeCenterX, FloorY - CubeSize / 2f, EdgeZ - CubeSize / 2f);
        CubeWorld world = cube.AddComponent<CubeWorld>();

        maze.transform.SetParent(cube.transform, true);
        CubeFace face1 = maze.AddComponent<CubeFace>();
        world.startFace = face1;

        GameObject face2 = BuildEmptyFace("Face_02", FloorLayer());
        face2.transform.SetParent(cube.transform, true);
        //轉成Y軸朝世界的+Z，貼在正方體的+Z側，local的-Z邊就是跟第一面相接的邊
        face2.transform.SetPositionAndRotation(new Vector3(CubeCenterX, FloorY - CubeSize / 2f, EdgeZ), Quaternion.Euler(90f, 0f, 0f));

        CubeEdge edge1 = CreateEdge(maze.transform, "Edge_To_Face_02");
        edge1.transform.position = new Vector3(CrossX, FloorY + TriggerSize.y / 2f, EdgeZ - TriggerInset);
        edge1.arrivalPoint.SetPositionAndRotation(new Vector3(CrossX, FloorY, EdgeZ - ArrivalInset), Quaternion.Euler(0f, 180f, 0f));

        float localX = CrossX - CubeCenterX;
        float localEdgeZ = -CubeSize / 2f;
        CubeEdge edge2 = CreateEdge(face2.transform, "Edge_To_Face_01");
        edge2.transform.localPosition = new Vector3(localX, TriggerSize.y / 2f, localEdgeZ + TriggerInset);
        edge2.transform.localRotation = Quaternion.identity;
        edge2.arrivalPoint.position = face2.transform.TransformPoint(new Vector3(localX, 0f, localEdgeZ + ArrivalInset));
        edge2.arrivalPoint.rotation = face2.transform.rotation;

        edge1.target = edge2;
        edge2.target = edge1;

        ClearBatchingStatic(cube);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = cube;
        Debug.Log("正方體原型場景已建立：" + TargetScene);
    }

    //在原點朝上的狀態蓋好並烘好NavMesh，之後再搬到正方體的側面
    private static GameObject BuildEmptyFace(string name, int floorLayer)
    {
        GameObject face = new GameObject(name);
        face.AddComponent<CubeFace>();

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.layer = floorLayer;
        floor.transform.SetParent(face.transform, false);
        floor.transform.localScale = new Vector3(CubeSize, 0.5f, CubeSize);
        floor.transform.localPosition = new Vector3(0f, -0.25f, 0f);//頂面在y=0

        //放幾個方塊當地標，才看得出來換了一面
        CreateBlock(face.transform, new Vector3(0f, 1f, 0f), new Vector3(4f, 2f, 4f));
        CreateBlock(face.transform, new Vector3(-12f, 1.5f, 8f), new Vector3(3f, 3f, 6f));
        CreateBlock(face.transform, new Vector3(14f, 1f, -8f), new Vector3(6f, 2f, 3f));

        NavMeshSurface surface = face.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        Physics.SyncTransforms();
        surface.BuildNavMesh();

        if (!AssetDatabase.IsValidFolder(DataFolder))
        {
            AssetDatabase.CreateFolder("Assets/Scenes", "CubePrototype");
        }
        string dataPath = DataFolder + "/NavMesh-" + name + ".asset";
        AssetDatabase.DeleteAsset(dataPath);
        AssetDatabase.CreateAsset(surface.navMeshData, dataPath);

        return face;
    }

    private static void CreateBlock(Transform parent, Vector3 localPosition, Vector3 size)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = "Block";
        block.transform.SetParent(parent, false);
        block.transform.localPosition = localPosition;
        block.transform.localScale = size;
    }

    private static CubeEdge CreateEdge(Transform face, string name)
    {
        GameObject edgeObject = new GameObject(name);
        edgeObject.layer = LayerMask.NameToLayer("Ignore Raycast");//不要擋到滑鼠點擊
        edgeObject.transform.SetParent(face, false);

        BoxCollider trigger = edgeObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = TriggerSize;

        GameObject arrival = new GameObject("Arrival");
        arrival.transform.SetParent(edgeObject.transform, false);

        CubeEdge edge = edgeObject.AddComponent<CubeEdge>();
        edge.arrivalPoint = arrival.transform;
        return edge;
    }

    //地板要放在愛麗絲點得到的圖層
    private static int FloorLayer()
    {
        AliceController[] alices = Object.FindObjectsByType<AliceController>(FindObjectsInactive.Include);
        int mask = alices.Length > 0 ? alices[0].moveMask.value : 0;
        for (int layer = 0; layer < 32; layer++)
        {
            if ((mask & (1 << layer)) != 0)
                return layer;
        }
        return 0;
    }

    //勾了Batching Static的物件進Play後不會跟著transform動，會轉的都要取消
    private static void ClearBatchingStatic(GameObject root)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            StaticEditorFlags flags = GameObjectUtility.GetStaticEditorFlags(child.gameObject);
            if ((flags & StaticEditorFlags.BatchingStatic) != 0)
            {
                GameObjectUtility.SetStaticEditorFlags(child.gameObject, flags & ~StaticEditorFlags.BatchingStatic);
            }
        }
    }

    private static GameObject FindRoot(Scene scene, string name)
    {
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root.name == name)
                return root;
        }
        return null;
    }
}
