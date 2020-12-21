using System.Collections.Generic;
using System.Linq;
using System;
using WebAPI.Models;

namespace WebAPI.Common
{
    public static class Extensions
    {
        public static List<T> ToNonNullList<T>(this IEnumerable<T> obj)
        {
            return obj == null ? new List<T>() : obj.ToList();
        }
        
        /// <summary>
        /// Check StartDate and EndDate in Project
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool CheckDateTime(this Project project)
        {
            bool isSuccess = false;
            int check = DateTime.Compare(project.StartDate, project.EndDate);
            switch (check)
            {
                case int n when n < 0:
                    isSuccess = true;
                    break;
                case int n when n > 0:
                    isSuccess = false;
                    break;
                default:
                    break;
            }
            return isSuccess;
        }
    }
}
