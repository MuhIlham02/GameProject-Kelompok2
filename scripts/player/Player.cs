using Godot;

public partial class Player : CharacterBody2D
{
	private AnimatedSprite2D animasi;

	public override void _Ready()
	{
		animasi = GetNode<AnimatedSprite2D>("Sprite2D");
		CallDeferred(nameof(SetSpawnPosition));
	}

	private void SetSpawnPosition()
	{
		GD.Print($"[PLAYER] Membaca TargetSpawnPoint dari GameManager: '{GameManager.TargetSpawnPoint}'");

		if (!string.IsNullOrEmpty(GameManager.TargetSpawnPoint))
		{
			var spawnPoint = GetParent().GetNodeOrNull<Marker2D>(GameManager.TargetSpawnPoint);
			
			if (spawnPoint != null)
			{
				GlobalPosition = spawnPoint.GlobalPosition;
				GD.Print($"[PLAYER] Berhasil pindah posisi ke: {spawnPoint.Name}");
			}
			else
			{
				GD.PrintErr($"[PLAYER] GAGAL! Marker2D '{GameManager.TargetSpawnPoint}' tidak ditemukan di {GetParent().Name}");
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 arah = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = arah * 150;
		MoveAndSlide();

		if (arah.X > 0) animasi.Play("idle_kanan");
		else if (arah.X < 0) animasi.Play("idle_kiri");
		else if (arah.Y > 0) animasi.Play("idle_bawah");
		else if (arah.Y < 0) animasi.Play("idle_atas");
	}
}
