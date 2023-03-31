using System.ComponentModel.DataAnnotations;

namespace Pustok_BackEndProject.ViewModels.CompareViewModel
{
	public class CompareVM
	{
		public int Id { get; set; }
		public string? Title { get; set; }
		public string? Description { get; set; }
		public string? Image { get; set; }
		public double Price { get; set; }
		public bool? InStock { get; set; }
		public string? CategoryName { get; set; }
		
		public int? Review { get; set; }

	}
}
