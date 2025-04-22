using Group7_iFINANCEAPP.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Group7_iFINANCEAPP.Models.ViewModels
{
    public class TrialBalanceViewModel
    {
        public TrialBalanceViewModel(List<TrailBalanceLineItem> lineItems, double? totalDebit, double? totalCredit)
        {
            this.lineItems = lineItems;
            this.totalDebit = (double)totalDebit;
            this.totalCredit = (double)totalCredit;
        }

        public List<TrailBalanceLineItem> lineItems { get; set; }
        public double totalDebit { get; set; }
        public double totalCredit { get; set; }
    }
}