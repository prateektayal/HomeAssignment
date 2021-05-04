using System.Collections.Generic;

namespace HomeProject.Model
{
    public class CarrierQuoteModel
    {
        public string Name { get; set; }
        public float Amount { get; set; }
    }

    public class CarrierBestDealModel
    {
        public IList<CarrierQuoteModel> Carriers { get; set; }
        public CarrierQuoteModel BestDeal { get; set; }
    }
}
