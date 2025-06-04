namespace ClientFounder.Models.Dtos
{
    public class ClientUpdateDto
    {
        public long INN { get; set; }
        public string Name { get; set; }
        public ClientType Type { get; set; }
    }
}
