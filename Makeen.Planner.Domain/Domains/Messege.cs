using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domains
{
    public class Messege
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public Guid Sender { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public Messege()
        {
            Id = Guid.NewGuid();
        }

    }
}
