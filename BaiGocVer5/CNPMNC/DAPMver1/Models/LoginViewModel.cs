using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [DataType(DataType.Password)]
    public required string MatKhau { get; set; }
}
