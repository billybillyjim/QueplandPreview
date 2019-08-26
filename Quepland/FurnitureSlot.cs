using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class FurnitureSlot
{
    public string Name { get; set; }
    public int ID { get; set; }
    public string Description { get; set; }
    public int[] AvailableFurnitureIDs { get; set; }
    public Furniture FurnitureToBuild { get; set; }

}

