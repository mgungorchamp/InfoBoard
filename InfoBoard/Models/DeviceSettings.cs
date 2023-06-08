namespace InfoBoard.Models
{
    public class DeviceSettings
    {
#nullable enable

        public int? id { get; set; }
        public int? user_id { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public string? version { get; set; }
        public string? last_handshake_temporary_code { get; set; }
        public string? last_heard_from { get; set; }
        public string? device_key { get; set; } 
        public ErrorInfo? error { get; set; }

#nullable disable
    }
}
