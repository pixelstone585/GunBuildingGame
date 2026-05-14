using Godot;
using System;

public partial class DamageAble : RigidBody3D
{
	[Export]
	public float Health { get; set; } = 100;
	[Export]
	public float Armor { get; set; } = 1f;
	[Export]
	public float PierceThreshold { get; set; } = 1.1f;
	[Export]
	public int IFrames = 10;

	private int IFrameCounter = 0;

	//calculate damage based on projectile Pirce value vs Target(self) Armor value and apply to self
	//return the projectiles leftover Pierce

	public float ProjectileHit(float Damage, float Pierce) {
        GD.Print("Ive been hit!");
        if (IFrameCounter > 0) { return Pierce; }//do nothing if the target(self) has I-frames
		Health -= Damage * Math.Min((Pierce / Armor), 1);
		IFrameCounter = IFrames;//give target(self) I-frames
		return (Pierce - PierceThreshold);
	}
	//damages the target
	public void Damage(float Damage) {
		Health -= Damage;
	}
	public void updateIFrames() {
		IFrameCounter = Math.Max(IFrameCounter - 1, 0);
	}
}
