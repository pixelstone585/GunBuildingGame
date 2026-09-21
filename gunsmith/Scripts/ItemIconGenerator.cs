using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

public partial class ItemIconGenerator : SubViewport
{
	public Dictionary<String, Texture2D> ItemIcons;
	public static ItemIconGenerator Instance;

	private Part PrevPart;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ItemIcons = new Dictionary<String, Texture2D>();
		Instance = this;
		RenderTargetUpdateMode = UpdateMode.Once;
    }
	public Texture2D GetIcon(PackedScene PartScene) {
		if (PrevPart != null) {
            PrevPart.QueueFree(); //clear previously renderd part
        }
        //return icon if it was previously generated
        if (ItemIcons.Keys.Contains(PartScene.ResourcePath)){
			return ItemIcons[PartScene.ResourcePath];
		}

		Part part = PartScene.Instantiate<Part>();
		AddChild(part);
        RenderTargetUpdateMode = UpdateMode.Once;
        Texture2D IconTex = GetTexture();
		ItemIcons.Add(PartScene.ResourcePath, IconTex);
		PrevPart = part;

		return IconTex;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
