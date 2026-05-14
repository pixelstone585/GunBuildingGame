using Godot;
using System;

public partial class DummyEnemy : DamageAble
{
	Label3D HealthBar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		HealthBar = GetNode<Label3D>("HealthBar");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(Name+": "+Health);
		updateIFrames();
		HealthBar.Text = "Hp:"+Health;
	}
}
