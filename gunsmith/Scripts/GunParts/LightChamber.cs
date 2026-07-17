using Godot;
using System;

public partial class LightChamber : Node3D
{
	Connector AmmoConnector;
	Connector BarrelConnector;
	Connector MechanicalConnector;

	[Signal]
	public delegate void ReturnPipelineEventHandler(FiringPipeLine Pipe);
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AmmoConnector = GetNode<Connector>("GunPartBase/AmmoConnector");
		BarrelConnector = GetNode<Connector>("GunPartBase/BarrelConnector");
		MechanicalConnector = GetNode<Connector>("GunPartBase/MechanicalConnector");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Build() {
		FiringPipeLine Pipe = new FiringPipeLine();
		AmmoConnector.PropagatePipeLine(Pipe);
		BarrelConnector.PropagatePipeLine(Pipe);
		MechanicalConnector.PropagatePipeLine(Pipe);
		EmitSignal(SignalName.ReturnPipeline, Pipe);
    }

	public void ConnectBuildButton(Button button, FiringPipeLine[] WeaponPipeLineArray) {
		button.Pressed += Build;
		GD.Print("Connected!");
	}
}
