using Newtonsoft.Json;
using SimpleDataManagement.Models;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

var dataSourcesDirectory = Path.Combine(Environment.CurrentDirectory, "DataSources");
var personsFilePath = Path.Combine(dataSourcesDirectory, "Persons_20220824_00.json");
var organizationsFilePath = Path.Combine(dataSourcesDirectory, "Organizations_20220824_00.json");
var vehiclesFilePath = Path.Combine(dataSourcesDirectory, "Vehicles_20220824_00.json");
var addressesFilePath = Path.Combine(dataSourcesDirectory, "Addresses_20220824_00.json");


//Quick test to ensure that all files are where they should be :)
foreach (var path in new[] { personsFilePath, organizationsFilePath, vehiclesFilePath, addressesFilePath })
{
    if (!File.Exists(path))
        throw new FileNotFoundException(path);
}

//TODO: Start your exercise here. Do not forget about answering Test #9 (Handled slightly different)
// Reminder: Collect the data from each file. Hint: You could use Newtonsoft's JsonConvert or Microsoft's JsonSerializer

// Read in people data
string fileInput = File.ReadAllText(personsFilePath);
var person = JsonConvert.DeserializeObject<List<Person>>(fileInput);

// Read in organizations data
fileInput = File.ReadAllText(organizationsFilePath);
var organizations = JsonConvert.DeserializeObject<List<Organization>>(fileInput);

// Read in vehicles data
fileInput = File.ReadAllText(vehiclesFilePath);
var vehicles = JsonConvert.DeserializeObject<List<Vehicle>>(fileInput);

// Read in address data
fileInput = File.ReadAllText(addressesFilePath);
var address = JsonConvert.DeserializeObject<List<Address>>(fileInput);

//Test #1: Do all files have entities? (True / False)

// check that all 4 lists have at least 1 element

if (person is null) 
{
    person = new List<Person>();
};
if (organizations is null) 
{
    organizations = new List<Organization>();
}
if (vehicles is null) 
{
    vehicles = new List<Vehicle>();
}
if (address is null)
{
    address = new List<Address>();
}

// check that all four lists are populated, empty files have been converted to empty lists
bool noEmptyFiles = person.Any() && organizations.Any() && vehicles.Any() && address.Any();

Console.WriteLine("1. Do all files have entities? Answer: {0}\n", noEmptyFiles);


//Test #2: What is the total count for all entities?

Console.WriteLine("2. What is the total count for all entities? Answer: {0}\n", 
                        person.Count+organizations.Count+vehicles.Count + address.Count);



//Test #3: What is the count for each type of Entity? Person, Organization, Vehicle, and Address

Console.WriteLine("3. What is the count for each type of Entity? Answer: \n");
Console.WriteLine("Person: {0}", person.Count);
Console.WriteLine("Organization: {0}", organizations.Count);
Console.WriteLine("Vehicle: {0}", vehicles.Count);
Console.WriteLine("Address: {0}\n", address.Count);

//Test #4: Provide a breakdown of entities which have associations in the following manor:
//         -Per Entity Count
//         - Total Count

// the aggregation function starts at 0 and sums up the number of associations in each entity list
// this could be simplified (and improved maintenance-wise) with an improvement to the object model
// described in answer 9
int personsAssociationsCount = person.Aggregate(0, (value, entity) => value + entity.Associations.Count);
int addressesAssociationsCount = address.Aggregate(0, (value, entity) => value + entity.Associations.Count);
int vehiclesAssociationsCount = vehicles.Aggregate(0, (value, entity) => value + entity.Associations.Count);
int organizationsAssociationsCount = organizations.Aggregate(0, (value, entity) => value + entity.Associations.Count);


Console.WriteLine("4. Breakdown of entities which have associations:\n");
Console.WriteLine("- Per Entity Count");
Console.WriteLine("Person: {0}", personsAssociationsCount);
Console.WriteLine("Address: {0}", addressesAssociationsCount);
Console.WriteLine("Vehicles: {0}", vehiclesAssociationsCount);
Console.WriteLine("Organizations: {0}", organizationsAssociationsCount);
Console.WriteLine("- Total Count: {0}\n", personsAssociationsCount + addressesAssociationsCount + vehiclesAssociationsCount + organizationsAssociationsCount);



