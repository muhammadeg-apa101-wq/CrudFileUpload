using System.ComponentModel.DataAnnotations;

namespace EntityFrameworkProject.Areas.Admin.ViewModels.CategoryVMs
{
    public class UpdateCategoryVM
    {
        [Required(ErrorMessage ="Bos ola bilmez!")]
        public string Name { get; set; }
    }
}
