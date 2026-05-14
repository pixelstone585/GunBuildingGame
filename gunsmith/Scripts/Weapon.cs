using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Weapon : Node3D
{
	[Export]
	public RayCast3D WeaponRayCast { get; set; }

    private FiringPipeLine[] PipeLines = new FiringPipeLine[4];



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //debugging
        FiringPipeLine tmp = (FiringPipeLine)GetNode("TestingPipeLine");
        tmp.Init(WeaponRayCast);
        RegisterPipeLine(tmp);
        
        //fallback if no raycast was assigned
        if (WeaponRayCast == null) {
			WeaponRayCast = new RayCast3D();
            AddChild(WeaponRayCast);
			GD.PushWarning("Weapon has not been assigned a raycast node, creating fallback node.");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        for (int i = 0; i < PipeLines.Length; i++) {
            if (PipeLines[i] != null) {
                PipeLines[i].PhysicsUpdate(delta);
            }
        }
    }

    public bool RegisterPipeLine(FiringPipeLine Pipe) {
        ReParentPipe(Pipe);
        bool IsSucsessfull = false;
        for (int i = 0; i < PipeLines.Length && !IsSucsessfull; i++) {
            if (PipeLines[i] == null) {
                PipeLines[i] = Pipe;
                IsSucsessfull = true;
            }
        }

        //return if the pipe was registered (eg if there are more then 4)
        return IsSucsessfull;
    }
    public void ClearPipeLines() {
        for (int i = 0; i < PipeLines.Length; i++)
        {
            PipeLines[i].QueueFree();
            PipeLines[i] = null;
        }
    }
    //reaparent FiringPipeline to self
    private void ReParentPipe(FiringPipeLine Pipe) {
        Node Parent = Pipe.GetParent();
        if (Parent != null) {
            Parent.RemoveChild(Pipe);
            AddChild(Pipe);
        }
    }
    
}