//Test #5: Provide the vehicle detail that is associated to the address "4976 Penelope Via South Franztown, NH 71024"?
//         StreetAddress: "4976 Penelope Via"
//         City: "South Franztown"
//         State: "NH"
//         ZipCode: "71024"

// Looking at the data it appears that vehicles are associated to address in their object model
// Addresses also appear to contain association information to vehicles (potential non-normalization?)
Console.WriteLine("5. Vehicle Details by Associated Address:\n");
string vehicleIdSearch = "";

// looping over address first allows us to only go through address and vehicles once, with less memory allocated
foreach(Address place in address)
{
    if (place.State.Equals("NH") && place.ZipCode.Equals("71024") 
        && place.City.Equals("South Franztown") && place.StreetAddress.Equals("4976 Penelope Via"))
    {
        // This for loop may not be strictly necessary given the current test data
        // But production data may have more association types than what we see here
        // it is also possible we would want to make this a list of entity ids to search
        foreach(Association a in place.Associations) {
            if (a.EntityType.Equals("Vehicle")) 
            {
                vehicleIdSearch = a.EntityId;
                break;
            }
        }
        
        break;
    }
}

bool vehicleFound = false;

// this would be better to include in the Vehicle class definition
static void printVehicleDetails(Vehicle v)
{
    Console.WriteLine("Id: {0}", v.EntityId);
    Console.WriteLine("Make: {0}", v.Make);
    Console.WriteLine("Model: {0}", v.Model);
    Console.WriteLine("Year: {0}", v.Year);
    Console.WriteLine("Plate: {0}", v.PlateNumber);
    Console.WriteLine("State: {0}", v.State);
    Console.WriteLine("Vin: {0}\n", v.Vin);
}

// if we didn't find a vehicle id to search, we shouldn't search through the list
if (!vehicleIdSearch.Equals(""))
{
    foreach (Vehicle vehicle in vehicles)
    {
        // in this case it should be safe to break as vehicle ids should be unique
        if (vehicle.EntityId.Equals(vehicleIdSearch))
        {
            printVehicleDetails(vehicle);
            vehicleFound = true;
            break;
        }
    }
}
// should handle both the case where the address has no associated vehciles, and the case where the associated
// vehicle doesn't exist in the vehicle data list
if (!vehicleFound)
{
    Console.WriteLine("No associated vehicles found for this address \n");
}



//Test #6: What person(s) are associated to the organization "thiel and sons"?

// looking at the data: persons seem to be associated to organizations
// but this information is not reflected back in the organization data
// therefore we'll check through persons first, and store any persons
// with any organization association in a dictionary
// this uses more memory but makes comparing organization ids faster
// we cannot guarantee only 1 thiel and sons because we aren't given an organization id to pull persons for


Dictionary<string, List<Person>> organizationToPersons = new Dictionary<string, List<Person>>();
foreach(Person per in person)
{
    foreach(Association assoc in per.Associations)
    {
        if (assoc.EntityType.Equals("Organization"))
        {
            if (!organizationToPersons.ContainsKey(assoc.EntityId))
            {
                organizationToPersons.Add(assoc.EntityId, new List<Person>());
            }
            organizationToPersons[assoc.EntityId].Add(per);
        }
    }   
}


// again, this would be better in the class definition
static void printPerson(Person p)
{
    Console.WriteLine("Person ID: {0}", p.EntityId);
    Console.WriteLine("Full Name: {0} {1} {2}:", p.FirstName, p.MiddleName, p.LastName);
    Console.WriteLine("Date of Birth: {0}\n", p.DateOfBirth);

}

// we now have a map of orgId => all persons associated to that org
// we need to find the orgs with those ids and check if they are a "thiel and sons"

