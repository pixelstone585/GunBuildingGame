using Godot;
using Godot.Collections;
using System;

public partial class Inventory : Control
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
		PartName = "Test part";
		Description = "hey look at me, im a test subject! ist this fun?";
		PartScene = GD.Load<PackedScene>("res://Scenes/GunParts/test_part.tscn");
        JsonStoreTest();
	}
	public void Init(String name,String description,PackedScene scene) {
		PartName = name;
		Description = description;
		PartScene = scene;
	}

	public void JsonStoreTest() {
		Dictionary JsonData = new Dictionary{
			{ "Name",PartName},
			{ "Description",Description},
			{ "ScenePath",PartScene.ResourcePath}
		};
		String json = Json.Stringify(JsonData);
		using FileAccess file = FileAccess.Open("res://Assets/PartItems/TestItemJson.json", FileAccess.ModeFlags.Write);
		file.StoreString(json);
		GD.Print("stored!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
