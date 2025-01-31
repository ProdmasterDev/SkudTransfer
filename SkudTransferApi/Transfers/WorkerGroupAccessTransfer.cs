using AutoMapper;
using SkudTransfer.Transfers;
using SkudTransferApi.Contexts;
using Old = ASPWeb.Models;
using New = ControllerDomain.Entities;
using Microsoft.EntityFrameworkCore;
using ControllerDomain.Entities;
using ASPWeb.Models;
using Microsoft.VisualBasic;

namespace SkudTransferApi.Transfers
{
    public class WorkerGroupAccessTransfer : Transfer
    {
        private readonly IMapper _mapper;
        private readonly OldSkudContext _oldContext;
        private readonly NewSkudContext _newContext;
        public WorkerGroupAccessTransfer(IMapper mapper, OldSkudContext oldContext, NewSkudContext newContext)
        {
            _mapper = mapper;
            _oldContext = oldContext;
            _newContext = newContext;
        }
        public async override Task DoTransfer()
        {
            var oldAccess = await _oldContext.Set<Old.Groups>().AsNoTracking().ToListAsync();
            foreach (var group in oldAccess)
            {
                var newGroupId = await _newContext.Set<New.WorkerGroup>().AsNoTracking().FirstOrDefaultAsync(x => x.Name == group.group);
                foreach (var accessGroup in group.accessGroups)
                {
                    var newAccessGroupId = await _newContext.Set<New.AccessGroup>().AsNoTracking().FirstOrDefaultAsync(x => x.Name == accessGroup);
                    if (newAccessGroupId != null && newGroupId != null)
                    {
                        var newAccessGroup = new WorkerGroupAccess() {WorkerGroupId = newGroupId.Id, AccessGroupId = newAccessGroupId.Id, isActive = true };
                        await _newContext.AddAsync(newAccessGroup);
                    }
                }
            }
            await _newContext.SaveChangesAsync();
        }
    }
}
