using Godot;
using System.Collections.Generic;

public partial class MapHazardManager : Node
{
    public static MapHazardManager Instance;

    [Export] public Godot.Collections.Array<BaseMapHazard> MapHazards;
    public List<BaseMapHazard> ActiveMapHazards = new List<BaseMapHazard>();
    [Export] private RoundManager RoundManager;

    [Export] private HBoxContainer MapHazardUIContainer;
    [Export] private PackedScene MapHazardUICardScene;

    public override void _Ready()
    {
        Instance = this;
    }

    public BaseMapHazard GetRandomHazard()
    {
        var randomIndex = GD.Randi() % MapHazards.Count;
        return MapHazards[(int)randomIndex];
    }

    public void ActivateHazard(BaseMapHazard hazard)
    {
        hazard.ApplyHazard(RoundManager);
        ActiveMapHazards.Add(hazard);
    }

    public void ActivateRandomHazard(int nb = 1)
    {
        List<BaseMapHazard> availableHazards = new List<BaseMapHazard>();

        foreach (var h in MapHazards)
        {
            if (!ActiveMapHazards.Contains(h))
            {
                availableHazards.Add(h);
            }
        }

        int amountToPick = Mathf.Min(nb, availableHazards.Count);

        for (int i = 0; i < amountToPick; i++)
        {
            int randomIndex = (int)(GD.Randi() % (uint)availableHazards.Count);
            BaseMapHazard selected = availableHazards[randomIndex];

            ActivateHazard(selected);

            availableHazards.RemoveAt(randomIndex);
        }

        FillUI();
    }

    public void DeactivateAllHazards()
    {
        foreach (var hazard in ActiveMapHazards)
        {
            hazard.RemoveHazard(RoundManager);
        }
        ActiveMapHazards.Clear();
    }

    private void FillUI()
    {
        if (ActiveMapHazards.Count > 0)
        {
            foreach (Node child in MapHazardUIContainer.GetChildren())
            {
                child.QueueFree();
            }

            foreach (var hazard in ActiveMapHazards)
            {
                var hazardCard = MapHazardUICardScene.Instantiate<MapHazardCard>();
                hazardCard.Setup(hazard);
                MapHazardUIContainer.AddChild(hazardCard);
            }
        }
    }

    public bool HasHazard<T>() where T : BaseMapHazard
    {
        foreach (var hazard in ActiveMapHazards)
        {
            if (hazard is T)
            {
                return true;
            }
        }
        return false;
    }
}
