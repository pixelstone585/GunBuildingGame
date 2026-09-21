using Godot;
using Godot.Collections;
using System;
using System.Xml;

public partial class PartGrid : Sprite3D
{
	[Export]
	Vector2I GridSize { get; set; }
	[Export]
	BuilderCam Camera { get; set; }

	Vector2 SLOTSIZEPIX;
	Part[,] PartArray;

	Area3D GridArea;
	CollisionShape3D GridCollisionShape;

	Part HeldPart;
	Vector3 GrabPoint;
	Vector3 GrabPlaceOffset;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//get nodes
		GridArea = GetNode<Area3D>("GridArea");
		GridCollisionShape = GridArea.GetNode<CollisionShape3D>("GridCollisionShape");
        //create part array
        PartArray = new Part[GridSize[0], GridSize[1]];
		//TESTING
		PackedScene TestPartScene = ResourceLoader.Load<PackedScene>("res://Scenes/test_part.tscn");
		Part TestPart = CreatePart(TestPartScene);
        PlacePart(TestPart, Vector2I.One);
		GD.Print(TestPart.GlobalPosition);
		//--------------------------------
		//configure gird cell size
		SLOTSIZEPIX = new Vector2(Texture.GetWidth(), Texture.GetHeight());
		RegionRect = new Rect2(Vector2.Zero, SLOTSIZEPIX * GridSize);
		PixelSize = 1 / SLOTSIZEPIX[0];

		//configure area3d
		GridArea.Position = new Vector3(GridSize[0] / 2, GridSize[1] / 2, 0);//position area in the middle of the grid
		//configure area3d shape
		BoxShape3D areaShape = new BoxShape3D();
		areaShape.Size = new Vector3(GridSize[0], GridSize[1],0.1f);
		GridCollisionShape.Shape =areaShape;
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		
		//drag and drop parts
		if (Input.IsActionJustPressed("pick_up_part")) {
            HeldPart=PickUpPart();
		}
		if (Input.IsActionPressed("pick_up_part")&&HeldPart!=null) {
            Dictionary RayCastResult = Camera.MouseRayCast(20, true);
			if (RayCastResult.Count > 0)
			{
				Vector3 PartPosition = (Vector3)RayCastResult["position"];
				HeldPart.Position = ToLocal(PartPosition)+GrabPoint;
			}
        }
		if (Input.IsActionJustReleased("pick_up_part") && HeldPart != null ) { 
			Vector2I PlaceIndex = RayCastFindIndex(GrabPlaceOffset);
			
			if (!(PlaceIndex[0] == -1 || PlaceIndex[1] == -1) && CanPlace(HeldPart, PlaceIndex))
			{
				PlacePart(HeldPart, PlaceIndex);
            }
			else {
				HeldPart.QueueFree();
			}
		}

