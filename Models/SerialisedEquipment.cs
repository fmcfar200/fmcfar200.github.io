using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpartanChallenge.Models
{
    public class SerialisedEquipment
    {
        public string Id { get; set; }
        public int ExternalId { get; set; }
        public string EquipmentTypeId { get; set; }
        public int MeterReading { get; set; }
    }
}