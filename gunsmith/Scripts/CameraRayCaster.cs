using Godot;
using Godot.Collections;
using System;

public partial class CameraRayCaster : Camera3D
{
	private GunPart LastObject;
	private Vector3 LastPosition;


    private RayCast3D RayCast;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RayCast = GetNode<RayCast3D>("RayCast");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		
    }

	public (Vector3 Position,GunPart TargetPartBase, Vector3 normal) MouseRayCast() {
		Vector2 MousePos = GetViewport().GetMousePosition();
		Vector3 RayEnd = ProjectLocalRayNormal(MousePos)*10;

        RayCast.TargetPosition = RayEnd;
		RayCast.ForceRaycastUpdate();
		if (RayCast.IsColliding()) {
			LastObject = ((GunPart)(((Node3D)RayCast.GetCollider()).GetParent()));//get position of the owner of the GunPartBase scene
																				  //LastPosition = ((Node3D)LastObject.Owner).Position;
			LastPosition = RayCast.GetCollisionPoint();
        }
		//return (RayCast.GetCollisionPoint(),RayCast.GetCollisionNormal());
		return (LastPosition, LastObject, RayCast.GetCollisionNormal());

    }

	public void SetPreviewExclusion(GunPart Preview) {
		RayCast.ClearExceptions();
        StaticBody3D Collider = Preview.GetNode<StaticBody3D>("PartPhysicsBody");
		RayCast.AddException(Collider);
	}

	public bool isAimedAtObject() {
		return RayCast.IsColliding();
	}
}
