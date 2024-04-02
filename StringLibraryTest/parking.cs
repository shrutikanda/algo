// /**
// * This reference program is provided by @jiuzhang.com
// * Copyright is reserved. Please indicate the source for forwarding
// */

// // enum type for Vehicle
// enum VehicleSize
// {
//     Motorcycle,
//     Compact,
//     Large,
// }

// //abstract Vehicle class
// abstract class Vehicle
// {
//     // Write your code here
//     protected internal int spotsNeeded;
//     protected VehicleSize size;

//     protected List<ParkingSpot> parkingSpots = new List<ParkingSpot>(); // id for parking where may occupy multi

//     public int getSpotsNeeded()
//     {
//         return spotsNeeded;
//     }

//     public VehicleSize getSize()
//     {
//         return size;
//     }

//     /* Park vehicle in this spot (among others, potentially) */
//     public void parkInSpot(ParkingSpot spot)
//     {
//         parkingSpots.Add(spot);
//     }

//     /* Remove car from spot, and notify spot that it's gone */
//     public void clearSpots()
//     {
//         foreach (var parkingSpot in parkingSpots)
//         {
//             parkingSpot.removeVehicle();
//         }
//         //parkingSpots.RemoveAll();
//     }
//     //this need to be implement in subclass
//     public abstract bool canFitInSpot(ParkingSpot spot);
// }

// class Motorcycle : Vehicle
// {
//     // Write your code here
//     public Motorcycle()
//     {
//         spotsNeeded = 1;
//         size = VehicleSize.Motorcycle;
//     }


//     public override bool canFitInSpot(ParkingSpot spot)
//     {
//         return true;
//     }


// }

// class Car : Vehicle
// {
//     // Write your code here
//     public Car()
//     {
//         spotsNeeded = 1;
//         size = VehicleSize.Compact;
//     }

//     public override bool canFitInSpot(ParkingSpot spot)
//     {
//         return spot.getSize() == VehicleSize.Large || spot.getSize() == VehicleSize.Compact;
//     }


// }

// class Bus : Vehicle
// {
//     // Write your code here
//     public Bus()
//     {
//         spotsNeeded = 5;
//         size = VehicleSize.Large;
//     }

//     public override bool canFitInSpot(ParkingSpot spot)
//     {
//         return spot.getSize() == VehicleSize.Large;
//     }


// }

// class ParkingSpot
// {
//     // Write your code here
//     private Vehicle vehicle;
//     private VehicleSize spotSize;
//     private int row;
//     private int spotNumber;
//     private Level level;

//     public ParkingSpot(Level lvl, int r, int n, VehicleSize sz)
//     {
//         level = lvl;
//         row = r;
//         spotNumber = n;
//         spotSize = sz;
//     }

//     public bool isAvailable()
//     {
//         return vehicle == null;
//     }
//     /* Checks if the spot is big enough for the vehicle (and is available). This compares
//      * the SIZE only. It does not check if it has enough spots. */
//     public bool canFitVehicle(Vehicle vehicle)
//     {
//         return isAvailable() && vehicle.canFitInSpot(this);
//     }
//     /* Park vehicle in this spot. */
//     public bool park(Vehicle v)
//     {
//         if (!canFitVehicle(v))
//         {
//             return false;
//         }
//         vehicle = v;
//         vehicle.parkInSpot(this);
//         return true;
//     }
//     /* Remove vehicle from spot, and notify level that a new spot is available */
//     public void removeVehicle()
//     {
//         level.spotFreed();
//         vehicle = null;
//     }

//     public int getRow()
//     {
//         return row;
//     }

//     public int getSpotNumber()
//     {
//         return spotNumber;
//     }

//     public VehicleSize getSize()
//     {
//         return spotSize;
//     }

//     public void print()
//     {
//         if (vehicle == null)
//         {
//             if (spotSize == VehicleSize.Compact)
//             {
//                 //  System.out.print("c");  
//             }
//             else if (spotSize == VehicleSize.Large)
//             {
//                 // System.out.print("l");  
//             }
//             else if (spotSize == VehicleSize.Motorcycle)
//             {
//                 // System.out.print("m");  
//             }
//         }
//         else
//         {
//             // vehicle.print();  
//         }
//     }
// }

// /* Represents a level in a parking garage */
// class Level
// {
//     // Write your code here
//     private int floor;
//     private ParkingSpot[] spots;
//     private int availableSpotss = 0; // number of free spots
//     private int SPOTS_PER_ROW;


//     public Level(int flr, int num_rows, int spots_per_row)
//     {
//         floor = flr;
//         int SPOTS_PER_ROW = spots_per_row;
//         int numberSpots = 0;
//         spots = new ParkingSpot[num_rows * spots_per_row];

