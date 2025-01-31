using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using ControllerDomain.Entities;
using System.Threading;
using System.Linq;

namespace SkudTransferApi.Transfers
{
    public class RefreshQuickAccessTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public RefreshQuickAccessTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var dbWorker = await _newContext
                .Set<New.Worker>()
                .Include(x => x.Cards)
                .Include(x => x.Accesses)
                    .ThenInclude(x => x.ControllerLocation)
                        .ThenInclude(x => x!.Controller)
                .Include(x => x.Group)
                    .ThenInclude(x => x.GroupAccess)
                        .ThenInclude(x => x.ControllerLocation)
                            .ThenInclude(x => x.Controller)
                .Include(x => x.AccessMethod)
                .Include(x => x.WorkerAccessGroup)
                    .ThenInclude(x => x.AccessGroup)
                        .ThenInclude(x => x.Accesses)
                            .ThenInclude(x => x.ControllerLocation)
                                .ThenInclude(x => x.Controller)
                .ToListAsync();

            if (dbWorker == null) { return; }
            foreach (var worker in dbWorker)
            {
                List<New.QuickAccess> newQuickAccesses = new();


                if (worker.AccessMethodId == 3)
                {
                    worker.Accesses.ToList().ForEach(x => AddAccessToList(newQuickAccesses, x, string.Empty, worker.DateBlock));
                }

                if (worker.AccessMethodId == 1 && worker.Group != null)
                {
                    //worker.Group.GroupAccess.ToList().ForEach(x => AddGroupAccessToList(newQuickAccesses, x, string.Empty, worker.DateBlock));
                    var workerGroupAccess = await _newContext
                      .Set<New.WorkerGroupAccess>()
                      .Include(x => x.AccessGroup)
                          .ThenInclude(x => x.Accesses)
                              .ThenInclude(x => x.ControllerLocation)
                                      .ThenInclude(x => x.Controller)
                                    .AsNoTracking()
                      .Where(x => x.WorkerGroupId == worker.GroupId)
                      .ToListAsync();
                    foreach (var groupsAccess in workerGroupAccess)
                    {
                        if (groupsAccess.isActive)
                        {
                            var accessGroup = groupsAccess.AccessGroup.Accesses;
                            accessGroup.ToList().ForEach(x => AddAccessGroupToList(newQuickAccesses, x, string.Empty, worker.DateBlock));
                        }
                    }
                }
                if (worker.AccessMethodId == 2 && worker.WorkerAccessGroup != null)
                {
                    foreach (var workerAccessGroup in worker.WorkerAccessGroup)
                    {
                        if (workerAccessGroup.isActive)
                        {
                            var accessGroup = workerAccessGroup.AccessGroup.Accesses;
                            accessGroup.ToList().ForEach(x => AddAccessGroupToList(newQuickAccesses, x, string.Empty, worker.DateBlock));
                        }
                    }
                }

                var quickAccessesRequest = _newContext.Set<New.QuickAccess>();
                foreach (var card in worker.Cards)
                {
                    if (newQuickAccesses.Count == 0)
                    {
                        var dbQuickAccesses = await quickAccessesRequest.Where(x => x.Card == card.CardNumb16).ToListAsync();
                        dbQuickAccesses.ForEach(x => x.Granted = 0);
                        _newContext.UpdateRange(dbQuickAccesses);
                    }
                    foreach (var qa in newQuickAccesses)
                    {
                        var dbQuickAccess = await quickAccessesRequest.FirstOrDefaultAsync(x => x.Sn == qa.Sn && x.Reader == qa.Reader && x.Card == card.CardNumb16);
                        if (dbQuickAccess != null)
                        {
                            if (dbQuickAccess.Granted != qa.Granted || dbQuickAccess.DateBlock != qa.DateBlock)
                            {
                                dbQuickAccess.Granted = qa.Granted;
                                dbQuickAccess.DateBlock = qa.DateBlock;
                                _newContext.Update(dbQuickAccess);
                            }
                        }
                        else
                        {
                            var dateBlock = worker.DateBlock;
                            if (dateBlock != null)
                                dateBlock = dateBlock.Value.ToUniversalTime();
                            dbQuickAccess = new New.QuickAccess() { Sn = qa.Sn, Reader = qa.Reader, Card = card.CardNumb16, DateBlock = dateBlock, Granted=1};
                            await _newContext.AddAsync(dbQuickAccess);
                        }
                    }
                }
            }
            try
            {
                await _newContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
        }
        private void AddAccessToList(List<New.QuickAccess> accesses, New.Access access, string card, DateTime? dateBlock)
        {
            if (access.ControllerLocation != null && access.ControllerLocation.Controller != null)
            {
                for (var readerNumber = 1; readerNumber <= 2; readerNumber++)
                {
                    if (dateBlock != null)
                        dateBlock = dateBlock.Value.ToUniversalTime();
                    accesses.Add(_mapper.Map(access, new New.QuickAccess() { Id = default, Reader = readerNumber, Card = card, DateBlock = dateBlock }));
                }
            }
        }
        private void AddGroupAccessToList(List<New.QuickAccess> accesses, New.GroupAccess access, string card, DateTime? dateBlock)
        {
            if (access.ControllerLocation != null && access.ControllerLocation.Controller != null)
            {
                for (var readerNumber = 1; readerNumber <= 2; readerNumber++)
                {
                    if (dateBlock != null)
                        dateBlock = dateBlock.Value.ToUniversalTime();
                    accesses.Add(_mapper.Map(access, new New.QuickAccess() { Id = default, Reader = readerNumber, Card = card, DateBlock = dateBlock }));
                }
            }
        }
        private void AddAccessGroupToList(List<New.QuickAccess> accesses, New.AccessGroupAccess access, string card, DateTime? dateBlock)
        {
            if (access.ControllerLocation != null && access.ControllerLocation.Controller != null)
            {
                for (var readerNumber = 1; readerNumber <= 2; readerNumber++)
                {
                    if (dateBlock != null)
                        dateBlock = dateBlock.Value.ToUniversalTime();
                    var oldQuickAccess = accesses.FirstOrDefault(x => x.Sn == access.ControllerLocation.Controller.Sn && x.Reader == readerNumber);
                    if (oldQuickAccess == null || oldQuickAccess.Granted == 0)
                    {
                        accesses.Add(_mapper.Map(access, new New.QuickAccess() { Id = default, Reader = readerNumber, Card = card, DateBlock = dateBlock }));
                    }
                }

            }
        }
    }
}
