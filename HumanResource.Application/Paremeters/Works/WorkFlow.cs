using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Application.Paremeters.Works
{
   public class WorkFlow
    {
        public string Id { get; set; }
        public string MyWorkId { get; set; }
    }
    public class WorkFlowOverTime
    {
        public string Id { get; set; }
        public double WorkTime { get; set; }
        public string MyWorkId { get; set; }
        public double DateOverTime { get; set; }
    }
    public class OptionsCv
    {
        public string Id { get; set; }
        public string MyWorkId { get; set; }
        public int p { get; set; }
        public int pz { get; set; }
    }
    public class FlowModel
    {
        public string Id { get; set; }
        public string MyWorkId { get; set; }
        public string Note { get; set; }
        public string Require { get; set; }
        public int? UserManagerId { get; set; }
        public int? UserNextId { get; set; }
        public double? DEnd { get; set; }
        public double? DStart { get; set; }
        public List<UserDeliver> UserDelivers { get; set; }
        public List<TypeUserDeli> TypeUserDelis { get; set; }
        public List<Error> Errors { get; set; }
    }
    public class UserDeliver
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
    }
    public class TypeUserDeli
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Report_TotalTimePara
    {
        public int UserId { get; set; }
        public double? dates { get; set; }
        public double? datee { get; set; }
        public double? TotalHour { get; set; }
        public double? TotalHourLk { get; set; }

    }
    public class Error
    {
        public int Id { get; set; }
        public string ErrorName { get; set; }
        public double? Point { get; set; }
    }
    public class TypeFlow_P
    {
        public int TypeFlow { get; set; }
    }
}
