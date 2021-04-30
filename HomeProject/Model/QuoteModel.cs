using System.Collections.Generic;

namespace HomeProject.Model
{
    public class QuoteModel
    {
        public AddressModel Source { get; set; }
        public AddressModel Destination { get; set; }
        public IList<Dimension> Packages { get; set; }
    }
    
    /// <summary>
    /// Class to return xml model I kept the name as XML because we need XML as root element
    /// </summary>
    public class XML
    {
        public float Quote { get; set; }
    }
}
