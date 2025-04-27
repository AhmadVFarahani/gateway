namespace Gateway.Application.Services.Dtos
{
    public abstract class BaseDto
    {
        public long Id{ get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
    }
}
