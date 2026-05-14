using Godot;
using System;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;
using static Weapon;

public partial class FiringPipeLine : Node3D
{
    [Export]
    public float Pierce { get; set; } = 2f;
    [Export]
    public float Damage { get; set; } = 10f;
    [Export]
    public float RateOfFire { get; set; } = 100f;
    public enum FireModes { Manual, semiAuto, FullAuto };
    [Export]
    public FireModes FireMode = FireModes.Manual;

    [Export]
    public float Range = 100f;

    public RayCast3D WeaponRayCast;

    private double TimeSinceLastFired;
    private float SecondsPerBullet;
    private Vector3 MuzzlePos = new Vector3(0, 0, -0.6f);
    private GpuParticles3D MuzzleFlashParticle;
    // Called when the node enters the scene tree for the first time.
    public void Init(RayCast3D WeaponRayCast) {
        //calculate seconds per bullet
        SecondsPerBullet = 60 / RateOfFire;
        //set raycast range
        this.WeaponRayCast = WeaponRayCast;
        //initialise TimeSinceLastFired
        switch (FireMode)
        {

            case FireModes.Manual:
                TimeSinceLastFired = SecondsPerBullet;
                //add chambering animation here
                break;

            case FireModes.semiAuto:
                TimeSinceLastFired = SecondsPerBullet;
                break;

            case FireModes.FullAuto:
                TimeSinceLastFired = 0;
                break;

            default:
                GD.PushWarning("weapon fire mode is not manual,semi-auto,or full auto. how did you manage that?");
                TimeSinceLastFired = 0;
                break;
        }
    }
    public override void _Ready()
    {
        //get muzzle flash particle
        MuzzleFlashParticle = (GpuParticles3D)GD.Load<PackedScene>("res://Scenes/muzzle_flash.tscn").Instantiate();
        MuzzleFlashParticle.OneShot = true;//set it so the emitter shots out all particles at once.
        AddChild(MuzzleFlashParticle);
        MuzzleFlashParticle.Position = MuzzlePos;
    }

    public void PhysicsUpdate(double delta) {
        WeaponRayCast.TargetPosition = -WeaponRayCast.Basis.Z * Range;
        switch (FireMode)
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

    private void SemiAuto(double delta)
    {
        
        if (Input.IsActionPressed("fire_weapon") && TimeSinceLastFired <= 0)
        {
            Fire();
            TimeSinceLastFired = SecondsPerBullet;
            MuzzleFlash();
        }
        else
        {
            MuzzleFlashReset();
        }
        TimeSinceLastFired = Math.Max(0, TimeSinceLastFired - delta);
    }

    private void FullAuto(double delta)
    {
        if (Input.IsActionPressed("fire_weapon"))
        {
            TimeSinceLastFired += delta;
            for (; TimeSinceLastFired >= SecondsPerBullet; TimeSinceLastFired -= SecondsPerBullet)
            {
                Fire();
                MuzzleFlash();
                MuzzleFlashReset();
            }

        }
        //reset counter when user stop firing
        if (Input.IsActionJustReleased("fire_weapon"))
        {
            TimeSinceLastFired = SecondsPerBullet;
        }
    }

    private void Fire()
    {

        Node3D HitNode;
        float BulletPierce = Pierce;
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
                BulletPierce = ((DamageAble)HitNode).ProjectileHit(Damage, BulletPierce);
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

    private void MuzzleFlashReset()
    {
        MuzzleFlashParticle.Emitting = false;
        MuzzleFlashParticle.Restart();
    }
    private void MuzzleFlash() {
        MuzzleFlashParticle.Emitting = true;
    }
}
