using Godot;
using System;
using System.Runtime.CompilerServices;
using static FiringPipeLine;
using static System.Net.Mime.MediaTypeNames;

public partial class Weapon : Node3D
{
	[Export]
	public RayCast3D WeaponRayCast { get; set; }
    [Export]
    public Vector3 MuzzlePos { get; set; }

    private FiringPipeLine PipeLine;

    public GpuParticles3D MuzzleFlashParticle;

    private double FireDelay;
    private double FireTimer=0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //debugging
        FiringPipeLine tmp = GetNode<FiringPipeLine>("TestingPipeLine");
        RegisterPipeLine(tmp);

        //get muzzle flash particle
        MuzzleFlashParticle = GD.Load<PackedScene>("res://Scenes/muzzle_flash.tscn").Instantiate<GpuParticles3D>();
        MuzzleFlashParticle.OneShot = true;//set it so the emitter shots out all particles at once.
        AddChild(MuzzleFlashParticle);
        MuzzleFlashParticle.Position = MuzzlePos;

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
        WeaponRayCast.TargetPosition = -WeaponRayCast.Basis.Z * PipeLine.Range;
        switch (PipeLine.FireMode)
        {

            case FireModes.Manual:
                SemiAuto(delta);
                //add chambering animation here
                break;

            case FireModes.semiAuto:
                SemiAuto(delta);
                break;

            case FireModes.FullAuto:
                FullAuto(delta);
                break;

            default:
                GD.PushWarning("weapon fire mode is not manual,semi-auto,or full auto. how did you manage that?");
                break;
        }
    }

    public void RegisterPipeLine(FiringPipeLine Pipe) {
        PipeLine = Pipe;
        //setup variables
        FireDelay = 1 / (60 * Pipe.RateOfFire);//get the delay between bullets from the rate of fire(rpm -> s/round)
        WeaponRayCast.TargetPosition = WeaponRayCast.TargetPosition * Pipe.Range;//set raycast range
    }
    public void ClearPipeLines() {
        PipeLine = null;
    }

    private void Fire()
    {
        GD.Print("BANG!");
        Node3D HitNode;
        float BulletPierce = PipeLine.Pierce;
        //loop while bullet 
        while (BulletPierce > 0)
        {
            //get collider
            HitNode = (Node3D)WeaponRayCast.GetCollider();
            //stop cheacking targets if non were hit
            if (HitNode == null)
            {
                BulletPierce = 0;
            }
            //logic for hitting a damageable object
            if (HitNode is DamageAble)
            {
                //(ignore the jankery)update Pierce to prevoius value - targets Pirece threshold
                BulletPierce = ((DamageAble)HitNode).ProjectileHit(PipeLine.Damage, BulletPierce);
                //add hit object to blacklist
                WeaponRayCast.AddException((DamageAble)HitNode);
            }
            else
            {
                BulletPierce = 0;
            }
            //update raycast
            WeaponRayCast.ForceRaycastUpdate();
        }
        //reset blacklist
        WeaponRayCast.ClearExceptions();
    }

    public void SemiAuto(double delta) {
        //reset muzzle flash one tick after it was used
        if (FireTimer == FireDelay) {
            MuzzleFlashReset();
        }
        if (FireTimer > 0) { FireTimer -= delta; }//step timer
        //fire if input was presssed and if the delay has passed
        if (Input.IsActionJustPressed("fire_weapon") && FireTimer <= 0) {
            Fire();
            FireTimer = FireDelay;
            MuzzleFlash();
        }
    }

    public void FullAuto(double delta) {
        //reset muzzle flash one tick after it was used
        if (FireTimer == FireDelay)
        {
            MuzzleFlashReset();
        }
        if (FireTimer > 0) { FireTimer -= delta; }//step timer
        //fire if input was presssed and if the delay has passed
        if (Input.IsActionPressed("fire_weapon") && FireTimer <= 0)
        {
            Fire();
            FireTimer = FireDelay;
            MuzzleFlash();
        }
    }
    private void MuzzleFlashReset()
    {
        MuzzleFlashParticle.Emitting = false;
        MuzzleFlashParticle.Restart();
    }
    private void MuzzleFlash()
    {
        MuzzleFlashParticle.Emitting = true;
    }

}
