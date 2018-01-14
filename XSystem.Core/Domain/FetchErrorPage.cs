using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XSystem.Core.Domain
{
    public class FetchErrorPage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Url { get; set; }
        
        public string Exception { get; set; }

        public static FetchErrorPage Create(string url,Exception exception)
        {
            var stringBuilder = new StringBuilder();
            while (exception!=null) {
                stringBuilder.AppendLine(exception.Message);
                exception = exception.InnerException;
            }
            return new FetchErrorPage {
                Url = url,
                Exception = stringBuilder.ToString()
            };
        }
    }
}