using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;

public class SceneFinder : EditorWindow
{
    [MenuItem("UITK/Scene Finder")]
    public static void ShowWindow()
    {
        var window = GetWindow<SceneFinder>();
        window.titleContent = new UnityEngine.GUIContent("Scene Finder");
        window.minSize = new UnityEngine.Vector2(200, 100);
    }

    public void CreateGUI()
    {
        ScrollView sv = new ScrollView();
        VisualElement container = sv.Q("unity-content-container");

        string[] guids = AssetDatabase.FindAssets("t:Scene");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.StartsWith("Assets") && !path.Contains("Entity"))
                container.Add(new SceneLine(path));
        }

        rootVisualElement.Add(sv);
    }

    class SceneLine : VisualElement
    {
        public SceneLine(string path)
        {
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;
            Label label = new Label(path);
            label.style.unityTextOverflowPosition = TextOverflowPosition.Start;
            label.style.overflow = Overflow.Hidden;
            label.style.textOverflow = TextOverflow.Ellipsis;
            label.style.flexShrink = 1;
            Add(label);
            Add(new Button(() => Load(path)) { text = "Load"});
        }

        void Load(string path)
            => EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
    }
}
