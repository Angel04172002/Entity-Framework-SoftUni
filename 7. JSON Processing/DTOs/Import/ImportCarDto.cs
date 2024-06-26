﻿using Newtonsoft.Json;

namespace CarDealer.DTOs.Import
{
    public class ImportCarDto
    {
        [JsonProperty("make")]
        public string Make { get; set; } = null!;

        [JsonProperty("model")]
        public string Model { get; set; } = null!;

        [JsonProperty("traveledDistance")]
        public long TraveledDistance { get; set; }

        [JsonProperty("partsId")]
        public List<int> Parts { get; set; } = new List<int>();
    }
}