		//rotate held part
		if (Input.IsActionPressed("pick_up_part") && HeldPart != null && Input.IsActionJustPressed("rotate_part_counter_clockwise")) {
			HeldPart.RotateCCW();

		}
        if (Input.IsActionPressed("pick_up_part") && HeldPart != null && Input.IsActionJustPressed("rotate_part_clockwise"))
        {
            HeldPart.RotateCW();
        }
    }

	public void PlacePart(Part part, Vector2I index)
	{
		part.Index = index;
		bool[,] PartShape = part.ShapeArray;
		int LenX = PartShape.GetLength(0);
        int LenY = PartShape.GetLength(1);
		int ArrLenX = PartArray.GetLength(0);
        int ArrLenY = PartArray.GetLength(1);
		//escape if placing would require going out of bounds
		if (index[0] + LenX - 1 >= ArrLenX || index[1] + LenY - 1 >= ArrLenY) {
			return;
		}

        for (int ix = 0; ix < LenX; ix++){
			for (int iy = 0; iy < LenY; iy++) {
				if (PartShape[iy, ix]) {
					PartArray[index[0] + ix, index[1] + iy] = part;
				}
			}
		}
		part.Position = new Vector3(index[0] + LenX / 2,  index[1] + LenY / 2,0.6f);
		GD.Print(PartShape[0, 1]);
    }

	public bool CanPlace(Part part, Vector2I index) {
		if (index[0] == -1 && index[1] == -1) { return false; }

		bool[,] PartShape = part.ShapeArray;
        int LenX = PartShape.GetLength(0);
        int LenY = PartShape.GetLength(1);
        int ArrLenX = PartArray.GetLength(0);
        int ArrLenY = PartArray.GetLength(1);
        //return false if placing would require going out of bounds
        if (index[0] + LenX - 1 >= ArrLenX || index[1] + LenY - 1 >= ArrLenY)
        {
            return false;
        }

        for (int ix = 0; ix < LenX; ix++)
        {
            for (int iy = 0; iy < LenY; iy++)
            {
                if (PartShape[iy, ix])
                {
					if (PartArray[index[0] + ix, index[1] + iy] != null) {
						return false;
					}
                }
            }
        }
		return true;
    }

	public Part RemovePart(Vector2I Index) {
		Part part = PartArray[Index[0], Index[1]];
		if (part == null) {
			return null;
		}
		Vector2I PartIndex = part.Index;
        bool[,] PartShape = part.ShapeArray;
        int LenX = PartShape.GetLength(0);
        int LenY = PartShape.GetLength(1);
        for (int ix = 0; ix < LenX; ix++)
        {
            for (int iy = 0; iy < LenY; iy++)
            {
                if (PartShape[iy, ix])
                {
					PartArray[PartIndex[0] + ix, PartIndex[1] + iy] = null;
                }
            }
        }
		return part;
    }

	public Vector2I GetIndexFromPosition(Vector2 Position) {
		Vector2I index = (Vector2I)Position;

        if (index[0] < GridSize[0] && index[0] >=0 && index[1] < GridSize[1] && index[1] >=0 ) {//cheack that the result is in array bounds
			return index;
		}
		return -Vector2I.One;//return (-1,-1) if position is out of bounds
	}
	public Vector2I GetIndexFromPosition(Vector3 WorldPosition) {
		Vector3 localPos = ToLocal(WorldPosition);//convert to local coordinants
		return GetIndexFromPosition(new Vector2(localPos[0], localPos[1]));//get xy componants
    }

	public Part PickUpPart() {
		//handle taking parts from inventory
		Control UiClickedOn = GetViewport().GuiGetHoveredControl();
        if ( UiClickedOn != null && UiClickedOn.Owner is ItemUi) {
			Part part = CreatePart(((ItemUi)UiClickedOn.Owner).PartScene);

			GrabPlaceOffset = Vector3.Zero; 
            return part;
        }
		//handle taking item from grid
        Dictionary RayCastResult = Camera.MouseRayCast(20, true);
        if (RayCastResult.Count > 0 && GridArea == (Node3D)RayCastResult["collider"])
        {
			GrabPoint = (Vector3)RayCastResult["position"];
            Vector2I index = GetIndexFromPosition(GrabPoint);
			Part part= RemovePart(index);
			if (part == null) {
				return null;
			}

			//figure out where the part is bieng grabbed from
            GrabPoint = part.Position-ToLocal(GrabPoint) ;
            GrabPoint.Z = 0;
			Vector3 OriginLocal = part.Position-ToLocal(part.ToGlobal(part.PartOrigin));
			GrabPlaceOffset = OriginLocal - GrabPoint;
			GD.Print(GrabPlaceOffset);
            return part;
        }
		return null;
    }

	public Part CreatePart(PackedScene PartScene) {
		Part NewPart = PartScene.Instantiate<Part>();
		AddChild(NewPart);
		return NewPart;
	}

	public Vector2I RayCastFindIndex(Vector3 Offset) {
        Dictionary RayCastResult = Camera.MouseRayCast(20, true);
        if (RayCastResult.Count > 0 && GridArea == (Node3D)RayCastResult["collider"])
        {
			Vector2I index = GetIndexFromPosition((Vector3)RayCastResult["position"]+ Offset);
			GD.Print((Vector3)RayCastResult["position"] + Offset);
            return index;
        }
		return -Vector2I.One;
    }
}
