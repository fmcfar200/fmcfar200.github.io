using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpartanChallenge.Models;
using System.Web;
using System.Collections;

namespace SpartanChallenge.Controllers
{
    //controller for equipment models
    public class EquipmentController : ApiController
    {
        //collection of the serialised equipment json objects
        public List<SerialisedEquipment> serialisedEquipmentList = 
            new List<SerialisedEquipment>();
        //collection of json equipment types
        public List<EquipmentType> equipmentTypeList =
            new List<EquipmentType>();
        
        //collection of all items to be displayed to client
        public List<ListItem> ListItemCollection = 
            new List<ListItem>();

        //Initial get when page is loaded
        // GET: api/Equipment/5
        public IHttpActionResult Get()
        {
            //try-catch for parsing the json data
            try
            {
               
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/EquipmentData.json"); //filepath of json file
                using (StreamReader reader = new StreamReader(filePath))
                {
                    //string is read and parsed to json object
                    string jsonString = reader.ReadToEnd();
                    JObject parsedString = JObject.Parse(jsonString);

                    //all the child data of the 'SerialisedEquipment' json objects are put into a list of Jtoken objects
                    //each item is taken from the list and mde into .NET objects
                    IList<JToken> serializedResults = parsedString["SerialisedEquipment"].Children().ToList();
                    foreach (JToken item in serializedResults)
                    {
                        SerialisedEquipment equipment = item.ToObject<SerialisedEquipment>();
                        serialisedEquipmentList.Add(equipment); //the equipment item is then stored to the global list
                    }

                    //same process again but for the Equipment type json objects
                    IList<JToken> typeResults = parsedString["EquipmentType"].Children().ToList();
                    foreach (JToken item in typeResults)
                    {
                        EquipmentType equipmentType = item.ToObject<EquipmentType>();
                        equipmentTypeList.Add(equipmentType);
                    }




                    //Each individual equipment type id is checked against the IDs of the EquimentType list
                    
                    foreach (SerialisedEquipment equipment in serialisedEquipmentList)
                    {
                        foreach (EquipmentType type in equipmentTypeList)
                        {
                            if (equipment.EquipmentTypeId == type.Id) 
                            {
                                //if a match is made then a new ListItem is made and their attributes set
                                ListItem item = new ListItem();
                                item.UnitId = equipment.ExternalId;
                                item.ItemId = type.ExternalId;
                                item.Description = type.Description;
                                //item is added to the list item collection
                                ListItemCollection.Add(item);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File was not read successfully: " + e.Message);
            }

            //if list item comes back null then the 404 Ihhtp action result is returned
            if (ListItemCollection == null)
            {
                return NotFound();
            }
            return Ok(ListItemCollection); //list item collection is sent to the client
        }


       

        //Post action from searching the list
        //Takes in a Search object from the client

        // Post: api/Equipment
        public IEnumerable Post([FromBody] Search search)
        {
            List<ListItem> queryResult = new List<ListItem>(); //holds the list of results from the search

            //FIX CODE: Lists from initial GET were returning null or empty so parsing code had to be repeated each query.
            try
            {
                string filePath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/EquipmentData.json");
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string jsonString = reader.ReadToEnd();
                    JObject parsedString = JObject.Parse(jsonString);

                    IList<JToken> serializedResults = parsedString["SerialisedEquipment"].Children().ToList();
                    foreach (JToken item in serializedResults)
                    {
                        SerialisedEquipment equipment = item.ToObject<SerialisedEquipment>();
                        serialisedEquipmentList.Add(equipment);
                    }

                    IList<JToken> typeResults = parsedString["EquipmentType"].Children().ToList();
                    foreach (JToken item in typeResults)
                    {
                        EquipmentType equipmentType = item.ToObject<EquipmentType>();
                        equipmentTypeList.Add(equipmentType);
                    }


                    foreach (SerialisedEquipment equipment in serialisedEquipmentList)
                    {
                        foreach (EquipmentType type in equipmentTypeList)
                        {
                            if (equipment.EquipmentTypeId == type.Id)
                            {
                                ListItem item = new ListItem();
                                item.UnitId = equipment.ExternalId;
                                item.ItemId = type.ExternalId;
                                item.Description = type.Description;
                                ListItemCollection.Add(item);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("File was not read successfully: " + e.Message);
            }


            //the search obect's query term is null checked
            if (search.sTerm != null)
            {
                //try catch for searching
                try
                {
                    //query string and type are parsed to integers
                    int query = Int32.Parse(search.sTerm);
                    int type = Int32.Parse(search.sType);

                    //depending on type (0= search individual units; 1 = search by type)
                    if (type == 0)
                    {
                        //find all items in the list with matching UnitID
                        queryResult = ListItemCollection.FindAll(s => s.UnitId.Equals(query));

                    }
                    else if (type == 1)
                    {
                        //find al items in the list with matching ItemID
                        queryResult = ListItemCollection.FindAll(s => s.ItemId.Equals(query));

                    }

                    //return full list collection if the query results is null or empty
                    if (queryResult.Count == 0 || queryResult == null)
                    {
                        queryResult = ListItemCollection;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not get search: " + e.Message);
                }
            }
            else if (search.sTerm == null)
            {
                queryResult = ListItemCollection;
            }

            return queryResult;

        }

    }
}
