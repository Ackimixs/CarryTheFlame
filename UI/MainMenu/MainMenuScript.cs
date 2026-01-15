using Godot;

public partial class MainMenuScript : Control
{
	[Export] public Button PlayButton;
	[Export] public Button QuitButton;
	[Export] public PackedScene GameScene;

	public override void _Ready()
	{
		PlayButton.Pressed += OnPlayButtonPressed;
		QuitButton.Pressed += OnQuitButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		GetTree().ChangeSceneToPacked(GameScene);
	}

	private void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept"))
		{
			OnPlayButtonPressed();
		}
		else if (@event.IsActionPressed("ui_cancel"))
		{
			OnQuitButtonPressed();
		}
	}
}
