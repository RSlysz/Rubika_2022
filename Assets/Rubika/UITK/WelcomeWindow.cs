using UnityEditor;
using UnityEngine.UIElements;

public class WelcomeWindow : EditorWindow
{
    [MenuItem("UITK/Welcome Window")]
    public static void ShowWindow()
    {
        var window = GetWindow<WelcomeWindow>();
        window.titleContent = new UnityEngine.GUIContent("Welcome !!!!");
    }

    Toggle @switch;
    TextField sentence;

    public void CreateGUI()
    {
        // Get root object of this window
        VisualElement root = rootVisualElement;

        // Direct adding of VisualElements
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import USS
        var stylesheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Rubika/UITK/WelcomeWindow.uss");
        root.styleSheets.Add(stylesheet);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Rubika/UITK/WelcomeWindow.uxml");
        VisualElement treeFromUXML = visualTree.Instantiate();
        root.Add(treeFromUXML);

        // Link elements
        var button = root.Q<Button>("SendToLog");
        @switch = root.Q<Toggle>("Switch");
        sentence = root.Q<TextField>("Sentence");

        button.clicked += SendToLog;
    }

    void SendToLog()
    {
        if (@switch.value)
            UnityEngine.Debug.Log(sentence.text);
    }
}
