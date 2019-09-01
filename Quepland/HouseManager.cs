using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

public class HouseManager
{
    public List<Furniture> furniture = new List<Furniture>();
    public List<FurnitureSlot> furnitureSlots = new List<FurnitureSlot>();
    public List<FurnitureSlot> emptySlots = new List<FurnitureSlot>();
    public async Task LoadFurnitureAndSlots(HttpClient Http)
    {
        Furniture[] furnitureArray = await Http.GetJsonAsync<Furniture[]>("data/furniture.json");
        furniture.AddRange(furnitureArray);
        FurnitureSlot[] slotsArray = await Http.GetJsonAsync<FurnitureSlot[]>("data/furnitureSlots.json");
        furnitureSlots.AddRange(slotsArray);
    }

    public List<Furniture> GetAvailableFurnitureForSlot(FurnitureSlot slot)
    {
        List<Furniture> availableFurniture = new List<Furniture>();
        foreach(int id in slot.AvailableFurnitureIDs)
        {
            availableFurniture.Add(furniture[id]);
        }
        return availableFurniture;
    }
}

