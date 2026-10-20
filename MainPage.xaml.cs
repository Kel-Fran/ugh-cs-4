namespace ugh_cs_4;

public partial class MainPage : ContentPage
{
	StreamingPlatform platform;

	public MainPage()
	{
		InitializeComponent();
		platform = new([
			new Movie(new Video("A", 10)),
			new Movie(new Video("B", 20)),
		]);
		LV_Content.ItemsSource = platform;
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		LV_Content.SelectedItem = platform.Longest;
	}
}
