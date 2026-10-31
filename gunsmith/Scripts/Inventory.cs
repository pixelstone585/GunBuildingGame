using Godot;
using System;

public partial class Inventory : Control
{
	[Export]
	public GridContainer InventoryGrid { get; set; }
	public String ItemUiPath = "res://Scenes/item_ui.tscn";
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddPart(Part part) {
        AddPart(part.GetPartPath());
    }
	public void AddPart(String PartJsonPath) {
        GD.Print("add");
        Godot.Collections.Array<Node> NodeList = InventoryGrid.GetChildren();
        //search for existing slot and add to it if possible
        foreach (Node item in NodeList)
        {
            if (item is ItemUi && ((ItemUi)item).JsonPath == PartJsonPath)
            {
                ((ItemUi)item).Quantity++;
                return;
            }
        }
        //create new slot
        ItemUi NewSlot = GD.Load<PackedScene>(ItemUiPath).Instantiate<ItemUi>();
        NewSlot.JsonPath = PartJsonPath;
        InventoryGrid.AddChild(NewSlot);
    }
}
