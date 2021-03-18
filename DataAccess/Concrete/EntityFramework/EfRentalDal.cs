﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.ComplexTypes;
using Entities.Concrete;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfRentalDal : EfEntityRepositoryBase<Rental, CarRentalDbContext>, IRentalDal
    {
        public List<CarRentalDetails> GetCarRentalDetails(Expression<Func<Rental, bool>> filter)
        {
            using (CarRentalDbContext context = new CarRentalDbContext())
            {
                var result = from r in filter is null ? context.Rentals : context.Rentals.Where(filter)
                    join car in context.Cars on r.CarId equals car.Id
                    join b in context.Brands on car.BrandId equals b.Id
                    join color in context.Colors on car.ColorId equals color.Id
                    join customer in context.Customers on r.CustomerId equals customer.UserId
                    join u in context.Users on customer.UserId equals u.Id
                    select new CarRentalDetails
                    {
                        RentalId = r.Id,
                        CustomerFirstName = u.FirstName,
                        CustomerLastName = u.LastName,
                        CustomerCompanyName = customer.CompanyName,
                        BrandName = b.Name,
                        ColorName = color.Name,
                        CarName = car.CarName,
                        DailyPrice = car.DailyPrice,
                        TotalPrice = Convert.ToDecimal(r.ReturnDate.Value.Day - r.RentDate.Day) * car.DailyPrice,
                        RentDate = r.RentDate,
                        ReturnDate = r.ReturnDate
                    };
                return result.ToList();
            }
        }
    }
}