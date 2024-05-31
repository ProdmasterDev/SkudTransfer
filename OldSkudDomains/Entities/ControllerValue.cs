namespace ASPWeb.Models
{
    public class ControllerValue
    {
        public string Controller { get; set; }
        public bool In { get; set; }
        public bool Out { get; set; }
    }

    public class UpdateAccessGroupCommand
    {
        public string AccessGroup { get; set; }
        public List<ControllerValue> ControllerValues { get; set; }
    }
}
