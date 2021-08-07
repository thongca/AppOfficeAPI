using HumanResource.Data.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Entities.Works
{
   public class CV_QT_WorkFlow
    {
        public string Id { get; set; }
        public string MyWorkId { get; set; }
        public int UserSendId { get; set; }
        public string SendName { get; set; }
        public int UserDeliverId { get; set; }
        public string DeliverName { get; set; }
        public DateTime SendDate { get; set; }
        public TypeFlowEnum TypeFlow { get; set; }
        public DateTime CreateDate { get; set; }
        public string MaLenh { get; set; }
        public string ParentId { get; set; }
        public string Note { get; set; }
        public string Require { get; set; }
        public string PositionSend { get; set; }
        public string PositionDeli { get; set; }
        public string DepartSend { get; set; }
        public string DepartDeli { get; set; }
        public bool Readed { get; set; }
        public DateTime? ReadDate { get; set; }
        public bool Handled { get; set; }
        public DateTime? HandleDate { get; set; }
        public int Repossibility { get; set; }
        public int CompanyId { get; set; }
    }
}
