using Godot;

public partial class BattlePlayer : CharacterBody2D
{
	[Export] public int MaxHp = 6;
	public int CurrentHp;
	[Export] public float Speed = 160f;
	[Export] public PackedScene ProjectileScene;
	[Export] public bool CanShoot = false;

	private AnimatedSprite2D animasi;
	private Vector2 lastDirection = Vector2.Down;

	[Signal] public delegate void PlayerDiedEventHandler();
	[Signal] public delegate void HpChangedEventHandler(int currentHp);

	public override void _Ready()
	{
		MaxHp = 6;
		CurrentHp = MaxHp;
		AddToGroup("Player");
		MotionMode = MotionModeEnum.Floating;

		animasi = GetNode<AnimatedSprite2D>("Sprite2D");
		CallDeferred(nameof(SetSpawnPosition));
	}

	private void SetSpawnPosition()
	{
		if (!string.IsNullOrEmpty(GameManager.TargetSpawnPoint))
		{
			var spawnPoint = GetParent().GetNodeOrNull<Marker2D>(GameManager.TargetSpawnPoint);
			if (spawnPoint != null)
			{
				GlobalPosition = spawnPoint.GlobalPosition;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (CurrentHp <= 0) return;

		Vector2 arah = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = arah * Speed;
		MoveAndSlide();

		if (arah.X > 0) animasi.Play("idle_kanan");
		else if (arah.X < 0) animasi.Play("idle_kiri");
		else if (arah.Y > 0) animasi.Play("idle_bawah");
		else if (arah.Y < 0) animasi.Play("idle_atas");

		if (arah != Vector2.Zero)
		{
			lastDirection = arah.Normalized();
		}

		if (CanShoot && (Input.IsActionJustPressed("ui_accept") || Input.IsMouseButtonPressed(MouseButton.Left)))
		{
			Shoot();
		}
	}

	private void Shoot()
	{
		if (ProjectileScene == null) return;

		var shot = ProjectileScene.Instantiate<LightProjectile>();
		shot.GlobalPosition = GlobalPosition;
		shot.Direction = lastDirection;
		shot.IsPlayerShot = true;

		GetParent().AddChild(shot);
	}

	public void TakeDamage(int damage)
	{
		CurrentHp -= damage;
		if (CurrentHp < 0) CurrentHp = 0;

		EmitSignal(SignalName.HpChanged, CurrentHp);

		if (CurrentHp <= 0)
		{
			CurrentHp = 0;
			EmitSignal(SignalName.PlayerDied);
		}
	}
}
