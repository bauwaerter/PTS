using System;
using Core.Domains;

namespace PTS.Models {
    public class TeacherScheduleModel {
        public TeacherUser Teacher { get; set; }
        public Schedule Schedule { get; set; }
    }

}