using Godot;

public partial class MainMenu : Control
{
	private Button _btnPlay;
	private Button _btnQuit;

	public override void _Ready()
	{
		_btnPlay = GetNode<Button>("BtnPlay");
		_btnQuit = GetNode<Button>("BtnQuit");

		_btnPlay.Pressed += OnPlayPressed;
		_btnQuit.Pressed += OnQuitPressed;
	}

	private void OnPlayPressed()
	{
		GD.Print("Tombol PLAY diklik!");
		GetTree().ChangeSceneToFile("res://scenes/Bedroom.tscn");
	}

	private void OnQuitPressed()
	{
		GD.Print("Tombol QUIT diklik!");
		GetTree().Quit();
	}
}
