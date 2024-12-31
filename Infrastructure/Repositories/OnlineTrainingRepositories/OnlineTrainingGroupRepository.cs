using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingDTO;
using Core.Entities.OnlineTrainingEntities;
using Core.Interfaces.Repositories.OnlineTrainingRepositories;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.OnlineTrainingRepositories
{
    public class OnlineTrainingGroupRepository : IOnlineTrainingGroupRepository
    {
        private readonly FitnessContext _context;
        public OnlineTrainingGroupRepository(FitnessContext context)
        {
            _context = context;
        }
        public IntResult Add(AddOnlineTrainingGroupDTO group)
        {
            var newGroup = new OnlineTrainingGroup(group);
            _context.OnlineTrainingGroups.Add(newGroup);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = newGroup.Id };
        }

        public IntResult Delete(int id)
        {
            var group = _context.OnlineTrainingGroups.Find(id);
            if (group is null)
            {
                return new IntResult { Massage = "No Group has this Id" };
            }
            _context.OnlineTrainingGroups.Remove(group);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = 1 };
        }

        public GetOnlineTrainingGroupWithDetailsDTO Get(int id)
        {
            var group = _context.OnlineTrainingGroups.Select(g => new GetOnlineTrainingGroupWithDetailsDTO
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                Price = g.Price,
                OfferPrice = g.OfferPrice,
                NoOfSessionsPerWeek = g.NoOfSessionsPerWeek,
                SubscriptionClosed = g.SubscriptionClosed,
                DurationOfSession = g.DurationOfSession,
                OfferEnded = g.OfferEnded,
                CoachName = g.Coach.FirstName + g.Coach.LastName
            }).FirstOrDefault(g => g.Id == id);
            return group;
        }
        public List<GetOnlineTrainingGroupDTO> ShowAvailableTrainingGroupForCouchPagination(string coachID, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize <= 0)
            {
                pageSize = 5;
            }
            page = Math.Min(page, NumOfPagesForAvailableTrainingGroupsOfCoach(coachID, pageSize));
            var paginate = _context.OnlineTrainingGroups.Where(x => x.CoachID == coachID && x.IsAvailable).Skip(Math.Max((page - 1) * pageSize, 0)).
                Take(pageSize).
                Select(p => new GetOnlineTrainingGroupDTO()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    Price = p.Price,
                    OfferPrice = p.OfferPrice,
                    SubscriptionClosed = p.SubscriptionClosed,
                    OfferEnded = p.OfferEnded,
                    NoOfSessionsPerWeek = p.NoOfSessionsPerWeek,
                    DurationOfSession = p.DurationOfSession

                }).ToList();
            return paginate;
        }
        public int NumOfPagesForAvailableTrainingGroupsOfCoach(string coachID, int pageSize)
        {
            /* var list = _context.OnlineTrainingGroups.Where(x => x.CoachID.Equals(coachID)).ToList();
             var count = (decimal)_context.OnlineTrainingGroups.Where(x => x.CoachID.Equals(coachID)).Count();
             var size = (decimal)pageSize;
             return (int)Math.Ceiling(count / size);*/
            return (int)Math.Ceiling((decimal)_context.OnlineTrainingGroups.Where(x => x.CoachID == coachID && x.IsAvailable).Count() / pageSize);
        }
        public IntResult Update(UpdateOnlineTrainingGroupDTO group, int id)
        {
            var newGroup = _context.OnlineTrainingGroups.Find(id);
            if (newGroup is null)
            {
                return new IntResult { Massage = "No Group has this Id" };
            }
            newGroup.Title = group.Title;
            newGroup.Description = group.Description;
            newGroup.Price = group.Price;
            newGroup.OfferPrice = group.OfferPrice;
            newGroup.NoOfSessionsPerWeek = group.NoOfSessionsPerWeek;
            newGroup.DurationOfSession = group.DurationOfSession;
            newGroup.OfferEnded = group.OfferEnded;
            newGroup.SubscriptionClosed = group.SubscriptionClosed;
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = 1 };
        }
    }
}
