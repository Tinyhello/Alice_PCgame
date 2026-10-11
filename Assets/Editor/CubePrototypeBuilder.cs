using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

//把GameScene複製成正方體原型場景：原本的迷宮當第一面（朝上），其他五面是放了地標方塊的空地板
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
    private const float EdgeZ = 36.35f;//入口平台的盡頭，迷宮只有這裡走得到正方體的邊
    private const float CrossX = -1.3f;//入口走道的中線

    private const float TriggerInset = 0.95f;//trigger中心離邊界多遠
    private const float ArrivalInset = 3.4f;//抵達點離邊界多遠，要在trigger外面
    private const float TriggerHeight = 2.5f;
    private const float TriggerDepth = 1f;
    private const float MazeEdgeWidth = 3f;//迷宮入口走道的寬度
    private const float CornerMargin = 5f;//空地板的邊界trigger兩端各留多少，避開正方體的角

    //迷宮以外的五面：朝外的方向與辨識用的顏色
    private static readonly Vector3[] FaceNormals = { Vector3.forward, Vector3.right, Vector3.back, Vector3.left, Vector3.down };
    private static readonly Color[] FaceColors =
    {
        new Color(0.85f, 0.45f, 0.40f),
        new Color(0.45f, 0.70f, 0.45f),
        new Color(0.40f, 0.55f, 0.85f),
        new Color(0.85f, 0.75f, 0.40f),
        new Color(0.65f, 0.45f, 0.80f),
    };

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

        AssetDatabase.DeleteAsset(DataFolder);
        AssetDatabase.CreateFolder("Assets/Scenes", "CubePrototype");

        //正方體的軸心在中心，旋轉時頂面才會維持在同一個高度
        Vector3 center = new Vector3(CubeCenterX, FloorY - CubeSize / 2f, EdgeZ - CubeSize / 2f);
        GameObject cube = new GameObject("Cube");
        cube.transform.position = center;
        CubeWorld world = cube.AddComponent<CubeWorld>();

        maze.transform.SetParent(cube.transform, true);
        world.startFace = maze.AddComponent<CubeFace>();

        int floorLayer = FloorLayer();
        Transform[] faces = new Transform[FaceNormals.Length];
        for (int i = 0; i < FaceNormals.Length; i++)
        {
            GameObject face = BuildEmptyFace("Face_0" + (i + 2), floorLayer, FaceColors[i], i);
            face.transform.SetParent(cube.transform, true);
            face.transform.SetPositionAndRotation(center + FaceNormals[i] * (CubeSize / 2f), FaceRotation(FaceNormals[i]));
            faces[i] = face.transform;
        }

        //迷宮只接到+Z那一面，越界點在入口走道上
        ConnectFaces(center, maze.transform, Vector3.up, faces[0], FaceNormals[0], CrossX - CubeCenterX, MazeEdgeWidth);

        //其他五面只要相鄰就整條邊都可以越界
        for (int a = 0; a < faces.Length; a++)
        {
            for (int b = a + 1; b < faces.Length; b++)
            {
                if (Vector3.Dot(FaceNormals[a], FaceNormals[b]) == 0f)
                {
                    ConnectFaces(center, faces[a], FaceNormals[a], faces[b], FaceNormals[b], 0f, CubeSize - CornerMargin * 2f);
                }
            }
        }

        ClearBatchingStatic(cube);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = cube;
        Debug.Log("正方體原型場景已建立：" + TargetScene);
    }

    //面自己的Y軸朝外
    private static Quaternion FaceRotation(Vector3 normal)
    {
        return normal == Vector3.down ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.FromToRotation(Vector3.up, normal);
    }

    //在原點朝上的狀態蓋好並烘好NavMesh，之後再搬到正方體上
    private static GameObject BuildEmptyFace(string name, int floorLayer, Color color, int variant)
    {
        GameObject face = new GameObject(name);
        face.AddComponent<CubeFace>();

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.layer = floorLayer;
        floor.transform.SetParent(face.transform, false);
        floor.transform.localScale = new Vector3(CubeSize, 0.5f, CubeSize);
        floor.transform.localPosition = new Vector3(0f, -0.25f, 0f);//頂面在y=0

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        Material floorMaterial = CreateMaterial(floorRenderer.sharedMaterial, Color.Lerp(color, Color.white, 0.5f), name + "_Floor");
        Material blockMaterial = CreateMaterial(floorRenderer.sharedMaterial, color, name + "_Block");
        floorRenderer.sharedMaterial = floorMaterial;

        //每一面的地標方塊排法不一樣，才看得出來換到哪一面
        float angle = variant * 37f;
        for (int i = 0; i < variant + 2; i++)
        {
            Vector3 position = Quaternion.Euler(0f, angle + i * 360f / (variant + 2), 0f) * new Vector3(0f, 0f, 13f);
            float height = 1.5f + i;
            CreateBlock(face.transform, position + Vector3.up * (height / 2f), new Vector3(4f, height, 4f), blockMaterial);
        }

        NavMeshSurface surface = face.AddComponent<NavMeshSurface>();
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = UnityEngine.AI.NavMeshCollectGeometry.PhysicsColliders;
        Physics.SyncTransforms();
        surface.BuildNavMesh();
        AssetDatabase.CreateAsset(surface.navMeshData, DataFolder + "/NavMesh-" + name + ".asset");

        return face;
    }

    private static Material CreateMaterial(Material source, Color color, string name)
    {
        Material material = new Material(source);
        material.color = color;
        AssetDatabase.CreateAsset(material, DataFolder + "/" + name + ".mat");
        return material;
    }

    private static void CreateBlock(Transform parent, Vector3 localPosition, Vector3 size, Material material)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = "Block";
        block.transform.SetParent(parent, false);
        block.transform.localPosition = localPosition;
        block.transform.localScale = size;
        block.GetComponent<Renderer>().sharedMaterial = material;
    }

    //在兩個相鄰的面各放一個邊界trigger並互相指向對方
    //offset是越界點沿著邊偏離中點多遠，width是可以越界的寬度
    private static void ConnectFaces(Vector3 center, Transform faceA, Vector3 normalA, Transform faceB, Vector3 normalB, float offset, float width)
    {
        Vector3 along = Vector3.Cross(normalA, normalB);
        Vector3 edgePoint = center + (normalA + normalB) * (CubeSize / 2f) + along * offset;

        CubeEdge edgeA = CreateEdge(faceA, "Edge_To_" + faceB.name, edgePoint, normalA, normalB, width);
        CubeEdge edgeB = CreateEdge(faceB, "Edge_To_" + faceA.name, edgePoint, normalB, normalA, width);
        edgeA.target = edgeB;
        edgeB.target = edgeA;
    }

    //up是所在的面朝外的方向，outward是從這一面往邊界外走的方向
    private static CubeEdge CreateEdge(Transform face, string name, Vector3 edgePoint, Vector3 up, Vector3 outward, float width)
    {
        GameObject edgeObject = new GameObject(name);
        edgeObject.layer = LayerMask.NameToLayer("Ignore Raycast");//不要擋到滑鼠點擊
        edgeObject.transform.SetParent(face, false);
        edgeObject.transform.SetPositionAndRotation(edgePoint - outward * TriggerInset + up * (TriggerHeight / 2f), Quaternion.LookRotation(outward, up));

        BoxCollider trigger = edgeObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = new Vector3(width, TriggerHeight, TriggerDepth);

        GameObject arrival = new GameObject("Arrival");
        arrival.transform.SetParent(edgeObject.transform, false);
        arrival.transform.SetPositionAndRotation(edgePoint - outward * ArrivalInset, Quaternion.LookRotation(-outward, up));//面向面的內側

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
