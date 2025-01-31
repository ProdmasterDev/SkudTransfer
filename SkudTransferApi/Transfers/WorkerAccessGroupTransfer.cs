using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using ControllerDomain.Entities;

namespace SkudTransferApi.Transfers
{
    public class WorkerAccessGroupTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public WorkerAccessGroupTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var oldWorkesAccessGroup = await _oldContext.Set<Old.Workers>().AsNoTracking().Where(x => x.personalCheck == 2).ToListAsync();

            foreach (var worker in oldWorkesAccessGroup)
            {
                var newWorkerId = await _newContext.Set<New.Worker>().AsNoTracking().FirstOrDefaultAsync(x=>x.LastName == worker.LastName);
                foreach(var accessGroup in worker.accessGroups)
                {
                    var newAccessGroupId = await _newContext.Set<New.AccessGroup>().AsNoTracking().FirstOrDefaultAsync(x => x.Name == accessGroup);
                    if (newAccessGroupId != null && newWorkerId != null)
                    {
                        var newAccessGroup = new WorkerAccessGroup() { WorkerId = newWorkerId.Id, AccessGroupId = newAccessGroupId.Id, isActive = true };
                        await _newContext.AddAsync(newAccessGroup);
                    }
                }
            }
           
           

            await _newContext.SaveChangesAsync();
        }
    }
}
