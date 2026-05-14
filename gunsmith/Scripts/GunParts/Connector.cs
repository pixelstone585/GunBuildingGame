using Godot;
using System;
using System.Runtime.InteropServices;

public partial class Connector : Area3D
{
	Node3D Part;
	[Signal]
	public delegate void ApplyModifiersEventHandler(FiringPipeLine Pipe);
	Connector OutConnector;
	bool IsTermination = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void Init(Connector OutConnector, Node3D Part) {
		this.OutConnector = OutConnector;
		this.Part = Part;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void PropagateInternal(FiringPipeLine Pipe) {
		EmitSignal(SignalName.ApplyModifiers, Pipe);
		if (IsTermination) {
			return;
		}
		OutConnector.PropagateExternal(Pipe);
	}
    public void PropagateExternal(FiringPipeLine Pipe)
    {
		Godot.Collections.Array<Area3D> Areas = GetOverlappingAreas();
		Connector Next=null;
		bool NextFound = false;
		for (int i = 0; i < Areas.Count && !NextFound; i++) {
			if (Areas[i] is Connector && Areas[i].GlobalTransform.IsEqualApprox(GlobalTransform)) {
				Next = (Connector)Areas[i];
				NextFound = true;
            }
		}
		if (NextFound) {
			Next.PropagateInternal(Pipe);
		}
		
    }
}
