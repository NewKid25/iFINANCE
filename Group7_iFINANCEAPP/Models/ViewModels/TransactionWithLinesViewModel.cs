using System.Collections.Generic;

namespace Group7_iFINANCEAPP.Models.ViewModels
{
	public class TransactionWithLinesViewModel
	{
		public Transaction Transaction { get; set; }
		public List<TransactionLine> TransactionLines { get; set; }
	}
}
