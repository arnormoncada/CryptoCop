using System.Collections.Generic;

namespace Cryptocop.Software.API.Models
{
    public class Envelope<T> where T : class
    {
        public Envelope(int pageNumber, IEnumerable<T> items)
        {
            PageNumber = pageNumber;
            Items = items;
        }
        public int PageNumber { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
