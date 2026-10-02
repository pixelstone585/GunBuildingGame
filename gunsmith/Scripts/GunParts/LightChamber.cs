using Godot;
using System;

public partial class LightChamber : Part
{
	Connector AmmoConnector;
	Connector BarrelConnector;

	[Signal]
	public delegate void ReturnPipelineEventHandler(FiringPipeLine Pipe);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AmmoConnector = GetNode<Connector>("Pivot/PartMesh/AmmoConnector");
		BarrelConnector = GetNode<Connector>("Pivot/PartMesh/BarrelConnector");
		Init();
    }
    public override void init() //called when Init() is called
    {
        bool[,] ShapeArray = new bool[,]
        {
            {true,true}
        };
        CreateShapeArrays(ShapeArray);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Build() {
		FiringPipeLine Pipe = new FiringPipeLine();
		AmmoConnector.PropagatePipeLine(Pipe);
		BarrelConnector.PropagatePipeLine(Pipe);
		EmitSignal(SignalName.ReturnPipeline, Pipe);
    }

	public void ConnectBuildButton(Button button, FiringPipeLine[] WeaponPipeLineArray) {
		button.Pressed += Build;
		GD.Print("Connected!");
	}
}
