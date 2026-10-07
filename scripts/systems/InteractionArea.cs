using Godot;

public partial class InteractionArea : Area2D
{
	[Export] public string TargetScenePath = "";
	[Export] public string TargetSpawnPoint = "";

	private bool _isPlayerNear = false;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Player") || body.Name == "Player")
		{
			_isPlayerNear = true;
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body.IsInGroup("Player") || body.Name == "Player")
		{
			_isPlayerNear = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (_isPlayerNear && @event.IsActionPressed("Interact"))
		{
			if (!string.IsNullOrEmpty(TargetScenePath))
			{
				GameManager.TargetSpawnPoint = TargetSpawnPoint;
				
				// Tampilkan pesan cek ke panel Output
				GD.Print($"[PINTU] Mengirim TargetSpawnPoint: '{TargetSpawnPoint}' ke GameManager.");
				
				GetTree().ChangeSceneToFile(TargetScenePath);
			}
		}
	}
}
