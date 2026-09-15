namespace Backend.DTO.Rabbitmq;

public class RabbitMqOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 5671;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VirtualHost { get; set; } = "/";
    public string Exchange { get; set; } = "helpdesk.events";
}