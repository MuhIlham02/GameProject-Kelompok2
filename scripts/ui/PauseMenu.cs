using Godot;

public partial class PauseMenu : CanvasLayer
{
	private Button _btnResume;
	private Button _btnSettings;
	private Button _btnQuit;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		_btnResume = GetNode<Button>("VBoxContainer/BtnResume");
		_btnSettings = GetNode<Button>("VBoxContainer/BtnSettings");
		_btnQuit = GetNode<Button>("VBoxContainer/BtnQuit");

		_btnResume.Pressed += OnResumePressed;
		_btnSettings.Pressed += OnSettingsPressed;
		_btnQuit.Pressed += OnQuitPressed;

		Hide();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("pause") || Input.IsActionJustPressed("ui_cancel"))
		{
			GetViewport().SetInputAsHandled();
			TogglePause();
		}
	}

	private void TogglePause()
	{
		bool isPaused = !GetTree().Paused;
		GetTree().Paused = isPaused;

		if (isPaused)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	private void OnResumePressed()
	{
		GetTree().Paused = false;
		Hide();
	}

	private void OnSettingsPressed()
	{
		GD.Print("Tombol Settings diklik!");
	}

	private void OnQuitPressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}
