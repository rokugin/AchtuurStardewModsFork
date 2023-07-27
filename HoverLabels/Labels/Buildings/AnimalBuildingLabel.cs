﻿using AchtuurCore.Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels.Labels.Buildings;
internal class AnimalBuildingLabel : BuildingLabel
{
    public override bool ShouldGenerateLabel(Vector2 cursorTile)
    {
        return ModEntry.IsPlayerOnFarm()
            && GetFarmBuildings().Any(b => 
                b.indoors.Value is not null
                && b.indoors.Value is AnimalHouse
                && b.GetRect().Contains(cursorTile)
            );
    }

    public override void SetCursorTile(Vector2 cursorTile)
    {
        base.SetCursorTile(cursorTile);
    }

    public override void GenerateLabel()
    {
        base.GenerateLabel();
        List<FarmAnimal> buildingAnimals = GetBuildingAnimals(this.hoverBuilding).ToList();

        int pettableAnimals = buildingAnimals.Count(a => !a.wasPet.Value);
        if (pettableAnimals > 0)
            this.Description.Add($"{pettableAnimals} animals left to pet");

        int shownAnimals = 0;
        int labelLimit = ModEntry.GetLabelSizeLimit();

        Dictionary<string, IEnumerable<FarmAnimal>> barnAnimalPerType = GetBuildingAnimalsPerType(this.hoverBuilding);
        foreach((string animalType, IEnumerable<FarmAnimal> animals) in barnAnimalPerType.OrderBy(x => x.Key).Select(x => (x.Key, x.Value)))
        {
            this.Description.Add($"{animalType} ({animals.Count()}):");
            foreach (FarmAnimal animal in animals.OrderBy(a => a.Name))
            {
                string descString = $"> {animal.Name}";

                // Check if animal was pet
                if (animal.wasPet.Value)
                    descString += " (Pet)";
                else if (animal.wasAutoPet.Value)
                    descString += " (Auto-Pet)";

                this.Description.Add(descString);
            }

            shownAnimals += animals.Count();
            if (labelLimit - shownAnimals <= 0)
                break;
        }

        if (!ModEntry.IsShowDetailButtonPressed() && buildingAnimals.Count > shownAnimals)
            this.Description.Add(I18n.LabelPressShowmore(ModEntry.GetShowDetailButtonName(), buildingAnimals.Count - shownAnimals));

    }

    protected static IEnumerable<FarmAnimal> GetBuildingAnimals(Building building)
    {
        if (building.indoors.Value is null || building.indoors.Value is not AnimalHouse animalHouse)
            return null;


        return Game1.getFarm().Animals.Values.Where(a => a.home == building)
            .Concat(animalHouse.animals.Values);
    }

    protected static Dictionary<string, IEnumerable<FarmAnimal>> GetBuildingAnimalsPerType(Building building)
    {
        if (building.indoors.Value is null || building.indoors.Value is not AnimalHouse)
            return null;

        Dictionary<string, IEnumerable<FarmAnimal>> indoorsAnimalPerType = new();

        // Get animals on farm that belong to this building
        IEnumerable<FarmAnimal> buildingAnimals = GetBuildingAnimals(building);

        // Get unique animal types of this building's animals
        IEnumerable<string> IndoorAnimalTypes = buildingAnimals.Select(a => a.type.Value).Distinct();

        foreach (string animalType in IndoorAnimalTypes)
        {
            // get animals with type animalType
            IEnumerable<FarmAnimal> animals = buildingAnimals.Where(a => a.type.Value == animalType);

            // dont have to check for duplicate keys since that should be impossible as IndoorAnimalTypes contains only distinct items
            indoorsAnimalPerType.Add(animalType, animals);
        }

        return indoorsAnimalPerType;
    }


}
