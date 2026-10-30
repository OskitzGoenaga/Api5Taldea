namespace WebApi.DTOs
{
    public class ErreserbaCreateDto
    {
        public int PistaId { get; set; }
        public int BazkideaId { get; set; }
        public string Data { get; set; } = string.Empty; 
        public string Ordua { get; set; } = string.Empty;
    }
}