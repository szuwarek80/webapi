namespace Search.WebAPI.Exe.Dto
{
    public class ManagementItemDto :
        BaseItemDto
    {
        public int MgmtID { get; set; }
        public string Name { get; set; }
        public string Market { get; set; }
        public string State { get; set; }
    }
}
