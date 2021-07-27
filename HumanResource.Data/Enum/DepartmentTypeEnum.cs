using System;
using System.Collections.Generic;
using System.Text;

namespace HumanResource.Data.Enum
{
   public enum DepartmentTypeEnum
    {
        /// <summary>
        /// Không thuộc phòng ban nào
        /// </summary>
        None = 0,
        /// <summary>
        /// Thuộc phòng ban
        /// </summary>
        Department = 1,
        /// <summary>
        /// Thuộc tổ trong phòng ban
        /// </summary>
        Nest = 2
    }
}
