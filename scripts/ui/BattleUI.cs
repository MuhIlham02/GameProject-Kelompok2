using Godot;

public partial class BattleUI : Control
{
	private Panel gameOverPanel;
	private Panel winPanel;
	private TextureProgressBar _playerHpBar;
	private TextureProgressBar _enemyHpBar;
	private bool _isBattleOver = false;

	public override void _Ready()
	{
		gameOverPanel = GetNodeOrNull<Panel>("GameOverPanel");
		winPanel = GetNodeOrNull<Panel>("WinPanel");
		_playerHpBar = GetNodeOrNull<TextureProgressBar>("PlayerHpBar");
		_enemyHpBar = GetNodeOrNull<TextureProgressBar>("EnemyHpBar");

		if (_playerHpBar != null)
		{
			_playerHpBar.Visible = false;
			_playerHpBar.MinValue = 0;
			_playerHpBar.MaxValue = 6;
			_playerHpBar.Step = 1;
			_playerHpBar.Value = 6;
		}

		if (_enemyHpBar != null)
		{
			_enemyHpBar.Visible = false;
			_enemyHpBar.MinValue = 0;
			_enemyHpBar.MaxValue = 6;
			_enemyHpBar.Step = 1;
			_enemyHpBar.Value = 6;
		}

		if (gameOverPanel != null)
		{
			gameOverPanel.Visible = false;
			var btnRestart = gameOverPanel.GetNodeOrNull<Button>("BtnRestart");
			if (btnRestart != null)
			{
				btnRestart.Pressed += () => GetTree().ReloadCurrentScene();
			}
		}

		if (winPanel != null)
		{
			winPanel.Visible = false;
			var btnWinRestart = winPanel.GetNodeOrNull<Button>("BtnRestart") 
								?? winPanel.FindChild("*Button*", true, false) as Button;
			if (btnWinRestart != null)
			{
				btnWinRestart.Pressed += () => GetTree().ReloadCurrentScene();
			}
		}

		var player = GetTree().GetFirstNodeInGroup("Player") as BattlePlayer
					 ?? GetTree().Root.FindChild("Player", true, false) as BattlePlayer;

		var enemy = GetTree().GetFirstNodeInGroup("Enemy") as ShadowEnemy
					?? GetTree().Root.FindChild("ShadowEnemy", true, false) as ShadowEnemy;

		if (player != null)
		{
			player.CanShoot = false;

			if (_playerHpBar != null)
			{
				_playerHpBar.MaxValue = player.MaxHp;
				_playerHpBar.Value = player.CurrentHp;
			}

			player.HpChanged += (hp) =>
			{
				if (_playerHpBar != null) _playerHpBar.Value = hp;
			};

			player.PlayerDied += () =>
			{
				if (_isBattleOver) return;
				_isBattleOver = true;

				HideAllHpBars();
				if (gameOverPanel != null) gameOverPanel.Visible = true;
			};
		}

		if (enemy != null)
		{
			if (_enemyHpBar != null)
			{
				_enemyHpBar.MaxValue = enemy.MaxHp;
				_enemyHpBar.Value = enemy.CurrentHp;
			}

			enemy.BattleStarted += () =>
			{
				if (_playerHpBar != null) _playerHpBar.Visible = true;
				if (_enemyHpBar != null) _enemyHpBar.Visible = true;
				if (player != null) player.CanShoot = true;
			};

			enemy.HpChanged += (hp) =>
			{
				if (_enemyHpBar != null) _enemyHpBar.Value = hp;
			};

			enemy.EnemyDied += () =>
			{
				if (_isBattleOver) return;
				_isBattleOver = true;

				HideAllHpBars();
				if (winPanel != null) winPanel.Visible = true;
			};
		}
	}

	private void HideAllHpBars()
	{
		if (_playerHpBar != null) _playerHpBar.Visible = false;
		if (_enemyHpBar != null) _enemyHpBar.Visible = false;
	}
}
