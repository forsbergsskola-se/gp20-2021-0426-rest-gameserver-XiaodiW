using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MMORPGClient.APIs {

    public class Item {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public string Type{ get; set; }
        
        [Range(0,99)]
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        
        public DateTime CreationTime { get; set; }

        [JsonIgnore]
        public Player Player { get; set; }
    }

}