namespace AttendanceAPI_v3.AttendanceModels
{
    public class AttendDTO
    {
        public string StudentUuid { get; set; } = null!;

        public uint ClassId { get; set; }
    }
}
