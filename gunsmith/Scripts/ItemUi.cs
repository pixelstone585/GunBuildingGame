using Godot;
using Godot.Collections;
using System;

public partial class ItemUi : Control
{
    public PackedScene PartScene; // scene of the part asociated with the item
    private String PartName; //part name
    private String Description; // part description
    private Texture2D Icon; // part icon. TODO: find out how to generate on runtime

    private Label NameDisplay;
    private TextureRect IconDisplay;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("!");
        //get nodes
        NameDisplay = GetNode<Label>("Container/PartName");
        IconDisplay = GetNode<TextureRect>("Container/PartImage");

        //TEST CODE
        LoadJson("res://Assets/PartItems/TestItemJson.json");
        NameDisplay.Text = PartName;
        IconDisplay.Texture = Icon;
    }
    public void Init(String name, String description, PackedScene scene)
    {
        PartName = name;
        Description = description;
        PartScene = scene;
    }

    public void JsonStoreTest()
    {
        Dictionary JsonData = new Dictionary{
            { "Name",PartName},
            { "Description",Description},
            { "ScenePath",PartScene.ResourcePath}
        };
        String json = Json.Stringify(JsonData);
        using FileAccess file = FileAccess.Open("res://Assets/PartItems/TestItemJson.json", FileAccess.ModeFlags.Write);
        file.StoreString(json);
    }

    public void LoadJson(String FilePath) {
        using FileAccess file = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
        String JsonUnParsed = file.GetAsText();
        Json JSON = new Json();
        Error error = JSON.Parse(JsonUnParsed);
        if (error == Error.Ok)
        {
            Dictionary<String, Variant> DataDict = JSON.Data.AsGodotDictionary<String, Variant>();
            PartName = (String)DataDict["Name"];
            Description = (String)DataDict["Description"];
            PartScene = GD.Load<PackedScene>((String)DataDict["ScenePath"]);
            Icon = ItemIconGenerator.Instance.GetIcon(PartScene);
        }
        else {
            GD.PrintErr("Failed to load item: " + error.ToString());
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
