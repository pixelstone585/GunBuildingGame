using Godot;
using System;

public partial class Projectile : Node3D
{
	[Export]
	public float Speed { get; set; } = 10f;

	[Export]
	public float Gravity { get; set; } = 0f;
	[Export]
	public float Pierce { get; set; } = 2f;
	[Export]
	public float Damage { get; set; } = 10f;

	private float YSpeed = 0;
	private Node3D display;
	private RayCast3D rayCast;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		display = GetNode<Node3D>("ProjectileDisplay");
		rayCast = GetNode<RayCast3D>("ProjectileRayCast");
		if (Speed >= 30) {
			Speed = 30;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        //cheack previous raycast result
        Node3D HitObject = (Node3D)rayCast.GetCollider();
		if (HitObject is DamageAble) {
            Pierce=((DamageAble)HitObject).ProjectileHit(Damage, Pierce);//(ignore the jankery)update Pierce to prevoius value - targets Pirece threshold
			//destroy projectile if it failes to pierce
			if (Pierce <=0) {
				QueueFree();
			}
        }
		//update position
		YSpeed += Gravity*(float)delta;
		Vector3 NewPosition = Position + new Vector3(0, -YSpeed*(float)delta, 0) + Basis.Z * Speed * (float)delta;
        display.LookAt(NewPosition);
		rayCast.TargetPosition = NewPosition;

		Position = NewPosition;
    }
}
