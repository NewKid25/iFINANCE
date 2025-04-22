using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Group7_iFINANCEAPP.Models.ViewModels
{
    public class ProfitLossViewModel
    {
        public ProfitLossViewModel(List<MasterAccount> incomeAccounts, List<MasterAccount> expenseAccounts, double totalIncome, double totalExpense)
        {
            this.incomeAccounts = incomeAccounts;
            this.expenseAccounts = expenseAccounts;
            this.totalIncome = totalIncome;
            this.totalExpense = totalExpense;
        }

        public List<MasterAccount> incomeAccounts;
        public List<MasterAccount> expenseAccounts;

        public double totalIncome;
        public double totalExpense;

    }
}