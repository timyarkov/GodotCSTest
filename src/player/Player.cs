using Godot;
using System;

// Architecturally dunno if having the root node be the char is bestest but
// avoids mess of getters so         y   a    g    n    i          ?

// Camera stuffs handled by camera script, not player script 

public partial class Player : CharacterBody3D
{
	// Properties
	[Export]
	public float BaseSpeed = 5.0f;

	[Export]
	public float SprintMult = 1.5f;

	[Export]
	public float Acceleration = 0.8f;

	[Export]
	public float JumpVelocity = 4.5f;

	private float _currMaxSpeed;
	// maybe dodgy speeding up solution
	private float _currSpeed = 0.0f;

	private const float _turnWeight = 0.6f; // Magic business

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	// Overrides
	public override void _Ready()
	{
		// Don't collide player with camera
		GetNode<SpringArm3D>("SpringArm3D").AddExcludedObject(GetRid());
		_currMaxSpeed = BaseSpeed;
	}

	public override void _Process(double delta)
	{
		// Move camera spring arm so it moves with the player
		var springArm = GetNode<SpringArm3D>("SpringArm3D");
		springArm.Set("position", Get("position").As<Vector3>());

		var t1 = springArm.Get("position").As<Vector3>();
		var t2 = Get("position").As<Vector3>();
		//GD.Print($"Spring: {t1.X} {t1.Y} {t1.Z} || char: {t2.X} {t2.Y} {t2.Z}");
	}

	public override void _Input(InputEvent @event)
	{
		// Sprinting
		if (@event.IsActionPressed("sprint"))
		{
			_currMaxSpeed = BaseSpeed * SprintMult;
		}
		else if (@event.IsActionReleased("sprint"))
		{
			_currMaxSpeed = BaseSpeed;
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
		// Exccceeeeeept since following https://www.youtube.com/watch?v=UpF7wm0186Q
		// have to flip around left right forwards and backwards...math etc.
		var direction = new Vector3
		{
			X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
			Z = Input.GetActionStrength("move_backward") - Input.GetActionStrength("move_forward")
		};
		direction = direction.Normalized();

		if (direction != Vector3.Zero)
		{
			// So that camera and model follows where camera faces
			direction = direction.Rotated(
				Vector3.Up,
				//GetNode<Node3D>("CameraRootNode/HorizontalPivot").Get("rotation").As<Vector3>().Y
				GetNode<Node3D>("SpringArm3D").Get("rotation").As<Vector3>().Y
			);

			velocity.X = direction.X * _currMaxSpeed;
			velocity.Z = direction.Z * _currMaxSpeed;

			_currSpeed = _currSpeed >= _currMaxSpeed ? _currMaxSpeed : _currSpeed + Acceleration;
			var factor = _currSpeed / _currMaxSpeed;
			var reductionVector = new Vector3
			{
				X = factor,
				Y = 1,
				Z = factor
			};
			velocity *= reductionVector;

			// Make model face where going, smoothed out
			var model = GetNode<Node3D>("Model");
			// https://www.reddit.com/r/godot/comments/135xpya/comment/jilwihx/?utm_source=share&utm_medium=web3x&utm_name=web3xcss&utm_term=1&utm_content=share_button
			// A bit yikes but basically set the global transformation to be if a transformation 
			// of looking right in front of the current position, interpolated for smoooth 
			model.GlobalTransform = model.GlobalTransform.InterpolateWith(
				model.GlobalTransform.LookingAt(Get("global_position").As<Vector3>() - direction, Vector3.Up),
				_turnWeight
			);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _currMaxSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _currMaxSpeed);
			_currSpeed = Mathf.MoveToward(_currSpeed, 0, Acceleration);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
