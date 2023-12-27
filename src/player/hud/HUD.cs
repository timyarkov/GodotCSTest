using Godot;
using System;

public partial class HUD : Control
{
	// Don't think needed for just speed display more for buttons
	// https://docs.godotengine.org/en/stable/getting_started/step_by_step/signals.html
	// [Signal]
	// public delegate void StartGameEventHandler();

	public void SetSpeedDisplay(double Speed)
	{
		Label speedLabel = GetNode<Label>("SpeedDisplay");
		speedLabel.Text = $"SPEED: {Speed:N2}";
	}

	public void SetVelocityDisplay(Vector3 Velocity) {
		GetNode<Label>("VelX").Text = $"VelX = {Velocity.X}";
		GetNode<Label>("VelY").Text = $"VelY = {Velocity.Y}";
		GetNode<Label>("VelZ").Text = $"VelZ = {Velocity.Z}";
	}
}