int numPrinted = 0;
Console.WriteLine("6. Persons associated with thiel and sons:\n");
foreach(Organization org in organizations)
{
    // using the key allows us skip any thiel and sons organizations with no associations
    if( organizationToPersons.ContainsKey(org.EntityId) && org.Name.ToLower().Equals("thiel and sons") )
    {
        foreach(Person personToPrint in organizationToPersons[org.EntityId])
        {
            numPrinted += 1;
            printPerson(personToPrint);
        }
    }
}

if(numPrinted == 0)
{
    Console.WriteLine("None\n");
}


//Test #7: How many people have the same first and middle name?

// I initially interpreted the question as "how many people have the same first/middle name pair as someone else"
// which can be solved using a dictionary
// However, the answer given seems to indicate we actually need the number of people where their first name
// and middle name fields have the same value (when put into the same case)

int sameNames = 0;
foreach(Person per in person)
{
    if(per.FirstName.ToLower() == per.MiddleName.ToLower())
    {
        sameNames += 1;
    }
}



Console.WriteLine("7. Number of People who have the same first and middle names: {0}\n", sameNames);

//Test #8: Provide a breakdown of entities where the EntityId contains "B3" in the following manor:
//         -Total count by type of Entity
//         - Total count of all entities


// for each of the four lists, use find all in the List definition
// to get a list of all elements with B3 in the entityId
// then simply access the count property
// use OrdinalIgnoreCase on the contains, as the answers given
// line up with b3 || B3 not strictly B3, so the ids should therefore be case insensitive
const string B3 = "B3";
int personsB3 = person.FindAll(
    delegate(Person p)
    {
        return p.EntityId.Contains(B3, StringComparison.OrdinalIgnoreCase);
    }
    ).Count;
int orgsB3 = organizations.FindAll(
    delegate (Organization org)
    {
        return org.EntityId.Contains(B3, StringComparison.OrdinalIgnoreCase);
    }
    ).Count;
int vehiclesB3 = vehicles.FindAll(
    delegate (Vehicle vehicle)
    {
        return vehicle.EntityId.Contains(B3, StringComparison.OrdinalIgnoreCase);
    }
    ).Count;
int addressB3 = address.FindAll(
    delegate (Address add)
    {
        return add.EntityId.Contains(B3, StringComparison.OrdinalIgnoreCase);
    }
    ).Count;

Console.WriteLine("8. Entities with B3 in the entity id:\n");
Console.WriteLine("Person: {0}", personsB3);
Console.WriteLine("Vehicle: {0}", vehiclesB3);
Console.WriteLine("Organization: {0}", orgsB3);
Console.WriteLine("Address: {0}", addressB3);
Console.WriteLine("Total: {0}", personsB3 + vehiclesB3 + orgsB3 + addressB3);

/*#9: 
 *
 *
 * The Object Model could be improved by creating an abstract entity class
 * which Address, Organization, Person, and Vehicle inherit.
 * 
 * This abstract class will handle the EntityId, Associations, and _entities fields of the four classes above.
 * This specifically improves the workflow by enabling us to create functions that apply
 * to each class where the specific details of each entity are not required (e.g. static int countAssociations(Entity entity) )
 * This class could also handle a few basic utilities such as printing its own entity id
 * 
 * Perhaps the most useful of these would be an "isAssociated" function that takes another entity and returns
 * whether or not the two entities have a relationship based on the info in either object 
 * I would want to implement it such that a.isAssociated(b) is equivalent to b.isAssociated(a). While I would like to
 * alter association.cs to create a two-way relationship more readily accessible, this is not feasible without changing
 * the structure of the JSON. In order to do this, we would need to receive a list of entity id associations separate
 * from the four entity types, which we cannot alter.
 * 
 * The abstract class would benefit the workflows where a function is applied to each of the four lists as the function
 * could be written once rather than four times. here, I wrote the same code 4 times changing out only the subtype of entity
 * This would lead to more maintainable code as an edit to one entity level function would only need to be
 * done once.
 * 
 * Additionally, I would implement a few utilities in the 4 more specific entities to print their information.
 * This isn't a huge deal, but would allow for more consistent prints in other files using the same data, which
 * leads to more consistent and understandable output for end users or front end development.
 * 
 * 
 * 
 */