//         //init size for each spot in array spots
//         for (int row = 0; row < num_rows; ++row)
//         {
//             for (int spot = 0; spot < spots_per_row / 4; ++spot)
//             {
//                 VehicleSize sz = VehicleSize.Motorcycle;
//                 spots[numberSpots] = new ParkingSpot(this, row, numberSpots, sz);
//                 numberSpots++;
//             }
//             for (int spot = spots_per_row / 4; spot < spots_per_row / 4 * 3; ++spot)
//             {
//                 VehicleSize sz = VehicleSize.Compact;
//                 spots[numberSpots] = new ParkingSpot(this, row, numberSpots, sz);
//                 numberSpots++;
//             }
//             for (int spot = spots_per_row / 4 * 3; spot < spots_per_row; ++spot)
//             {
//                 VehicleSize sz = VehicleSize.Large;
//                 spots[numberSpots] = new ParkingSpot(this, row, numberSpots, sz);
//                 numberSpots++;
//             }
//         }

//         availableSpotss = numberSpots;
//     }

//     /* Try to find a place to park this vehicle. Return false if failed. */
//     public bool parkVehicle(Vehicle vehicle)
//     {
//         if (availableSpots() < vehicle.getSpotsNeeded())
//         {
//             return false; // no enough spots
//         }
//         int spotNumber = findAvailableSpots(vehicle);
//         if (spotNumber < 0)
//         {
//             return false;
//         }
//         return parkStartingAtSpot(spotNumber, vehicle);
//     }

//     /* find a spot to park this vehicle. Return index of spot, or -1 on failure. */
//     private int findAvailableSpots(Vehicle vehicle)
//     {
//         int spotsNeeded = vehicle.getSpotsNeeded();
//         int lastRow = -1;
//         int spotsFound = 0;

//         for (int i = 0; i < spots.Length; i++)
//         {
//             ParkingSpot spot = spots[i];
//             if (lastRow != spot.getRow())
//             {
//                 spotsFound = 0;
//                 lastRow = spot.getRow();
//             }
//             if (spot.canFitVehicle(vehicle))
//             {
//                 spotsFound++;
//             }
//             else
//             {
//                 spotsFound = 0;
//             }
//             if (spotsFound == spotsNeeded)
//             {
//                 return i - (spotsNeeded - 1); // index of spot
//             }
//         }
//         return -1;
//     }

//     /* Park a vehicle starting at the spot spotNumber, and continuing until vehicle.spotsNeeded. */
//     private bool parkStartingAtSpot(int spotNumber, Vehicle vehicle)
//     {
//         vehicle.clearSpots();

//         bool success = true;

//         for (int i = spotNumber; i < spotNumber + vehicle.spotsNeeded; i++)
//         {
//             success &= spots[i].park(vehicle);
//         }

//         availableSpotss -= vehicle.spotsNeeded;
//         return success;
//     }

//     /* When a car was removed from the spot, increment availableSpots */
//     public void spotFreed()
//     {
//         availableSpotss++;
//     }

//     public int availableSpots()
//     {
//         return availableSpotss;
//     }

//     public void print()
//     {
//         int lastRow = -1;
//         for (int i = 0; i < spots.Length; i++)
//         {
//             ParkingSpot spot = spots[i];
//             if (spot.getRow() != lastRow)
//             {
//                 // System.out.print("  ");  
//                 lastRow = spot.getRow();
//             }
//             spot.print();
//         }
//     }
// }

// class ParkingLot
// {
//     private Level[] levels;
//     private int NUM_LEVELS;

//     // @param n number of leves
//     // @param num_rows  each level has num_rows rows of spots
//     // @param spots_per_row each row has spots_per_row spots
//     public ParkingLot(int n, int num_rows, int spots_per_row)
//     {
//         // Write your code here
//         NUM_LEVELS = n;
//         levels = new Level[NUM_LEVELS];
//         for (int i = 0; i < NUM_LEVELS; i++)
//         {
//             levels[i] = new Level(i, num_rows, spots_per_row);
//         }
//     }

//     // Park the vehicle in a spot (or multiple spots)
//     // Return false if failed
//     public bool parkVehicle(Vehicle vehicle)
//     {
//         // Write your code here
//         for (int i = 0; i < levels.Length; i++)
//         {
//             if (levels[i].parkVehicle(vehicle))
//             {
//                 return true;
//             }
//         }
//         return false;
//     }

//     // unPark the vehicle
//     public void unParkVehicle(Vehicle vehicle)
//     {
//         // Write your code here
//         vehicle.clearSpots();
//     }

//     public void print()
//     {
//         for (int i = 0; i < levels.Length; i++)
//         {
//             // System.out.print("Level" + i + ": ");  
//             levels[i].print();
//             //  System.out.println("");  
//         }
//         // System.out.println("");  
//     }
// }