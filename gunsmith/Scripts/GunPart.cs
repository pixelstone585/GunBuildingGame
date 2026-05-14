using Godot;
using System;

public partial class GunPart : MeshInstance3D
{
	public Vector3 Origin = Vector3.Zero;
	[Export]
	public Vector3 size { get; set; }
	public Vector3 Clearance = Vector3.Zero;
	public Vector3 ClearanceOrigin=Vector3.Zero;

	public void AutoSetOrigin() {
		
		Origin = size / 2;
    }

	public override void _Ready() {
        AutoSetOrigin();
    }

	public void DisableCollision() {
		GetNode<CollisionShape3D>("PartPhysicsBody/PartCollider").SetDeferred("disabled",true);
	}
}
