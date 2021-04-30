using System.Collections.Generic;

namespace HomeProject.Model
{
    public class ConsignmentModel
    {
        public AddressModel Consignee { get; set; }
        public AddressModel Consignor { get; set; }
        public IList<Dimension> Cartons { get; set; }
    }
}
