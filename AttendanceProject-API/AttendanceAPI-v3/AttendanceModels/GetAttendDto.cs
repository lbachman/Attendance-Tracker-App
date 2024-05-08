namespace AttendanceAPI_v3.AttendanceModels
{
    public class GetAttendDto
    {
        public string StudentUuid { get; set; } = null!;

        public uint ClassId { get; set; }

        public DateTime AttendanceDate { get; set; }
    }
}
