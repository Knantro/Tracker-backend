using System.ComponentModel.DataAnnotations;

namespace Tracker.Models {
    public class LoginModel {
        [Required(ErrorMessage = "Не указан Email или телефон")]
        public string EmailOrPhoneNumber { get; set; }
         
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}