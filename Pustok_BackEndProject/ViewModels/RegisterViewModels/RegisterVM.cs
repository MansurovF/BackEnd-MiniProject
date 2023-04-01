using System.ComponentModel.DataAnnotations;

namespace Pustok_BackEndProject.ViewModels.RegisterViewModels
{
	public class RegisterVM
	{

		[StringLength(255)]
		public string Name { get; set; }
		[StringLength(255)]
		public string Surname { get; set; }
		[EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[DataType(DataType.Password)]
		public string RepeatPassword { get; set; }
	}
}
