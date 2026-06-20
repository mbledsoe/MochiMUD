using System.ComponentModel.DataAnnotations;

namespace MochiMud.WebApp.Authentication
{
    public class RegistrationOptions
    {
        public const string SectionName = "Registration";

        [Required(AllowEmptyStrings = false)]
        public string InviteCode { get; set; } = string.Empty;
    }
}
