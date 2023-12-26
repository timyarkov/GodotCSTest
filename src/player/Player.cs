using Godot;
using System;

// Architecturally dunno if having the root node be the char is bestest but
// avoids mess of getters so         y   a    g    n    i          ?
public partial class Player : CharacterBody3D
{
	// Properties
	[Export]
	public float BaseSpeed = 5.0f;

	[Export]
	public float JumpVelocity = 4.5f;

	[Export]
	public float CameraSensitivity = 0.01f;
	// Private
	private bool _mouseCaptured;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	// Overrides
	public override void _Ready()
	{
		CaptureMouse();
		_mouseCaptured = true;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion && _mouseCaptured)
		{
			// https://murphysdad.substack.com/p/how-to-implement-a-simple-3rd-person
			// Camera Movement
			var relativePos = @event.Get("relative").As<Vector2>();
			var rotation = relativePos * CameraSensitivity;

			var horPivot = GetNode<Node3D>("CameraRootNode/HorizontalPivot");
			horPivot.Rotate(Vector3.Down, rotation.X);

			var vertPivot = GetNode<Node3D>("CameraRootNode/HorizontalPivot/VerticalPivot");
			vertPivot.Rotate(Vector3.Right, rotation.Y);

			// Stop up/down rotation if above/below
			var rotationThreshold = 1.15f; // magic radians number that looks ok
			var currVertRot = vertPivot.Get("rotation").As<Vector3>();
			if (currVertRot.X > rotationThreshold)
			{
				vertPivot.Set("rotation", new Vector3(rotationThreshold, 0, 0));
			}
			else if (currVertRot.X < -rotationThreshold)
			{
				vertPivot.Set("rotation", new Vector3(-rotationThreshold, 0, 0));
			}

			//GD.Print(currVertRot);
		}
	}

	public override void _Process(double delta)
	{
		// Capture/Uncapture Mouse
		if (Input.IsActionJustPressed("mouse_capture_toggle"))
		{
			if (_mouseCaptured)
			{
				UnCaptureMouse();
			}
			else
			{
				CaptureMouse();
			}

			_mouseCaptured = !_mouseCaptured;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// https://murphysdad.substack.com/p/how-to-implement-a-simple-3rd-person
		var direction = new Vector3
		{
			X = Input.GetActionStrength("move_left") - Input.GetActionStrength("move_right"),
			Z = Input.GetActionStrength("move_forward") - Input.GetActionStrength("move_backward")
		};
		direction = direction.Normalized();

		if (direction != Vector3.Zero)
		{

			// So that camera and model follows where camera faces
			direction = direction.Rotated(
				Vector3.Up,
				GetNode<Node3D>("CameraRootNode/HorizontalPivot").Get("rotation").As<Vector3>().Y
			);

			velocity.X = direction.X * BaseSpeed;
			velocity.Z = direction.Z * BaseSpeed;

			var model = GetNode<Node3D>("Model");
			model.LookAt(Get("global_position").As<Vector3>() - direction, Vector3.Up);
			//model.LookAt(Get("translation").As<Vector3>() - direction, Vector3.Up);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, BaseSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, BaseSpeed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	// Util
	public void CaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public void UnCaptureMouse()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
