using Core.DTOs.GeneralDTO;
using Core.DTOs.OnlineTrainingSubscriptionDTO;
using Core.Entities.OnlineTrainingEntities;
using Core.Enums;
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
    public class OnlineTrainingSubscriptionRepository : IOnlineTrainingSubscriptionRepository
    {
        private readonly FitnessContext _context;
        public OnlineTrainingSubscriptionRepository(FitnessContext context)
        {
            _context = context;
        }
        public IntResult Add(AddSubscriptionDTO subscription)
        {
            var training = _context.OnlineTrainings.Find(subscription.OnlineTrainingId);
            if (training is null)
            {
                return new IntResult { Massage = "No Training has this Id" };
            }
            if (training.IsAvailable)
            {
                return new IntResult { Massage = "this Training is not available to subscripe" };
            }
            var oldSubscription = _context.OnlineTrainings.Include(x => x.OnlineTrainingSubscriptions).Where(x => x.Id == subscription.OnlineTrainingId).
                FirstOrDefault().OnlineTrainingSubscriptions.FirstOrDefault(x => x.TraineeID == subscription.TraineeID && x.EndDate >= DateTime.Now);
            if (oldSubscription is not null)
            {
                return new IntResult { Massage = "you already have active subscription in this Online Trining and it will end in" + oldSubscription.EndDate };
            }
            var newSubscription = new OnlineTrainingSubscription(subscription, training);


            // stripe code


            _context.OnlineTrainingSubscriptions.Add(newSubscription);
            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new IntResult { Massage = ex.Message };
            }
            return new IntResult { Id = newSubscription.Id };
        }

        public ShowSubscriptionDTO Get(int id)
        {
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.Id == id).Select(subscription => new ShowSubscriptionDTO
            {
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                Status = subscription.EndDate >= DateTime.Now ? "Active" : "Completed",
                OnlineTrainingId = subscription.OnlineTrainingId,
                OnlineTrainingTitle = subscription.OnlineTraining.Title,
                TraineeID = subscription.TraineeID,
                TraineeName = subscription.Trainee.FirstName + subscription.Trainee.LastName,
                Cost = subscription.Cost
            }).FirstOrDefault();
            return result;
        }

        public List<ShowSubscriptionOfOnlineTrainingDTO> ShowForOnlineTrainingActivePagination(int OnlineTrainingId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfTrainings = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate >= DateTime.Now && x.OnlineTrainingId == OnlineTrainingId).Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfTrainings / pageSize);
            page = Math.Min(page, numOfPage);
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate >= DateTime.Now && x.OnlineTrainingId == OnlineTrainingId).
                Select(x => new ShowSubscriptionOfOnlineTrainingDTO
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.EndDate >= DateTime.Now,
                    TraineeID = x.TraineeID,
                    TraineeName = x.Trainee.FirstName + x.Trainee.LastName
                }).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).ToList();

            return result;

        }

        public List<ShowSubscriptionOfOnlineTrainingDTO> ShowForOnlineTrainingAllPagination(int OnlineTrainingId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfTrainings = _context.OnlineTrainingSubscriptions.Where(x => x.OnlineTrainingId == OnlineTrainingId).Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfTrainings / pageSize);
            page = Math.Min(page, numOfPage);
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.OnlineTrainingId == OnlineTrainingId).
                Select(x => new ShowSubscriptionOfOnlineTrainingDTO
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.EndDate >= DateTime.Now,
                    TraineeID = x.TraineeID,
                    TraineeName = x.Trainee.FirstName + x.Trainee.LastName
                }).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).ToList();
            return result;
        }

        public List<ShowSubscriptionOfOnlineTrainingDTO> ShowForOnlineTrainingCompletedPagination(int OnlineTrainingId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfTrainings = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate < DateTime.Now && x.OnlineTrainingId == OnlineTrainingId).Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfTrainings / pageSize);
            page = Math.Min(page, numOfPage);
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate < DateTime.Now && x.OnlineTrainingId == OnlineTrainingId).
                Select(x => new ShowSubscriptionOfOnlineTrainingDTO
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.EndDate >= DateTime.Now,
                    TraineeID = x.TraineeID,
                    TraineeName = x.Trainee.FirstName + x.Trainee.LastName
                }).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).ToList();
            return result;
        }

        public List<ShowSubscriptionForTraineDTO> ShowForTraineActivePagination(string traineId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfTrainings = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate >= DateTime.Now && x.TraineeID == traineId).Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfTrainings / pageSize);
            page = Math.Min(page, numOfPage);
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate >= DateTime.Now && x.TraineeID == traineId).
                Select(x => new ShowSubscriptionForTraineDTO
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.EndDate >= DateTime.Now,
                    CoachId = x.OnlineTraining.CoachID,
                    CoachName = x.OnlineTraining.Coach.FirstName + x.OnlineTraining.Coach.LastName,
                    OnlineTrainingId = x.OnlineTrainingId,
                    OnlineTrainingTitle = x.OnlineTraining.Title
                }).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).ToList();
            return result;
        }

        public List<ShowSubscriptionForTraineDTO> ShowForTraineAllPagination(string traineId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfTrainings = _context.OnlineTrainingSubscriptions.Where(x => x.TraineeID == traineId).Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfTrainings / pageSize);
            page = Math.Min(page, numOfPage);
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.TraineeID == traineId).
                Select(x => new ShowSubscriptionForTraineDTO
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.EndDate >= DateTime.Now,
                    CoachId = x.OnlineTraining.CoachID,
                    CoachName = x.OnlineTraining.Coach.FirstName + x.OnlineTraining.Coach.LastName,
                    OnlineTrainingId = x.OnlineTrainingId,
                    OnlineTrainingTitle = x.OnlineTraining.Title
                }).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).ToList();

            return result;
        }
        public List<ShowSubscriptionForTraineDTO> ShowForTraineCompletedPagination(string traineId, int page, int pageSize)
        {
            page = Math.Max(page, 1);
            if (pageSize == 0)
            {
                pageSize = 10;
            }
            var numOfTrainings = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate < DateTime.Now && x.TraineeID == traineId).Count();
            var numOfPage = (int)Math.Ceiling((decimal)numOfTrainings / pageSize);
            page = Math.Min(page, numOfPage);
            var result = _context.OnlineTrainingSubscriptions.Where(x => x.EndDate < DateTime.Now && x.TraineeID == traineId).
                Select(x => new ShowSubscriptionForTraineDTO
                {
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    IsActive = x.EndDate >= DateTime.Now,
                    CoachId = x.OnlineTraining.CoachID,
                    CoachName = x.OnlineTraining.Coach.FirstName + x.OnlineTraining.Coach.LastName,
                    OnlineTrainingId = x.OnlineTrainingId,
                    OnlineTrainingTitle = x.OnlineTraining.Title
                }).Skip(Math.Max((page - 1) * pageSize, 0)).Take(pageSize).ToList();
            return result;
        }
    }
}
