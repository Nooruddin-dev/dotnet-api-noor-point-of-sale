using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.ModuleSpecificModels.Common.RequestForms
{
    public class NotificationReadRequestForm
    {
        public int NotificationId { get; set; }

        [JsonIgnore]
        [NotMapped]
        public int BusnPartnerId { get; set; }
    }
}
