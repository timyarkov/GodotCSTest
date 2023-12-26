using Godot;
using System;

public partial class MainGame : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//InitGame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Update hud with player speed display
		HUD hud = GetNode<HUD>("Hud");
		Player p = GetNode<Player>("Player");
		hud.SetSpeedDisplay(p.Get("velocity").As<Vector3>().DistanceTo(Vector3.Zero));
	}

	// Some Ref but don't think needed in this case
	// https://docs.godotengine.org/en/stable/getting_started/first_2d_game/05.the_main_game_scene.html
	// https://docs.godotengine.org/en/stable/getting_started/first_2d_game/06.heads_up_display.html
	// public void InitGame() {

	// }
}
