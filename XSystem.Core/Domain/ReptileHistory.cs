using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XSystem.Core.Domain
{
    public class ReptileHistory
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public DateTime DateTime { get; set; }

        public virtual List<FetchErrorPage> ErrorPages { get; set; }

        public ReptileHistory()
        {
            ErrorPages = new List<FetchErrorPage>();
        }
    }
}