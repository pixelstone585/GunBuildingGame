using Godot;
using System;

public partial class TestPart : Part
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ShapeArray = new bool[,]
		{
			{true,true},
			{ true,false}
		};
		GrabPoint = new Vector3(-0.5f,-0.5f,0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
