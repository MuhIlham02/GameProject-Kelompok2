using Godot;

public partial class LightProjectile : Area2D
{
	[Export] public float Speed = 300f;
	[Export] public int Damage = 1;

	public Vector2 Direction = Vector2.Zero;
	public bool IsPlayerShot = true;

	private double lifetime = 3.5;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		QueueRedraw();
	}

	public override void _Draw()
	{
		Color warnaInti;
		Color warnaGlow;

		if (IsPlayerShot)
		{
			warnaInti = new Color(1.0f, 1.0f, 0.8f, 1.0f);
			warnaGlow = new Color(1.0f, 0.85f, 0.2f, 0.35f);
		}
		else
		{
			warnaInti = new Color(0.7f, 0.2f, 1.0f, 1.0f);
			warnaGlow = new Color(0.35f, 0.05f, 0.55f, 0.45f);
		}

		DrawCircle(Vector2.Zero, 5f, warnaGlow);
		DrawCircle(Vector2.Zero, 2.5f, warnaInti);
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * Speed * (float)delta;

		lifetime -= delta;
		if (lifetime <= 0)
		{
			QueueFree();
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (IsPlayerShot && body is ShadowEnemy enemy)
		{
			enemy.TakeDamage(Damage);
			QueueFree();
		}
		else if (!IsPlayerShot && body is BattlePlayer player)
		{
			player.TakeDamage(Damage);
			QueueFree();
		}
	}
}
