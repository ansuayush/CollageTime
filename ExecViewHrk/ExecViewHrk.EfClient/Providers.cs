using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecViewHrk.EfClient
{
    public class Providers
    {
        [Key]
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string Gateway { get; set; }
    }
}
