using System.Collections.Generic;

namespace ASC.Web.Areas.Configuration.Models
{
    public class MasterValuesViewModel
    {
        public List<MasterDataValueViewModel> MasterDataValues { get; set; }
        public MasterDataValueViewModel MasterDataValueInContext { get; set; }
        public bool IsEdit { get; set; }
    }
}