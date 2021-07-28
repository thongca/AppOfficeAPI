using HumanResource.Application.Helper;
using HumanResource.Application.Paremeters.Works;
using HumanResource.Data.EF;
using HumanResource.Data.Entities.Works;
using HumanResource.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResoureAPI.Common.WorksCommon
{
    public static class WorksCommon
    {
        public static bool checkDefaultWorksFlow(humanDbContext dbContext , string WorkName, int DepartmentId)
        {
            var task = dbContext.CV_DM_DefaultTask.FirstOrDefault(x => x.Name == WorkName);
            if (task == null)
            {
                CV_DM_DefaultTask obj = new CV_DM_DefaultTask();
                obj.Name = WorkName;
                obj.Id = Helper.GenKey();
                obj.GroupTaskId = 10;
                obj.Frequency = 0;

            }
            return false;
        }
        public static async Task saveDateChangeMyWork(humanDbContext dbContext, string MyWorkId, DateTime DateEnd, DateTime DateStart, int UserId)
        {
            CV_QT_MyWorkChangeDate myWorkChangeDate = new CV_QT_MyWorkChangeDate()
            {
                MyWorkId = MyWorkId,
                EndDate = DateEnd,
                StartDate= DateStart,
                CreatedBy = UserId
            };
            dbContext.CV_QT_MyWorkChangeDate.Add(myWorkChangeDate);
            await dbContext.SaveChangesAsync();
        }
        public static bool PauseMyWork(humanDbContext _context, CV_QT_MyWork myWork, CV_QT_WorkNote workNote)
        {

            if (myWork.CycleWork == 1)
            {
                if (myWork.StartDate.Value.Hour < 17 || (myWork.StartDate.Value.Hour == 17 && myWork.StartDate.Value.Minute < 5))
                {
                    myWork.WorkTime = myWork.WorkTime + (SpaceTimeOnDay.CalSpaceTimeOnDay(myWork.StartDate.Value, DateTime.Now) / 60);
                }
                   
                myWork.CycleWork = 2;
                CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                his.MyWorkId = myWork.Id;
                his.CreateDate = DateTime.Now;
                his.CycleWork = 2;
                his.UserCreateId = myWork.UserTaskId;
                _context.CV_QT_StartPauseHistory.Add(his);
                if (workNote != null)
                {
                    workNote.DateEnd = his.CreateDate;
                    workNote.WorkTime = (his.CreateDate - workNote.DateStart.Value).TotalHours;
                }

            }
            else if (myWork.CycleWork == 3)
            {
                if (myWork.EndPause.Value.Hour < 17 || (myWork.EndPause.Value.Hour == 17 && myWork.EndPause.Value.Minute < 5))
                {
                    myWork.WorkTime = myWork.WorkTime + (SpaceTimeOnDay.CalSpaceTimeOnDay(myWork.EndPause.Value, DateTime.Now) / 60);
                }
              
                myWork.CycleWork = 2;
                CV_QT_StartPauseHistory his = new CV_QT_StartPauseHistory(); // lưu vào bảng lịch sử
                his.MyWorkId = myWork.Id;
                his.CreateDate = DateTime.Now;
                his.CycleWork = 2;
                his.UserCreateId = myWork.UserTaskId;
                _context.CV_QT_StartPauseHistory.Add(his);
                _context.CV_QT_StartPauseHistory.Add(his);
                if (workNote != null)
                {
                    workNote.DateEnd = his.CreateDate;
                    workNote.WorkTime = (his.CreateDate - workNote.DateStart.Value).TotalHours;
                }
            }
            else
            {
                return false;
            }
            _context.SaveChanges();
            return true;
        }
        public static async Task<int> getDepartmentID(humanDbContext _context,int departmentId) // lấy phòng ban từ tổ Id
        {
            int DepId = 0;
            var room =await  _context.Sys_Dm_Department.FindAsync(departmentId);
            if (room.ParentId == null)
            {
                DepId = room.Id;
            }
            else
            {
                DepId = room.ParentId ?? 0;
            }
            return DepId;
        }
        // tính điểm tự động khi đã trình hoàn thành
        public static double countPointAuto(DateTime timeend, DateTime timecomplete, double timeTask)
        {
            int totalTime = 0;
            if (DateTime.Now > timeend)
            {
                totalTime = Convert.ToInt32((timecomplete - timeend).TotalHours);
            }
            switch (timeTask)
            {
                case 1.0:
                    return totalTime * 0.1;
                case 1.1:
                    return totalTime * 0.2;
                case 1.2:
                    return totalTime * 0.3;
                default:
                 return 0.0;
            }
        }
        // tính điểm tự động khi nhắc nhở và gia hạn
        public static double countPointAutoNNGh(DateTime timeend, double timeTask)
        {
            int totalTime = 0;
            if (DateTime.Now > timeend)
            {
              totalTime = Convert.ToInt32((DateTime.Now - timeend).TotalHours);
            }
            switch (timeTask)
            {
                case 1.0:
                    return totalTime * 0.1;
                case 1.1:
                    return totalTime * 0.2;
                case 1.2:
                    return totalTime * 0.3;
                default:
                    return 0.0;
            }
        }
        public static string covertLevelTaskToString(int levelTask)
        {
            switch (levelTask)
            {
                case 1:
                    return "Rất khó";
                case 2:
                    return "Khó";
                case 3:
                    return "Trung bình";
                case 4:
                    return "Dễ";
                default:
                    break;
            }
            return "Trung bình";
        }
        public static string covertLevelTimeToString(int levelTask)
        {
            switch (levelTask)
            {
                case 1:
                    return "Rất vội";
                case 2:
                    return "Vội";
                case 3:
                    return "Bình thường";
                default:
                    break;
            }
            return "Bình thường";
        }
        public static async Task<List<UserAndStatusDeliver>> getUserFromTypeUserDeli(humanDbContext dbContext, int Type, string MyworkId)
        {
            List<UserAndStatusDeliver> list = new List<UserAndStatusDeliver>();
            var cV_QT_MyWork =await dbContext.CV_QT_MyWork.FindAsync(MyworkId);
            if (Type == 1) // người chủ trì
            {
                if (cV_QT_MyWork != null)
                {
                    UserAndStatusDeliver userAndStatus = new UserAndStatusDeliver()
                    {
                        UserId = cV_QT_MyWork.UserTaskId,
                        Type = Type
                    };
                    list.Add(userAndStatus);
                    return list;
                }
                
            } else if(Type == 2) // người phối hợp
            {
                var cV_QT_MySupportWork = dbContext.CV_QT_MySupportWork.Where(x=> x.MyWorkId == MyworkId);
                if (cV_QT_MySupportWork.Count() > 0)
                {
                    foreach (var item in cV_QT_MySupportWork)
                    {
                        UserAndStatusDeliver userAndStatus = new UserAndStatusDeliver()
                        {
                            UserId = item.UserId,
                            Type = Type
                        };
                        list.Add(userAndStatus);
                    }
                    return list;
                }
            } else if(Type == 3) // người làm trước
            {
                if (cV_QT_MyWork != null)
                {
                    if (cV_QT_MyWork.Predecessor != null)
                    {
                        var cV_TQ = dbContext.CV_QT_MyWork.FirstOrDefault(x=>x.Code == cV_QT_MyWork.Predecessor);
                        UserAndStatusDeliver userAndStatus = new UserAndStatusDeliver()
                        {
                            UserId = cV_TQ.UserTaskId,
                            Type = Type
                        };
                        list.Add(userAndStatus);
                        return list;
                    }
                    else
                    {
                        return new List<UserAndStatusDeliver>();
                    }
                  
                }
            }
            return new List<UserAndStatusDeliver>();
        }
        public static double setTimeWorkStep(DateTime timeBegin,int cycleWork, double timeWork, DateTime? timePause)
        {
            double workT = 0.0;
            if (cycleWork == 1 || cycleWork == 3)
            {
                if (timePause == null) 
                {
                    workT = timeWork + (DateTime.Now - timeBegin).TotalHours;
                } else
                {
                    workT = timeWork + (DateTime.Now - timePause.Value).TotalHours;
                }
            } else
            {
                workT = timeWork;
            }
            return workT;
        }
        public static int getTrangThaiKetThucCv(int TypeComplete, DateTime endDate, DateTime completeDate)
        {
            if (TypeComplete == 0)
            {
                if ((DateTime.Now - endDate).TotalHours > 0)
                {
                    return 1; // quá hạn
                }
                else if ((DateTime.Now - endDate).TotalHours < -8.0) // truoc 5 h thi bao chua den han
                {
                    return 2; // chưa đến hạn
                }
                else if ((DateTime.Now - endDate).TotalHours >= -8.0) 
                {
                    return 3; // gần đến hạn
                }
            } else if(TypeComplete == 1)
            {
                if ((completeDate - endDate).TotalHours > 0)
                {
                    return 4; // tạm hoàn thành nhưng quá hạn
                }
                else
                {
                    return 5; // tạm hoàn thành
                }
               
            }
            else if (TypeComplete == 2)
            {
                if ((completeDate - endDate).TotalHours > 0)
                {
                    return 6; // Quá hạn
                }
                else
                {
                    return 7; // Chưa đến hạn
                }

            }
            else if (TypeComplete == 3)
            {
                if ((completeDate - endDate).TotalHours > 0)
                {
                    return 8; // hoàn thành nhưng quá hạn
                }
                else
                {
                    return 9; // hoàn thành
                }

            }
            return 0;
        }
        public static int getTrangThaiKetThucCv(int TypeComplete, DateTime endDate, DateTime completeDate, TypeFlowEnum workflow)
        {
            if (workflow == TypeFlowEnum.CongViecKhoiTaoSau)
            {
                if (TypeComplete == 3)
                {
                    return 9; // hoan thanh
                } else
                {
                    return 5; //tam hoan thanh
                }
                
            }
            if (TypeComplete == 0)
            {
                if ((DateTime.Now - endDate).TotalHours > 0)
                {
                    return 1; // quá hạn
                }
                else if ((DateTime.Now - endDate).TotalHours < -8.0) // truoc 5 h thi bao chua den han
                {
                    return 2; // chưa đến hạn
                }
                else if ((DateTime.Now - endDate).TotalHours >= -8.0)
                {
                    return 3; // gần đến hạn
                }
            }
            else if (TypeComplete == 1)
            {
                if ((completeDate - endDate).TotalHours > 0)
                {
                    return 4; // tạm hoàn thành nhưng quá hạn
                }
                else
                {
                    return 5; // tạm hoàn thành
                }

            }
            else if (TypeComplete == 2)
            {
                if ((completeDate - endDate).TotalHours > 0)
                {
                    return 6; // Quá hạn
                }
                else
                {
                    return 7; // Chưa đến hạn
                }

            }
            else if (TypeComplete == 3)
            {
                if ((completeDate - endDate).TotalHours > 0)
                {
                    return 8; // hoàn thành nhưng quá hạn
                }
                else
                {
                    return 9; // hoàn thành
                }

            }
            return 0;
        }
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int skip, int take)
        {
            return query.Skip(skip).Take(take);
        }
        public static CV_QT_WorkFlow objWorkFlow(humanDbContext dbContext, string MyWorkId,int UserSendId, int UserDeliverId, TypeFlowEnum TypeFlow, string MaLenh, string ParentId, string Note, string Require, int Repossibility)
        {
            var userSend = dbContext.Sys_Dm_User.Find(UserSendId);
            var userDeli = dbContext.Sys_Dm_User.Find(UserDeliverId);
            CV_QT_WorkFlow obj = new CV_QT_WorkFlow();
            obj.DeliverName = userDeli.FullName;
            obj.Id = Helper.GenKey();
            obj.MyWorkId = MyWorkId;
            obj.UserSendId = UserSendId;
            obj.SendName = userSend.FullName;
            obj.UserDeliverId = UserDeliverId;
            obj.SendDate = DateTime.Now;
            obj.TypeFlow = TypeFlow;
            obj.CreateDate = DateTime.Now;
            obj.MaLenh = MaLenh;
            obj.ParentId = ParentId;
            obj.Note = Note;
            obj.Require = Require;
            obj.PositionDeli = userDeli.PositionName;
            obj.PositionSend = userSend.PositionName;
            obj.DepartSend = userSend.DepartmentName;
            obj.DepartDeli = userDeli.DepartmentName;
            obj.Readed = false;
            obj.ReadDate = null;
            obj.Handled = false;
            obj.HandleDate = null;
            obj.Repossibility = Repossibility;
            return obj;
        }
    }
}
