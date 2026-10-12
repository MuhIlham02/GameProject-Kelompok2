using Godot;

public partial class ShadowEnemy : CharacterBody2D
{
	[Export] public int MaxHp = 6;
	public int CurrentHp;
	[Export] public float Speed = 45f;
	[Export] public float DetectionRange = 75f;
	[Export] public float StopDistance = 50f;
	[Export] public float RetreatDistance = 35f;
	[Export] public PackedScene ProjectileScene;

	[ExportGroup("Sprite Arah (Khusus jika pakai Sprite2D)")]
	[Export] public Texture2D TextureDepan;
	[Export] public Texture2D TextureBelakang;
	[Export] public Texture2D TextureKiri;
	[Export] public Texture2D TextureKanan;

	[Signal] public delegate void BattleStartedEventHandler();
	[Signal] public delegate void EnemyDiedEventHandler();
	[Signal] public delegate void HpChangedEventHandler(int currentHp);

	private BattlePlayer targetPlayer;
	private AnimatedSprite2D animSprite;
	private Sprite2D normalSprite;

	private bool isAggro = false;
	private double shootCooldown = 2.4;
	private double shootTimer = 0;
	private double contactDamageCooldown = 1.0;
	private double contactDamageTimer = 0;

	public override void _Ready()
	{
		MaxHp = 6;
		CurrentHp = MaxHp;
		AddToGroup("Enemy");
		MotionMode = MotionModeEnum.Floating;

		animSprite = GetNodeOrNull<AnimatedSprite2D>("Sprite2D");
		normalSprite = GetNodeOrNull<Sprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (CurrentHp <= 0) return;

		if (targetPlayer == null)
		{
			targetPlayer = GetTree().GetFirstNodeInGroup("Player") as BattlePlayer 
						   ?? GetParent()?.GetNodeOrNull<BattlePlayer>("Player");
		}

		if (targetPlayer == null) return;

		float jarak = GlobalPosition.DistanceTo(targetPlayer.GlobalPosition);

		if (!isAggro && jarak <= DetectionRange)
		{
			isAggro = true;
			EmitSignal(SignalName.BattleStarted);
		}

		if (isAggro)
		{
			Vector2 arahKePlayer = (targetPlayer.GlobalPosition - GlobalPosition).Normalized();
			UpdateArahMusuh(arahKePlayer);

			if (jarak > StopDistance)
			{
				Velocity = arahKePlayer * Speed;
			}
			else if (jarak < RetreatDistance)
			{
				Velocity = -arahKePlayer * (Speed * 0.9f);
			}
			else
			{
				Velocity = Vector2.Zero;
			}

			MoveAndSlide();

			contactDamageTimer += delta;
			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				var collision = GetSlideCollision(i);
				if (collision.GetCollider() is BattlePlayer player && contactDamageTimer >= contactDamageCooldown)
				{
					player.TakeDamage(1);
					contactDamageTimer = 0;
					break;
				}
			}

			shootTimer += delta;
			if (shootTimer >= shootCooldown)
			{
				ShootDarkLight(arahKePlayer);
				shootTimer = 0;
			}
		}
		else
		{
			Velocity = Vector2.Zero;
		}
	}

	private void UpdateArahMusuh(Vector2 arah)
	{
		if (animSprite != null)
		{
			if (Mathf.Abs(arah.X) > Mathf.Abs(arah.Y))
			{
				if (arah.X > 0) animSprite.Play("idle_kanan");
				else animSprite.Play("idle_kiri");
			}
			else
			{
				if (arah.Y > 0) animSprite.Play("idle_bawah");
				else animSprite.Play("idle_atas");
			}
		}
		else if (normalSprite != null)
		{
			if (Mathf.Abs(arah.X) > Mathf.Abs(arah.Y))
			{
				if (arah.X > 0 && TextureKanan != null) normalSprite.Texture = TextureKanan;
				else if (arah.X < 0 && TextureKiri != null) normalSprite.Texture = TextureKiri;
			}
			else
			{
				if (arah.Y > 0 && TextureDepan != null) normalSprite.Texture = TextureDepan;
				else if (arah.Y < 0 && TextureBelakang != null) normalSprite.Texture = TextureBelakang;
			}
		}
	}

	private void ShootDarkLight(Vector2 arah)
	{
		if (ProjectileScene == null || targetPlayer == null) return;

		var shot = ProjectileScene.Instantiate<LightProjectile>();
		shot.GlobalPosition = GlobalPosition;
		shot.Direction = arah;
		shot.IsPlayerShot = false;

		GetParent().AddChild(shot);
	}

	public void TakeDamage(int damage)
	{
		if (!isAggro)
		{
			isAggro = true;
			EmitSignal(SignalName.BattleStarted);
		}

		CurrentHp -= damage;
		if (CurrentHp < 0) CurrentHp = 0;

		EmitSignal(SignalName.HpChanged, CurrentHp);

		if (CurrentHp <= 0)
		{
			CurrentHp = 0;
			EmitSignal(SignalName.EnemyDied);
			QueueFree();
		}
	}
}
