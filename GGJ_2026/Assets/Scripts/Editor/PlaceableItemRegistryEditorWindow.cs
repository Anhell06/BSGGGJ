#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Окно редактора реестра PlaceableItem: список всех предметов, добавление и редактирование.
/// Меню: Tools → Placement → Placeable Item Registry
/// </summary>
public class PlaceableItemRegistryEditorWindow : EditorWindow
{
    private PlaceableItemRegistry _registry;
    private SerializedObject _serializedRegistry;
    private SerializedProperty _itemsProperty;
    private Vector2 _listScroll;
    private Vector2 _inspectorScroll;
    private int _selectedIndex = -1;
    private Editor _itemEditor;
    private const float ListWidth = 280f;
    private const float PreviewSize = 52f;
    private const string RegistryResourcePath = "PlaceableItemRegistry";

    [MenuItem("Tools/Placement/Placeable Item Registry")]
    public static void ShowWindow()
    {
        var window = GetWindow<PlaceableItemRegistryEditorWindow>("Placeable Items");
        window.minSize = new Vector2(400f, 300f);
    }

    private void OnEnable()
    {
        LoadRegistryFromResources();
        RefreshSerialized();
    }

    private void OnDisable()
    {
        DestroyImmediate(_itemEditor);
    }

    private void RefreshSerialized()
    {
        if (_registry == null)
        {
            _serializedRegistry = null;
            _itemsProperty = null;
            return;
        }
        _serializedRegistry = new SerializedObject(_registry);
        _itemsProperty = _serializedRegistry.FindProperty("items");
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6f);
        DrawRegistryField();
        if (_registry == null)
            return;

        _serializedRegistry?.Update();

        EditorGUILayout.Space(4f);
        using (new EditorGUILayout.HorizontalScope())
        {
            DrawListPanel();
            DrawInspectorPanel();
        }

