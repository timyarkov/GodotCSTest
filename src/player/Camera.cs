using Godot;
using System;

// Adapted from https://www.youtube.com/watch?v=UpF7wm0186Q
public partial class Camera : SpringArm3D
{
	[Export]
	public float CameraSensitivity = 0.01f;
	private bool _mouseCaptured;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TopLevel = true;
		CaptureMouse();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && _mouseCaptured)
		{
			var rotation = Get("rotation_degrees").As<Vector3>();
			rotation.X -= @event.Get("relative").As<Vector2>().Y * CameraSensitivity;
			rotation.X = Math.Clamp(rotation.X, -90.0f, 30.0f);

			rotation.Y -= @event.Get("relative").As<Vector2>().X * CameraSensitivity;
			rotation.Y = _wrapDegrees(rotation.Y);

			Set("rotation_degrees", rotation);
		}
		else if (@event.IsActionPressed("mouse_capture_toggle"))
		{
			if (_mouseCaptured)
			{
				UnCaptureMouse();
			}
			else
			{
				CaptureMouse();
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Util
	public void CaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_mouseCaptured = true;
	}

	public void UnCaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		_mouseCaptured = false;
	}

	private float _wrapDegrees(float deg)
	{
		if (deg > 360)
		{
			return deg - 360;
		}
		else if (deg < 0)
		{
			return 360 - Math.Abs(deg);
		}
		else
		{
			return deg;
		}
	}
}
