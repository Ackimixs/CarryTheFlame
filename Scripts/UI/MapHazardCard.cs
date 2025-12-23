using Godot;
using System;

public partial class MapHazardCard : NinePatchRect
{
    [Export] private Label hazardTitleLabel;
    [Export] private Label hazardDescriptionLabel;

    public void Setup(BaseMapHazard mapHazard)
    {
        hazardTitleLabel.Text = mapHazard.DisplayName;
        hazardDescriptionLabel.Text = mapHazard.Description;
    }
}
