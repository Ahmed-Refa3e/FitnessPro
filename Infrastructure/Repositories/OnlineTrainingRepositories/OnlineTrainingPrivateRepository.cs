using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingDTO;
using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OnlineTrainingRepositories
{
    //public class OnlineTrainingPrivateRepository : IOnlineTrainingPrivateRepository
    //{
    //    private readonly FitnessContext _context;
    //    public OnlineTrainingPrivateRepository(FitnessContext context)
    //    {
    //        _context = context;
    //    }
    //    public IntResult Add(AddOnlineTrainingPrivateDTO privt)
    //    {
    //        var newPrivt = new OnlineTrainingPrivate(privt);
    //        _context.OnlineTrainingPrivates.Add(newPrivt);
    //        try
    //        {
    //            _context.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            return new IntResult { Massage = ex.Message };
    //        }
    //        return new IntResult { Id = newPrivt.Id };
    //    }

    //    public IntResult Delete(int id)
    //    {
    //        var privt = _context.OnlineTrainingPrivates.Find(id);
    //        if (privt is null)
    //        {
    //            return new IntResult { Massage = "No Private Training has this Id" };
    //        }
    //        _context.OnlineTrainingPrivates.Remove(privt);
    //        try
    //        {
    //            _context.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            return new IntResult { Massage = ex.Message };
    //        }
    //        return new IntResult { Id = 1 };
    //    }

    //    public GetOnlineTrainingPrivateWithDetailsDTO Get(int id)
    //    {
    //        var privt = _context.OnlineTrainingPrivates.Select(p => new GetOnlineTrainingPrivateWithDetailsDTO
    //        {
    //            Id = p.Id,
    //            Title = p.Title,
    //            Description = p.Description,
    //            Price = p.Price,
    //            OfferPrice = p.OfferPrice,
    //            SubscriptionClosed = p.SubscriptionClosed,
    //            OfferEnded = p.OfferEnded,
    //            CoachName = p.Coach.FirstName + p.Coach.LastName
    //        }).FirstOrDefault(p => p.Id == id);
    //        return privt;
    //    }

    //    public List<GetOnlineTrainingPrivateDTO> ShowTrainingPrivateForCouchPagination(string coachID, int page, int pageSize)
    //    {
    //        page = Math.Max(page, 1);
    //        if (pageSize <= 0)
    //        {
    //            pageSize = 5;
    //        }
    //        page = Math.Min(page, NumOfPagesForTrainingPrivateOfCoach(coachID, pageSize));
    //        var paginate = _context.OnlineTrainingPrivates.Where(x => x.CoachID == coachID).Skip(Math.Max((page - 1) * pageSize, 0)).
    //            Take(pageSize).
    //            Select(p => new GetOnlineTrainingPrivateDTO()
    //            {
    //                Id = p.Id,
    //                Title = p.Title,
    //                Description = p.Description,
    //                Price = p.Price,
    //                OfferPrice = p.OfferPrice,
    //                SubscriptionClosed = p.SubscriptionClosed,
    //                OfferEnded = p.OfferEnded
    //            }).ToList();
    //        return paginate;
    //    }
    //    public int NumOfPagesForTrainingPrivateOfCoach(string coachID, int pageSize)
    //    {
    //        return (int)Math.Ceiling((decimal)_context.OnlineTrainingPrivates.Where(x => x.CoachID == coachID).Count() / pageSize);
    //    }
    //    public IntResult Update(UpdateOnlineTrainingPrivateDTO privt, int id)
    //    {
    //        var newPrivt = _context.OnlineTrainingPrivates.Find(id);
    //        if (newPrivt is null)
    //        {
    //            return new IntResult { Massage = "No Group has this Id" };
    //        }
    //        newPrivt.Title = privt.Title;
    //        newPrivt.Description = privt.Description;
    //        newPrivt.Price = privt.Price;
    //        newPrivt.OfferPrice = privt.OfferPrice;
    //        newPrivt.OfferEnded = privt.OfferEnded;
    //        newPrivt.SubscriptionClosed = privt.SubscriptionClosed;
    //        try
    //        {
    //            _context.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            return new IntResult { Massage = ex.Message };
    //        }
    //        return new IntResult { Id = 1 };
    //    }
    //}
}
