using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD2Launcherv2.Models
{
    public class Pd2AuthorList
    {
        public string StorageETag { get; set; }
        public List<FilterAuthor> StorageAuthorList { get; set; }
    }
}
