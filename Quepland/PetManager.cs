using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public class PetManager
{
    private List<Pet> Pets = new List<Pet>();

    public async Task LoadPets(HttpClient Http)
    {
        Pet[] PetArray = await Http.GetJsonAsync<Pet[]>("data/pets.json");
        Pets.AddRange(PetArray);
        foreach(Pet p in Pets)
        {
            Skill[] skillArray = await Http.GetJsonAsync<Skill[]>("data/skills.json");
            p.skills = skillArray.ToList();    
        }
    }
    public List<Pet> GetPets()
    {
        return Pets;
    }
    public string GetPetDescription(string petName)
    {
        if(Pets.Find(x => x.Name == petName) != null)
        {
            return Pets.Find(x => x.Name == petName).Description;
        }
        else
        {
            return "";
        }
    }
}
