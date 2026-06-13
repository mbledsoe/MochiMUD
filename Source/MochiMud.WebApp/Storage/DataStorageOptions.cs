using System.ComponentModel.DataAnnotations;

namespace MochiMud.WebApp.Storage
{
    public class DataStorageOptions
    {
        public const string SectionName = "DataStorage";

        [Required(AllowEmptyStrings = false)]
        public string BaseDataDirectory { get; set; } = string.Empty;
    }
}
