using System.Collections.Generic;

namespace HomeProject.Model
{
    public class WarehouseModel
    {
        public AddressModel ContactAddress { get; set; }
        public AddressModel WarehouseAddress { get; set; }
        public IList<DimensionModel> PackageDimensions { get; set; }
    }
}
