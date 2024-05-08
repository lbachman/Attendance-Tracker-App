namespace AttendanceAPI_v3.AttendanceModels
{
    /// <summary>
    /// this dto is for the instructor post in the desktop app. Allows to post attendance by username rather than uuid. 
    /// </summary>
    public class InstructorAttendDto
    {
        public string StudentUserName { get; set; } = null!;

        public uint ClassId { get; set; }

        public DateTime AttendanceDateTime { get; set; }

    }
}