        _serializedRegistry?.ApplyModifiedProperties();
    }

    private void LoadRegistryFromResources()
    {
        _registry = Resources.Load<PlaceableItemRegistry>(RegistryResourcePath);
    }

    private void DrawRegistryField()
    {
        EditorGUI.BeginChangeCheck();
        _registry = (PlaceableItemRegistry)EditorGUILayout.ObjectField(
            "Реестр",
            _registry,
            typeof(PlaceableItemRegistry),
            false);
        if (EditorGUI.EndChangeCheck())
        {
            RefreshSerialized();
            _selectedIndex = -1;
            DestroyImmediate(_itemEditor);
        }

        if (GUILayout.Button("Загрузить из Resources", GUILayout.Height(20f)))
        {
            LoadRegistryFromResources();
            RefreshSerialized();
            _selectedIndex = -1;
            DestroyImmediate(_itemEditor);
        }

        if (_registry == null)
        {
            EditorGUILayout.HelpBox(
                "Реестр не найден в Resources. Поместите PlaceableItemRegistry.asset в папку Resources или создайте новый.",
                MessageType.Info);
            if (GUILayout.Button("Создать новый реестр в Resources"))
                CreateNewRegistry();
        }
    }

    private void CreateNewRegistry()
    {
        const string resourcesFolder = "Assets/Configs/Resources";
        if (!AssetDatabase.IsValidFolder("Assets/Configs"))
            AssetDatabase.CreateFolder("Assets", "Configs");
        if (!AssetDatabase.IsValidFolder("Assets/Configs/Resources"))
            AssetDatabase.CreateFolder("Assets/Configs", "Resources");
        var path = $"{resourcesFolder}/{RegistryResourcePath}.asset";
        var registry = CreateInstance<PlaceableItemRegistry>();
        AssetDatabase.CreateAsset(registry, path);
        AssetDatabase.SaveAssets();
        _registry = registry;
        RefreshSerialized();
        Selection.activeObject = registry;
    }

    private void DrawListPanel()
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(ListWidth)))
        {
            EditorGUILayout.LabelField("Предметы", EditorStyles.boldLabel);

            using (var scope = new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Добавить выбранное", GUILayout.Height(22f)))
                    AddFromSelection();
                if (GUILayout.Button("Создать новый", GUILayout.Height(22f)))
                    CreateNewItem();
            }

            int count = _itemsProperty?.arraySize ?? 0;
            if (count == 0)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.HelpBox("Список пуст. Добавьте предметы кнопками выше.", MessageType.None);
                return;
            }

            _listScroll = EditorGUILayout.BeginScrollView(_listScroll);
            for (int i = 0; i < count; i++)
            {
                DrawListItem(i);
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawListItem(int index)
    {
        var prop = _itemsProperty.GetArrayElementAtIndex(index);
        var item = (PlaceableItem)prop.objectReferenceValue;
        if (item == null)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"<null> #{index}");
            if (GUILayout.Button("×", GUILayout.Width(22f)))
            {
                _itemsProperty.DeleteArrayElementAtIndex(index);
                if (_selectedIndex == index) _selectedIndex = -1;
                else if (_selectedIndex > index) _selectedIndex--;
            }
            EditorGUILayout.EndHorizontal();
            return;
        }

        bool selected = _selectedIndex == index;
        var bg = selected ? new GUIStyle("SelectionRect") : EditorStyles.helpBox;
        EditorGUILayout.BeginHorizontal(bg);

        if (GUILayout.Button(GUIContent.none, GUILayout.Width(PreviewSize + 4f), GUILayout.Height(PreviewSize + 4f)))
        {
            _selectedIndex = index;
            RebuildItemEditor(item);
        }

        var rect = GUILayoutUtility.GetLastRect();
        var preview = AssetPreview.GetAssetPreview(item.gameObject);
        if (AssetPreview.IsLoadingAssetPreview(item.gameObject.GetInstanceID()))
            Repaint();
        if (preview != null)
            GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit);
        else
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f));

        using (new EditorGUILayout.VerticalScope())
        {
            EditorGUILayout.LabelField(string.IsNullOrEmpty(item.ItemName) ? item.gameObject.name : item.ItemName, EditorStyles.boldLabel);
            if (GUILayout.Button("Выбрать", GUILayout.Height(18f)))
            {
                Selection.activeGameObject = item.gameObject;
                EditorGUIUtility.PingObject(item.gameObject);
            }
            if (GUILayout.Button("Удалить", GUILayout.Height(18f)))
            {
                _itemsProperty.DeleteArrayElementAtIndex(index);
                if (_selectedIndex == index)
                {
                    _selectedIndex = -1;
                    DestroyImmediate(_itemEditor);
                }
                else if (_selectedIndex > index)
                    _selectedIndex--;
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawInspectorPanel()
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.ExpandWidth(true)))
        {
            if (_selectedIndex < 0 || _itemsProperty == null || _selectedIndex >= _itemsProperty.arraySize)
            {
                EditorGUILayout.LabelField("Редактор", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("Выберите предмет в списке слева.", MessageType.Info);
                return;
            }

            var prop = _itemsProperty.GetArrayElementAtIndex(_selectedIndex);
            var item = (PlaceableItem)prop.objectReferenceValue;
            if (item == null)
            {
                EditorGUILayout.HelpBox("Ссылка на предмет отсутствует.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Редактор: " + (string.IsNullOrEmpty(item.ItemName) ? item.name : item.ItemName), EditorStyles.boldLabel);
            _inspectorScroll = EditorGUILayout.BeginScrollView(_inspectorScroll);
            if (_itemEditor != null && _itemEditor.target == item)
            {
                _itemEditor.OnInspectorGUI();
            }
            else
            {
                RebuildItemEditor(item);
                _itemEditor?.OnInspectorGUI();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void RebuildItemEditor(PlaceableItem item)
    {
        DestroyImmediate(_itemEditor);
        _itemEditor = item != null ? Editor.CreateEditor(item) : null;
    }

    /// <summary>
    /// Реестр — ScriptableObject; в него сохраняются только ссылки на префабы (ассеты).
    /// Ссылки на объекты в сцене при сериализации становятся null.
    /// </summary>
    private static string GetPrefabsFolder()
    {
        const string folder = "Assets/Configs/Resources/Prefabs";
        if (!AssetDatabase.IsValidFolder("Assets/Configs"))
            AssetDatabase.CreateFolder("Assets", "Configs");
        if (!AssetDatabase.IsValidFolder("Assets/Configs/Resources"))
            AssetDatabase.CreateFolder("Assets/Configs", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Configs/Resources/Prefabs"))
            AssetDatabase.CreateFolder("Assets/Configs/Resources", "Prefabs");
        return folder;
    }

    private void AddToRegistry(PlaceableItem assetItem)
    {
        if (assetItem == null) return;
        if (_itemsProperty != null)
        {
            _itemsProperty.arraySize++;
            _itemsProperty.GetArrayElementAtIndex(_itemsProperty.arraySize - 1).objectReferenceValue = assetItem;
            _serializedRegistry.ApplyModifiedProperties();
        }
        else
        {
            _registry.Add(assetItem);
            EditorUtility.SetDirty(_registry);
        }
    }

    private void AddFromSelection()
    {
        var go = Selection.activeGameObject;
        if (go == null)
        {
            EditorUtility.DisplayDialog("Добавить", "Выберите объект в иерархии, на котором висит PlaceableItem.", "OK");
            return;
        }
        var item = go.GetComponent<PlaceableItem>();
        if (item == null)
        {
            EditorUtility.DisplayDialog("Добавить", "На выбранном объекте нет компонента PlaceableItem.", "OK");
            return;
        }
        PlaceableItem toAdd = PrefabUtility.IsPartOfAnyPrefab(item.gameObject)
            ? PrefabUtility.GetCorrespondingObjectFromSource(item)
            : null;
        if (toAdd == null)
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Сохранить как префаб",
                go.name,
                "prefab",
                GetPrefabsFolder());
            if (string.IsNullOrEmpty(path)) return;
            var prefabRoot = PrefabUtility.SaveAsPrefabAsset(go, path);
            toAdd = prefabRoot != null ? prefabRoot.GetComponent<PlaceableItem>() : null;
        }
        if (toAdd == null)
        {
            EditorUtility.DisplayDialog("Добавить", "Не удалось получить ссылку на префаб PlaceableItem.", "OK");
            return;
        }
        AddToRegistry(toAdd);
    }

    private void CreateNewItem()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "PlaceableItem";
        var item = go.AddComponent<PlaceableItem>();
        Undo.RegisterCreatedObjectUndo(go, "Create PlaceableItem");

        var folder = GetPrefabsFolder();
        var path = AssetDatabase.GenerateUniqueAssetPath(folder + $"/PlaceableItem_{go.GetInstanceID()}.prefab");
        var prefabRoot = PrefabUtility.SaveAsPrefabAsset(go, path);
        var assetItem = prefabRoot != null ? prefabRoot.GetComponent<PlaceableItem>() : null;

        if (assetItem == null)
        {
            EditorUtility.DisplayDialog("Ошибка", "Не удалось сохранить префаб.", "OK");
            return;
        }

        AddToRegistry(assetItem);
        Selection.activeGameObject = go;
        _selectedIndex = (_itemsProperty?.arraySize ?? 1) - 1;
        RebuildItemEditor(assetItem);

        DestroyImmediate(go);
    }
}
#endif
