using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WBH.Models;

namespace WBH.Helpers
{
    public static class SortHelper
    {
        public static IQueryable<Product> ApplySort(IQueryable<Product> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "price_asc": return query.OrderBy(p => p.Price);
                case "price_desc": return query.OrderByDescending(p => p.Price);
                case "name_asc": return query.OrderBy(p => p.ProductName);
                case "name_desc": return query.OrderByDescending(p => p.ProductName);
                default: return query.OrderBy(p => p.IDProduct);
            }
        }
        // Sắp xếp Order theo nhiều trường
        public static IQueryable<Order> ApplyOrderSort(IQueryable<Order> query, string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder))
                return query
                    .OrderByDescending(o => o.DateOrder) // ngày mới nhất trước
                    .ThenBy(o => o.Status == "Đã hủy" ? 1 :
                                o.Status == "Đang xử lý" ? 2 :
                                o.Status == "Hoàn thành" ? 3 :
                                o.Status == "Chưa xử lý" ? 4 : 5);

            var sortFields = sortOrder.Split(',');
            IOrderedQueryable<Order> orderedQuery = null;

            foreach (var field in sortFields)
            {
                switch (field.Trim().ToLower())
                {
                    case "status_asc":
                        if (orderedQuery == null)
                            orderedQuery = query
                                .OrderBy(o => o.Status == "Đã hủy" ? 1 :
                                              o.Status == "Đang xử lý" ? 2 :
                                              o.Status == "Hoàn thành" ? 3 :
                                              o.Status == "Chưa xử lý" ? 4 : 5)
                                .ThenByDescending(o => o.DateOrder);
                        else
                            orderedQuery = orderedQuery
                                .ThenBy(o => o.Status == "Đã hủy" ? 1 :
                                             o.Status == "Đang xử lý" ? 2 :
                                             o.Status == "Hoàn thành" ? 3 :
                                             o.Status == "Chưa xử lý" ? 4 : 5);
                        break;

                    case "status_desc":
                        if (orderedQuery == null)
                            orderedQuery = query
                                .OrderByDescending(o => o.Status == "Đã hủy" ? 1 :
                                                       o.Status == "Đang xử lý" ? 2 :
                                                       o.Status == "Hoàn thành" ? 3 :
                                                       o.Status == "Chưa xử lý" ? 4 : 5)
                                .ThenByDescending(o => o.DateOrder);
                        else
                            orderedQuery = orderedQuery
                                .ThenByDescending(o => o.Status == "Đã hủy" ? 1 :
                                                          o.Status == "Đang xử lý" ? 2 :
                                                          o.Status == "Hoàn thành" ? 3 :
                                                          o.Status == "Chưa xử lý" ? 4 : 5);
                        break;

                    case "date_asc":
                        if (orderedQuery == null)
                            orderedQuery = query.OrderBy(o => o.DateOrder);
                        else
                            orderedQuery = orderedQuery.ThenBy(o => o.DateOrder);
                        break;

                    case "date_desc":
                        if (orderedQuery == null)
                            orderedQuery = query.OrderByDescending(o => o.DateOrder);
                        else
                            orderedQuery = orderedQuery.ThenByDescending(o => o.DateOrder);
                        break;

                    default:
                        break;
                }
            }

            return orderedQuery ?? query.OrderByDescending(o => o.DateOrder);
        }
    }

}