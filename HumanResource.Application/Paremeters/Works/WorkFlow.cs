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
        public Nullable<DateTime> DateChange { get; set; }
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
        public DateTime dates { get; set; }
        public DateTime datee { get; set; }

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
